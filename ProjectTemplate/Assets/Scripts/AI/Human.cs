using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
#if UNITY_EDITOR // for Handles.Label debugging
using UnityEditor;
#endif

namespace Humans
{
    public enum HauntType
    {
        Bathroom, Bedroom, Kitchen, LivingRoom
    }

    public enum HumanName
    {
        Sleepyhead, CuriousKat, Gourmando
    }

    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class Human : MonoBehaviour
    {
        public static readonly float k_DistThreshold = 0.6f;

        public readonly static float WalkSpeed = 2f;
        public readonly static float RunSpeed = 4f;

        private Dictionary<HumanNeed, Transform> m_NeedsRoomTx = new();
        private List<Transform> m_Escapes = new();
        [SerializeField]
        private HumanDataScriptableObject m_HumanData;

        private BaseState m_CurrentState;

        [SerializeField]
        private Transform m_TargetFollowPoint;

        [HideInInspector]
        public Animator Animator;
        public BaseState IdleState;
        public BaseState MovingState;
        public BaseState TaskState;
        public BaseState HauntedState;

        public HumanNeed CurrentTask = HumanNeed.Error;
        public HumanNeed TaskBeforeHaunt = HumanNeed.Error;

        [SerializeField]
        private HumanName m_HumanName = HumanName.Sleepyhead;

        public bool Hauntable = true;
        public bool Escaped { get; set; }
        public bool Killed { get; private set; }

        bool m_StartedWait = false;
        [HideInInspector]
        public bool Continue;

        private HumanNeed m_TaskLastFrame;
        private NavMeshAgent m_Agent;
        private bool m_LastPausedState;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        private void Start()
        {
            IdleState = new Idle(this, m_TargetFollowPoint);
            MovingState = new Moving(this, m_TargetFollowPoint);
            TaskState = new Task(this, m_TargetFollowPoint);
            HauntedState = new Haunted(this, m_TargetFollowPoint);

            m_CurrentState = GetInitialState();
            CalculateNextTask(false, onStart: true);
            if (m_CurrentState != null)
            {
                m_CurrentState.Enter(isEscaping: false);
                UIManager.Instance.HumanThought(m_HumanName, m_HumanData.FirstNeed);
            }

            m_Agent = GetComponent<NavMeshAgent>();
        }

        public void Initialize(WaypointsDict waypoints)
        {
            m_NeedsRoomTx.Add(HumanNeed.Hunger, waypoints.KitchenWaypoint);
            m_NeedsRoomTx.Add(HumanNeed.Bathroom,waypoints.BathroomWaypoint);
            m_NeedsRoomTx.Add(HumanNeed.Sleep, waypoints.BedWaypoint);
            m_NeedsRoomTx.Add(HumanNeed.Curious, waypoints.LivingRoomWaypoint);
            m_NeedsRoomTx.Add(HumanNeed.Haunted, waypoints.HallwayWaypoint);

            foreach (var e in waypoints.EscapeWaypoints)
            {
                m_Escapes.Add(e);
            }
        }

        private void Update()
        {
            if (!Killed)
            {
                if (MenuHandler.IsGamePaused && !m_LastPausedState)
                {
                    m_Agent.isStopped = true;
                }

                if (!MenuHandler.IsGamePaused && m_LastPausedState)
                {
                    m_Agent.isStopped = false;
                }

                m_LastPausedState = MenuHandler.IsGamePaused;

                if (m_LastPausedState) return;

                // Needs fulfilment
                m_HumanData.UpdateNeeds();

                // State machine logic
                if (m_CurrentState != null)
                {
                    m_CurrentState.UpdateLogic();
                }

                if (CurrentTask != m_TaskLastFrame)
                {
                    UIManager.Instance.HumanThought(m_HumanName, CurrentTask);
                }

                m_TaskLastFrame = CurrentTask;
            }
        }

        /// <summary>
        /// Sets the new CurrentTask based on needs values and sets the NavMesh waypoint
        /// </summary>
        /// <param name="wasHaunted">Is this calculation being called after being Haunted?</param>
        /// <param name="onStart">Was this called in MonoBehaviour Start()?</param>
        public void CalculateNextTask(bool wasHaunted, bool onStart = false)
        {
            if (!onStart)
            {
                HumanManager.UpdateOccupancy(CurrentTask, false);
            }
            CurrentTask = m_HumanData.GetCurrentNeed(onStart, wasHaunted, m_CurrentState.StateType);
            HumanManager.UpdateOccupancy(CurrentTask, true);
            m_TargetFollowPoint.position = m_NeedsRoomTx[CurrentTask].position;
        }

        public void ChangeState(BaseState newState, bool isHaunted, bool isEscaping = false)
        {
            m_CurrentState.Exit(isHaunted);

            m_CurrentState = newState;
            m_CurrentState.Enter(isEscaping);

            if (newState == HauntedState || isHaunted || isEscaping)
            {
                UIManager.Instance.HumanInDanger(m_HumanName);
            }
            else if (!Animator.GetBool(BaseState.NeedToAnimName[HumanNeed.Haunted]) && m_HumanData.CurrentFear < 70)
            {
                UIManager.Instance.HumanHealthy(m_HumanName);
            }
        }

        private BaseState GetInitialState()
        {
            return IdleState;
        }

        public void BeginHaunt(float amount, HauntType haunt)
        {
            Hauntable = false;
            RefillNeed(haunted: true, HumanDataScriptableObject.HauntToNeed[haunt]); // Discourage npc from coming back too soon
            TaskBeforeHaunt = CurrentTask;
            HumanManager.UpdateOccupancy(TaskBeforeHaunt, false);
            m_TargetFollowPoint.position = m_NeedsRoomTx[HumanNeed.Haunted].position;
            CurrentTask = HumanNeed.Haunted;
            ChangeState(HauntedState, true);
            var haunted = m_HumanData.GetHaunted(amount, haunt);
            Animator.SetFloat("fear", Animator.GetFloat("fear") + amount);
            if (haunted)
            {
                BeginEscape();
                PauseAllNeeds(false);
            }
        }

        public void BeginEscape()
        {
            StartCoroutine(RunAwayCamera());
            Transform closestEscape = m_Escapes[0];
            NavMeshPath temp = new NavMeshPath();
            float minDist = Int32.MaxValue;
            foreach (var e in m_Escapes)
            {
                NavMesh.CalculatePath(transform.position, e.position, NavMesh.AllAreas, temp);
                float length = 0;
                for (int i = 1; i < temp.corners.Length; i++)
                {
                    length += Vector3.Distance(temp.corners[i - 1], temp.corners[i]);
                }
                if (length < minDist)
                {
                    closestEscape = e;
                    minDist = length;
                }
            }
            m_TargetFollowPoint.position = closestEscape.position;
            ChangeState(MovingState, isHaunted: true, isEscaping: true);
        }

        public void TimeUpRunAway()
        {
            BeginEscape();
            PauseAllNeeds(false);
        }

        public void TestWait(float time)
        {
            if (!m_StartedWait)
            {
                Continue = false;
                m_StartedWait = true;
                StartCoroutine(Wait(time));
            }
        }

        // TEMP
        IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
            Continue = true;
            m_CurrentState.UpdateLogic();
            m_StartedWait = false;
        }

