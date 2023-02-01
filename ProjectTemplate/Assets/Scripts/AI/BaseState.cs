using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    private Transform WaypointsParent;
    protected Human Human;
    protected Transform m_Target;
    public string StateName;

    public BaseState(Human human, string name, Transform target, Transform waypoints)
    {
        Human = human;
        StateName = name;
        m_Target = target;
        WaypointsParent = waypoints;
    }

    public virtual void Enter()
    {
        //Debug.Log("base idle");
    }
    public virtual void UpdateLogic()
    {
        // TODO: calculate task queue
        //Debug.Log("base update logic");
        // TODO: move target!!
        m_Target.position = WaypointsParent.Find("Kitchen").position;

    }
    public virtual void Exit()
    {
        //Debug.Log("base exit");
    }
}

public class Idle : BaseState
{
    public Idle(Human human, Transform target, Transform waypoints) : base(human, "Idle", target, waypoints) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("enteirng IDLE");
        // TODO: animation
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.Log("update logic IDLE");
        // TODO: animation
        // TODO: stop moving
        // TODO: check if should switch??
        Human.ChangeState(Human.MovingState);
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
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.Log("update logic MOVING");
        // TODO: animation
        if (Vector3.Distance(m_Target.position, Human.transform.position) < k_DistThreshold)
        {
            Human.ChangeState(Human.TaskState);
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
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.Log("update logic EAT");
        // TODO: animation
        // TODO: task!
        // if animation done?
        Human.Continue = false;
        Human.TestWait(5f);
        if (Human.Continue)
        {
            Human.ChangeState(Human.IdleState);
        }
    }
}