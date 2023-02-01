using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Human : MonoBehaviour
{
    [SerializeField]
    private HumanDataScriptableObject HumanData;

    [SerializeField]
    // TODO: remove for debug
    private BaseState m_CurrentState;

    [SerializeField]
    private Transform m_TargetFollowPoint;

    private NavMeshAgent m_Agent;

    public BaseState IdleState;
    public BaseState MovingState;
    public BaseState TaskState;

    private Transform WaypointsParent;
    public bool Continue = false;
    public HumanTask CurrentTask = HumanTask.None;

    private void Start()
    {
        // TODO: clean this up
        WaypointsParent = GameObject.Find("House/Waypoints").GetComponent<Transform>();

        m_Agent = GetComponent<NavMeshAgent>();

        IdleState = new Idle(this, m_TargetFollowPoint, WaypointsParent);
        MovingState = new Moving(this, m_TargetFollowPoint, WaypointsParent);
        TaskState = new EatTask(this, m_TargetFollowPoint, WaypointsParent);

        m_CurrentState = GetInitialState();
        if (m_CurrentState != null)
        {
            m_CurrentState.Enter();
        }
    }

    private void Update()
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.UpdateLogic();
        }

        if (m_TargetFollowPoint != null)
        {
            m_Agent.destination = m_TargetFollowPoint.position;
        }
    }

    public void ChangeState(BaseState newState)
    {
        m_CurrentState.Exit();

        m_CurrentState = newState;
        m_CurrentState.Enter();
    }

    private BaseState GetInitialState()
    {
        Debug.Log("get initial state idle");
        return IdleState;
    }

    public void TestWait(float time)
    {
        Debug.Log("Starting wait");
        StartCoroutine(Wait(time));
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("finished wait");
        Continue = true;
    }

    //public void UpdateNeed(HumanTask)
    //{

    //}
}

public enum HumanTask
{
    Eat, Sleep, Bathroom, None
}