        public void PauseNeed(bool pause)
        {
            m_HumanData.PauseNeed(CurrentTask, pause);
        }

        public void PauseAllNeeds(bool pause)
        {
            m_HumanData.PauseAllNeeds(pause);
        }

        public void RefillNeed(bool haunted = false, HumanNeed hauntedRoom = HumanNeed.Error)
        {
            if (haunted)
            {
                m_HumanData.RefillNeed(hauntedRoom);
                return;
            }
            m_HumanData.RefillNeed(CurrentTask);
        }

        public void PrepareKill(HauntType hauntType)
        {
            Killed = true;
            GetComponent<NavMeshAgent>().isStopped = true;
            if (hauntType == HauntType.Kitchen || hauntType == HauntType.LivingRoom)
            {
                GetComponent<NavMeshAgent>().enabled = false;
            }
            StartCoroutine(MoveToKill(hauntType));
        }

        public void Kill(float killTime, HauntType hauntType)
        {
            Debug.Log($"not allowikng current task {HumanDataScriptableObject.HauntToNeed[hauntType]}");
            HumanManager.UpdateOccupancy(HumanDataScriptableObject.HauntToNeed[hauntType], true);
            // TODO: recalculate goals?
            StartCoroutine(Deactivate(killTime, hauntType));
            UIManager.Instance.KillHuman(m_HumanName);
            HumanManager.Instance.CheckGameOver();
        }

