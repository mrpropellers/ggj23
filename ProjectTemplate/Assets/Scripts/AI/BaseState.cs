using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseState
{
    private Transform m_Waypoints;
    protected Human m_Human;
    protected NavMeshAgent m_Agent;
    protected Transform m_Target;
    public string StateName;
    //protected bool m_IsWaiting = false;

    public BaseState(Human human, string name, Transform target, Transform waypoints)
    {
        m_Human = human;
        m_Agent = m_Human.GetComponent<NavMeshAgent>(); // TODO: clean this up
        StateName = name;
        m_Target = target;
        m_Waypoints = waypoints;
    }

    public virtual void Enter()
    {
        //Debug.Log("base idle");
    }
    public virtual void UpdateLogic()
    {
        // Debug.Log(m_IsWaiting);
        // TODO: calculate task queue
        //Debug.Log("base update logic");
        // TODO: move target!!
        m_Target.position = m_Waypoints.Find("Kitchen").position;

    }
    public virtual void Exit()
    {
        //Debug.Log("base exit");
    }

    //public virtual void Wait(bool val)
    //{
    //    m_IsWaiting = val;
    //}
}

public class Idle : BaseState
{
    public Idle(Human human, Transform target, Transform waypoints) : base(human, "Idle", target, waypoints) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("enteirng IDLE");
         m_Agent.isStopped = true;
        // TODO: animation
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.Log("update logic IDLE");
        // TODO: animation
        // TODO: stop moving
        //m_Human.Continue = false;
        m_Human.TestWait(2f);
        //if (!m_IsWaiting)
        if (m_Human.Continue)
        {
            Debug.Log("moving to moving state...");
            m_Human.ChangeState(m_Human.MovingState);
        }
        // TODO: check if should switch??
    }
}

public class Moving : BaseState
{
    private const float k_DistThreshold = 1.5f;

    public Moving(Human human, Transform target, Transform waypoints) : base(human, "Moving", target, waypoints) {}

    public override void Enter()
    {
        base.Enter();
        // TODO: animation
        Debug.Log("enteirng MOVING");
         m_Agent.isStopped = false;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.Log("update logic MOVING");
        // TODO: animation
        if (Vector3.Distance(m_Target.position, m_Human.transform.position) < k_DistThreshold)
        {
            m_Human.ChangeState(m_Human.TaskState);
        }
    }
}

public class EatTask : BaseState
{
    public EatTask(Human human, Transform target, Transform waypoints) : base(human, "Eat", target, waypoints) { }

    public override void Enter()
    {
        base.Enter();
        // TODO: animation
        Debug.Log("enteirng EAT");
        m_Agent.isStopped = true;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.Log("update logic EAT");
        // TODO: animation
        // TODO: task!
        // if animation done?
        //m_Human.Continue = false;
        m_Human.TestWait(5f);
        //if (!m_IsWaiting)
        if (m_Human.Continue)
        {
            m_Human.ChangeState(m_Human.IdleState);
        }
    }
}
