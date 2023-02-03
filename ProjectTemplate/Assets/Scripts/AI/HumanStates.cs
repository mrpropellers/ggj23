using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Humans
{
    public enum StateType
    {
        Idle, Moving, Task, Haunted
    }

    public abstract class BaseState
    {
        protected Human m_Human;
        protected NavMeshAgent m_Agent;
        protected Transform m_Target;
        public StateType StateType;

        public BaseState(Human human, StateType name, Transform target)
        {
            m_Human = human;
            m_Agent = m_Human.GetComponent<NavMeshAgent>(); // TODO: clean this up
            StateType = name;
            m_Target = target;
        }

        public virtual void Enter() { }
        public virtual void UpdateLogic()
        {
            // TODO: calculate task queue
        }
        public virtual void Exit(bool isHaunted) { }
    }

    public class Idle : BaseState
    {
        public Idle(Human human, Transform target) : base(human, StateType.Idle, target) { }

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
                m_Human.ChangeState(m_Human.MovingState, false);
            }
            // TODO: check if should switch??
        }
    }

    public class Moving : BaseState
    {
        private const float k_DistThreshold = 1.5f;

        public Moving(Human human, Transform target) : base(human, StateType.Moving, target) { }

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
                m_Human.ChangeState(m_Human.TaskState, false);
            }
        }
    }

    public class Task : BaseState
    {
        public Task(Human human, Transform target) : base(human, StateType.Task, target) { }

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
                m_Human.ChangeState(m_Human.IdleState, false);
            }
        }

        public override void Exit(bool isHaunted)
        {
            base.Exit(isHaunted);
            if (!isHaunted)
            {
                m_Human.RefillNeed();
                m_Human.CalculateNextTask(false);
            }
            m_Human.PauseAllNeeds(false);
        }
    }

    public class Haunted : BaseState
    {
        public Haunted(Human human, Transform target) : base(human, StateType.Haunted, target) { }

        public override void Enter()
        {
            base.Enter();
            // TODO: animation
            m_Agent.isStopped = true;
            m_Human.PauseAllNeeds(true);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            // TODO: animation
            // TODO: task!
            m_Human.TestWait(3f);
            // TODO: uhhh go back to normal i guess 
            if (m_Human.Continue)
            {
                m_Human.ChangeState(m_Human.IdleState, false);
            }
        }

        public override void Exit(bool isHaunted)
        {
            base.Exit(isHaunted);
            //m_Human.PauseAllNeeds(false);
            //m_Human.CalculateNextTask(true);
        }
    }

}
