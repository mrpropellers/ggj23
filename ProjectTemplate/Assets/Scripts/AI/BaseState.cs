using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    protected Human Human;
    public string StateName;

    public BaseState(Human human, string name)
    {
        Human = human;
        StateName = name;
    }

    public virtual void Enter()
    {
        //Debug.Log("base idle");
    }
    public virtual void UpdateLogic()
    {
        // TODO: calculate task queue
        //Debug.Log("base update logic");
    }
    public virtual void Exit()
    {
        //Debug.Log("base exit");
    }
}

public class Idle : BaseState
{
    public Idle(Human human) : base(human, "Idle") { }

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
    private Transform m_Target;
    private const float k_DistThreshold = 1.5f;

    public Moving(Human human, Transform targetTx) : base(human, "Moving")
    {
        m_Target = targetTx;
    }

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

public class Task : BaseState
{
    public Task(Human human) : base(human, "Moving") { }

    public override void Enter()
    {
        base.Enter();
        // TODO: animation
        Debug.Log("enteirng TASK");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.Log("update logic TASK");
        // TODO: animation
        // TODO: task!
        Human.ChangeState(Human.IdleState);
    }
}