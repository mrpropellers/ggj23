using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Humans
{
    public class BaseState
    {
        private Transform m_Waypoints;
        protected Human m_Human;
        protected NavMeshAgent m_Agent;
        protected Transform m_Target;
        public string StateName;

        public BaseState(Human human, string name, Transform target, Transform waypoints)
        {
            m_Human = human;
            m_Agent = m_Human.GetComponent<NavMeshAgent>(); // TODO: clean this up
            StateName = name;
            m_Target = target;
            m_Waypoints = waypoints;
        }

        public virtual void Enter() { }
        public virtual void UpdateLogic()
        {
            // TODO: calculate task queue
        }
        public virtual void Exit() { }
    }

    public class Idle : BaseState
    {
        public Idle(Human human, Transform target, Transform waypoints) : base(human, "Idle", target, waypoints) { }

        public override void Enter()
        {
            base.Enter();
            m_Agent.isStopped = true;
            // TODO: animation
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            // TODO: animation
            // TODO: stop moving
            m_Human.TestWait(2f);
            if (m_Human.Continue)
            {
                m_Human.ChangeState(m_Human.MovingState);
            }
            // TODO: check if should switch??
        }
    }

    public class Moving : BaseState
    {
        private const float k_DistThreshold = 1.5f;

        public Moving(Human human, Transform target, Transform waypoints) : base(human, "Moving", target, waypoints) { }

        public override void Enter()
        {
            base.Enter();
            if (m_Target != null)
            {
                m_Agent.destination = m_Target.position;
            }
            // TODO: animation
            m_Agent.isStopped = false;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            // TODO: animation
            if (Vector3.Distance(m_Target.position, m_Human.transform.position) < k_DistThreshold)
            {
                m_Human.ChangeState(m_Human.TaskState);
            }
        }
    }

    public class Task : BaseState
    {
        public Task(Human human, Transform target, Transform waypoints) : base(human, "Task", target, waypoints) { }

        public override void Enter()
        {
            base.Enter();
            // TODO: animation
            m_Agent.isStopped = true;

            // TODO: stop decrementing this need
            m_Human.PauseNeed(true);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            // TODO: animation
            // TODO: task!
            m_Human.TestWait(2f);
            if (m_Human.Continue)
            {
                m_Human.ChangeState(m_Human.IdleState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            m_Human.RefillNeed();
            m_Human.PauseNeed(false);
            m_Human.CalculateNextTask();
        }
    }

}
