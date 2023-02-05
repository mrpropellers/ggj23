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
        // TODO: haunted
        public static Dictionary<HumanNeed, string> NeedToAnimName = new()
        {
            { HumanNeed.Bathroom, "toilet" }, { HumanNeed.Curious, "read" },
            { HumanNeed.Hunger, "eat" }, { HumanNeed.Sleep, "sleep" },
            { HumanNeed.Haunted, "jump" }, { HumanNeed.Error, string.Empty }
        };
        protected Human m_Human;
        protected NavMeshAgent m_Agent;
        protected Transform m_Target;
        public StateType StateType;

        public BaseState(Human human, StateType name, Transform target)
        {
            m_Human = human;
            m_Agent = m_Human.GetComponent<NavMeshAgent>();
            StateType = name;
            m_Target = target;
        }

        public virtual void Enter() { }
        public virtual void UpdateLogic() { }
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
            m_Human.Animator.SetBool("walking", false);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            // TODO: animation
            m_Human.TestWait(3f);
            if (m_Human.Continue)
            {
                m_Human.ChangeState(m_Human.MovingState, false);
            }
        }
    }

    public class Moving : BaseState
    {
        // TODO: tune this
        private const float k_DistThreshold = 1f;

        public Moving(Human human, Transform target) : base(human, StateType.Moving, target) { }

        public override void Enter()
        {
            base.Enter();
            if (m_Target != null)
            {
                m_Agent.destination = m_Target.position;
            }
            m_Agent.isStopped = false;
            m_Human.Animator.SetBool("walking", true);
            // TODO: set speed to higher if scared
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

        public override void Exit(bool isHaunted)
        {
            base.Exit(isHaunted);
            m_Human.Animator.SetBool("walking", false);
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
            m_Human.PauseNeed(true);
            m_Human.Animator.SetBool("walking", false);
            // TODO: get task name => animation bool
            m_Human.Animator.SetBool(NeedToAnimName[m_Human.CurrentTask], true);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            // TODO: animation
            // TODO: task!
            m_Human.TestWait(5f);
            if (m_Human.Continue)
            {
                m_Human.ChangeState(m_Human.IdleState, false);
            }
        }

        public override void Exit(bool isHaunted)
        {
            base.Exit(isHaunted);
            m_Human.Animator.SetBool(NeedToAnimName[HumanNeed.Haunted], false);
            m_Human.Animator.SetBool(NeedToAnimName[m_Human.CurrentTask], false);
            if (!isHaunted)
            {
                m_Human.RefillNeed();
                m_Human.CalculateNextTask(false);
            }
            m_Human.PauseAllNeeds(false);
            // TODO: get task name => animation bool
        }
    }

    public class Haunted : BaseState
    {
        public Haunted(Human human, Transform target) : base(human, StateType.Haunted, target) { }

        public override void Enter()
        {
            base.Enter();
            m_Human.Animator.SetBool(NeedToAnimName[m_Human.CurrentTask], true);
            m_Agent.isStopped = true;
            m_Human.PauseAllNeeds(true);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            // TODO: animation
            // TODO: task!
            m_Human.TestWait(3f);
            if (m_Human.Continue)
            {
                m_Human.ChangeState(m_Human.IdleState, false);
            }
        }

        public override void Exit(bool isHaunted)
        {
            base.Exit(isHaunted);
            // Don't set "jump" to false until after moving!
            //m_Human.Animator.SetBool(NeedToAnimName[m_Human.CurrentTask], false);
        }
    }

}
