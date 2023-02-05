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

    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class Human : MonoBehaviour
    {
        private Dictionary<HumanNeed, Transform> m_NeedsRoomTx = new();
        private List<Transform> m_Escapes = new();
        [SerializeField]
        private HumanDataScriptableObject m_HumanData;

        private BaseState m_CurrentState;

        [SerializeField]
        private Transform m_TargetFollowPoint;

        public Animator Animator;
        public BaseState IdleState;
        public BaseState MovingState;
        public BaseState TaskState;
        public BaseState HauntedState;

        public HumanNeed CurrentTask = HumanNeed.Error;

        // TODO: fix all these
        bool m_StartedWait = false;
        [HideInInspector]
        public bool Continue;

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
                m_CurrentState.Enter();
            }
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
            // Needs fulfilment
            m_HumanData.UpdateNeeds();

            // State machine logic
            if (m_CurrentState != null)
            {
                m_CurrentState.UpdateLogic();
            }
        }

        /// <summary>
        /// Sets the new CurrentTask based on needs values and sets the NavMesh waypoint
        /// </summary>
        /// <param name="wasHaunted">Is this calculation being called after being Haunted?</param>
        /// <param name="onStart">Was this called in MonoBehaviour Start()?</param>
        public void CalculateNextTask(bool wasHaunted, bool onStart = false)
        {
            HumanManager.UpdateOccupancy(CurrentTask, false);
            CurrentTask = m_HumanData.GetCurrentNeed(onStart, wasHaunted, m_CurrentState.StateType);
            HumanManager.UpdateOccupancy(CurrentTask, true);
            m_TargetFollowPoint.position = m_NeedsRoomTx[CurrentTask].position;
        }

        public void ChangeState(BaseState newState, bool isHaunted)
        {
            m_CurrentState.Exit(isHaunted);

            m_CurrentState = newState;
            m_CurrentState.Enter();
        }

        private BaseState GetInitialState()
        {
            return IdleState;
        }

        public void BeginHaunt(float amount, HauntType haunt)
        {
            HumanManager.UpdateOccupancy(CurrentTask, false);
            m_TargetFollowPoint.position = m_NeedsRoomTx[HumanNeed.Haunted].position;
            CurrentTask = HumanNeed.Haunted;
            ChangeState(HauntedState, true);
            var haunted = m_HumanData.GetHaunted(amount, haunt);
            if (haunted)
            {
                BeginEscape();
                PauseAllNeeds(false); // TODO ?
            }
        }

        public void BeginEscape()
        {
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
            ChangeState(MovingState, true);
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

        public void RefillNeed()
        {
            m_HumanData.RefillNeed(CurrentTask);
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
                Gizmos.DrawCube(transform.position + new Vector3(0, 1.5f, 0), Vector3.one);

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
    }


}
