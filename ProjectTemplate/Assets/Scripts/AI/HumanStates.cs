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
        public static Dictionary<HumanNeed, string> NeedToAnimName = new()
        {
            { HumanNeed.Bathroom, "toilet" }, { HumanNeed.Curious, "read" },
            { HumanNeed.Hunger, "eat" }, { HumanNeed.Sleep, "sleep" },
            { HumanNeed.Haunted, "jump" }, { HumanNeed.Error, string.Empty }
        };

        public static Dictionary<HumanNeed, Vector3> NeedToHardcodeRotation = new()
        {
            { HumanNeed.Bathroom, new Vector3(0, 270, 0) }, { HumanNeed.Sleep, new Vector3(0, 180, 0) },
            { HumanNeed.Curious, new Vector3(0, 180, 0) }, { HumanNeed.Hunger, new Vector3(0, 270, 0) },
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

        public virtual void Enter(bool isEscaping) { }
        public virtual void UpdateLogic() { }
        public virtual void Exit(bool isHaunted) { }
    }

    public class Idle : BaseState
    {
        public Idle(Human human, Transform target) : base(human, StateType.Idle, target) { }

        public override void Enter(bool isEscaping)
        {
            base.Enter(isEscaping);
            m_Agent.isStopped = true;
            m_Human.Animator.SetBool("walking", false);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            m_Human.TestWait(3f);
            if (m_Human.Continue)
            {
                m_Human.ChangeState(m_Human.MovingState, false);
            }
        }
    }

    public class Moving : BaseState
    {
        public Moving(Human human, Transform target) : base(human, StateType.Moving, target) { }

        public override void Enter(bool isEscaping)
        {
            base.Enter(isEscaping);
            if (m_Target != null)
            {
                m_Agent.destination = m_Target.position;
            }

            m_Agent.speed = m_Human.Animator.GetBool(NeedToAnimName[HumanNeed.Haunted]) ? m_Agent.speed = Human.RunSpeed : m_Agent.speed = Human.WalkSpeed;

            m_Agent.isStopped = false;
            m_Human.Animator.SetBool("walking", true);
            if (isEscaping) // Set to "scared" anim
            {
                m_Human.Animator.SetBool(NeedToAnimName[HumanNeed.Haunted], true);
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (Vector3.Distance(m_Target.position, m_Human.transform.position) < Human.k_DistThreshold)
            {
                m_Human.transform.position = m_Target.position;
                m_Human.ChangeState(m_Human.TaskState, false);
            }
        }

        public override void Exit(bool isHaunted)
        {
            base.Exit(isHaunted);
            if (!isHaunted)
            {
                m_Human.Hauntable = true;
                Debug.Log("hauntable true");
            }
            m_Human.Animator.SetBool("walking", false);
        }
    }

    public class Task : BaseState
    {
        public Task(Human human, Transform target) : base(human, StateType.Task, target) { }

        public override void Enter(bool isEscaping)
        {
            base.Enter(isEscaping);
            m_Agent.isStopped = true;
            m_Human.PauseNeed(true);

            // Rotate towards task
            if (NeedToHardcodeRotation.TryGetValue(m_Human.CurrentTask, out var rot))
            {
                m_Human.transform.rotation = Quaternion.Euler(rot);
            }

            m_Human.Animator.SetBool("walking", false);
            m_Human.Animator.SetBool(NeedToAnimName[m_Human.CurrentTask], true);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            m_Human.TestWait(Random.Range(4f, 10f));
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
                m_Human.Animator.SetBool(NeedToAnimName[m_Human.CurrentTask], false);
                m_Human.Animator.SetBool(NeedToAnimName[HumanNeed.Haunted], false);
                m_Human.RefillNeed();
                m_Human.CalculateNextTask(false);
            }
            else
            {
                m_Human.Animator.SetBool(NeedToAnimName[m_Human.TaskBeforeHaunt], false);
            }
            m_Human.PauseAllNeeds(false);
        }
    }

    public class Haunted : BaseState
    {
        public Haunted(Human human, Transform target) : base(human, StateType.Haunted, target) { }

        public override void Enter(bool isEscaping)
        {
            base.Enter(isEscaping);
            m_Human.Animator.SetBool(NeedToAnimName[m_Human.CurrentTask], true);
            m_Agent.isStopped = true;
            m_Human.PauseAllNeeds(true);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
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