        public IEnumerator MoveToKill(HauntType hauntType)
        {
            var start = transform.position;
            var goalTaskPos = m_NeedsRoomTx[HumanDataScriptableObject.HauntToNeed[hauntType]].position;
            var t = 0f;
            var len = 1f;
            while (t <= 1)
            {
                t += Time.deltaTime / len;
                transform.position = Vector3.Lerp(start, goalTaskPos, t);
                yield return null;
            }
            if (BaseState.NeedToHardcodeRotation.TryGetValue(CurrentTask, out var rot))
            {
                transform.rotation = Quaternion.Euler(rot);
            }

            transform.position = goalTaskPos;
        }

        private IEnumerator Deactivate(float time, HauntType hauntType)
        {
            if (hauntType == HauntType.Kitchen)
            {
                time -= 8f;
            }
            yield return new WaitForSeconds(time);
            HumanManager.UpdateOccupancy(HumanDataScriptableObject.HauntToNeed[hauntType], false);
            // blastoff!
            if (hauntType == HauntType.Kitchen || hauntType == HauntType.LivingRoom)
            {
                GetComponent<Animator>().enabled = false;
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;
                switch (hauntType)
                {
                    case HauntType.LivingRoom:
                        rb.AddForce((-transform.forward + transform.up) * 10f , ForceMode.Impulse);
                        break;
                    case HauntType.Kitchen:
                        rb.AddForce((-transform.right) * 10f , ForceMode.Impulse);
                        break;
                }

                this.enabled = false;
            }
            else
            {
                gameObject.SetActive(false);
            }

        }

        // DEBUG
        private readonly Dictionary<StateType, Color> m_StateToColor = new()
        {
            { StateType.Idle, Color.gray },
            { StateType.Moving, Color.blue },
            { StateType.Task, Color.yellow },
            { StateType.Haunted, Color.red },
        };

        private void OnDrawGizmos()
        {
            if (m_CurrentState != null)
            {
                Gizmos.color = m_StateToColor[m_CurrentState.StateType];
                Gizmos.DrawWireCube(transform.position + new Vector3(0, 1.5f, 0), Vector3.one);

#if UNITY_EDITOR
                Handles.color = Color.white;
                var val = "";
                val += $"Haunt: {m_HumanData.CurrentFear}\n";
                if (m_HumanData.TaskList.Count > 0)
                {
                    val += $"Peek task: {m_HumanData.TaskList.Peek()}\n";
                }
                foreach (var i in m_HumanData.NeedStatus)
                {
                    val += $"{i.NeedType}=>{i.CurrentValue}\n";
                }
                Handles.Label(transform.position + Vector3.up * 3, val);
#endif

                // Target
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(m_TargetFollowPoint.position + Vector3.up, 1f);
#if UNITY_EDITOR
                Handles.Label(m_TargetFollowPoint.position + Vector3.up * 2, m_NeedsRoomTx[CurrentTask].gameObject.name);
#endif
            }
        }

        private IEnumerator RunAwayCamera()
        {
            yield return new WaitForSeconds(3f);
            GameplayManager.Instance.NPCFollowCam.SetActive(true);
            yield return new WaitForSeconds(3f);
            GameplayManager.Instance.NPCFollowCam.SetActive(false);
        }
    }
}
