using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR // for Handles.Label debugging
using UnityEditor;
#endif

namespace Humans
{
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

        public HumanNeed LowestNeed;

        // TODO: fix all these
        private Transform WaypointsParent;
        bool m_StartedWait = false;
        [HideInInspector]
        public bool Continue;

        private void Start()
        {
            // TODO: clean this up
            WaypointsParent = GameObject.Find("House/Waypoints").GetComponent<Transform>();
            m_NeedsRoomTx.Add(HumanNeed.Hunger, WaypointsParent.Find("Kitchen"));
            m_NeedsRoomTx.Add(HumanNeed.Bathroom, WaypointsParent.Find("Bathroom"));
            m_NeedsRoomTx.Add(HumanNeed.Sleep, WaypointsParent.Find("Couch"));

            IdleState = new Idle(this, m_TargetFollowPoint, WaypointsParent);
            MovingState = new Moving(this, m_TargetFollowPoint, WaypointsParent);
            TaskState = new Task(this, m_TargetFollowPoint, WaypointsParent);

            m_CurrentState = GetInitialState();
            CalculateNextTask();
            if (m_CurrentState != null)
            {
                m_CurrentState.Enter();
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

        public void CalculateNextTask()
        {
            // TODO: move this
            LowestNeed = m_HumanData.GetCurrentNeed();
            m_TargetFollowPoint.position = m_NeedsRoomTx[LowestNeed].position;
        }

        public void ChangeState(BaseState newState)
        {
            m_CurrentState.Exit();

            m_CurrentState = newState;
            m_CurrentState.Enter();
        }

        private BaseState GetInitialState()
        {
            return IdleState;
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
            m_HumanData.PauseNeed(LowestNeed, pause);
        }

        public void RefillNeed()
        {
            m_HumanData.RefillNeed(LowestNeed);
        }

        // DEBUG
        private readonly Dictionary<string, Color> m_StateToColor = new()
        {
            { "Idle", Color.gray },
            { "Moving", Color.blue },
            { "Task", Color.yellow }
        };

        private void OnDrawGizmos()
        {
            if (m_CurrentState != null)
            {
                Gizmos.color = m_StateToColor[m_CurrentState.StateName];
                Gizmos.DrawCube(transform.position + new Vector3(0, 1.5f, 0), Vector3.one);

#if UNITY_EDITOR
                Handles.color = Color.white;
                var val = "";
                foreach (var i in m_HumanData.NeedStatus)
                {
                    val += $"{i.NeedType}=>{i.CurrentValue}\n";
                }
                Handles.Label(transform.position + Vector3.up * 2, val);
#endif

                // Target
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_TargetFollowPoint.position + Vector3.up, 1f);
#if UNITY_EDITOR
                Handles.Label(m_TargetFollowPoint.position + Vector3.up * 2, m_NeedsRoomTx[LowestNeed].gameObject.name);
#endif
            }
        }
    }


}
