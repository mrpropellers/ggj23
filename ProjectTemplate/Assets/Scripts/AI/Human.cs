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
    // TEMP
    public enum HauntType
    {
        Bathroom, Bedroom, Kitchen, LivingRoom
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class Human : MonoBehaviour
    {
        private Dictionary<HumanNeed, Transform> m_NeedsRoomTx = new();
        [SerializeField]
        private HumanDataScriptableObject m_HumanData;

        private BaseState m_CurrentState;

        [SerializeField]
        private Transform m_TargetFollowPoint;

        public BaseState IdleState;
        public BaseState MovingState;
        public BaseState TaskState;
        public BaseState HauntedState;

        public HumanNeed CurrentTask;

        // TODO: fix all these
        bool m_StartedWait = false;
        [HideInInspector]
        public bool Continue;

        private void Start()
        {
            IdleState = new Idle(this, m_TargetFollowPoint);
            MovingState = new Moving(this, m_TargetFollowPoint);
            TaskState = new Task(this, m_TargetFollowPoint);
            HauntedState = new Haunted(this, m_TargetFollowPoint);

            m_CurrentState = GetInitialState();
            if (m_CurrentState != null)
            {
                m_CurrentState.Enter();
            }
            CalculateNextTask(true);
        }

        public void Initialize(WaypointsDict waypoints)
        {
            m_NeedsRoomTx.Add(HumanNeed.Hunger, waypoints.KitchenWaypoint);
            m_NeedsRoomTx.Add(HumanNeed.Bathroom,waypoints.BathroomWaypoint);
            m_NeedsRoomTx.Add(HumanNeed.Sleep, waypoints.BedWaypoint);
            m_NeedsRoomTx.Add(HumanNeed.Haunted, waypoints.HallwayWaypoint);
        }

        private void Update()
        {
            // Needs fulfilment
            m_HumanData.UpdateNeeds();
            //m_HumanData.GetCurrentNeed(false, m_CurrentState.StateType);

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
            // TODO: do something else if haunted
            CurrentTask = m_HumanData.GetCurrentNeed(onStart, wasHaunted, m_CurrentState.StateType);
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
            // TODO: stop current task
            m_TargetFollowPoint.position = m_NeedsRoomTx[HumanNeed.Haunted].position;
            ChangeState(HauntedState, true);
            // TODO: fear calc
            m_HumanData.GetHaunted(amount, haunt);
            // TODO: move to hallway
            // TODO: wait for time in hallway ("fear task"?)
            // TODO: decrease need decrement
            // TODO: start secondary task

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
            { StateType.Task, Color.yellow }
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
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_TargetFollowPoint.position + Vector3.up, 1f);
#if UNITY_EDITOR
                Handles.Label(m_TargetFollowPoint.position + Vector3.up * 2, m_NeedsRoomTx[CurrentTask].gameObject.name);
#endif
            }
        }
    }


}
