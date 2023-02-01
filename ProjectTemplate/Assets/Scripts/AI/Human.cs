using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Human : MonoBehaviour
{
    //[SerializeField]
    // TODO: make private
    //private HumanDataScriptableObject HumanData;
    public HumanDataScriptableObject HumanData;

    // TODO: make private
    public BaseState m_CurrentState;

    [SerializeField]
    private Transform m_TargetFollowPoint;

    private NavMeshAgent m_Agent;

    public BaseState IdleState;
    public BaseState MovingState;
    public BaseState TaskState;

    private Transform WaypointsParent;
    bool m_StartedWait = false;
    public bool Continue;
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
        if (!m_StartedWait)
        {
            //m_CurrentState.Wait(true);
            Continue = false;
            m_StartedWait = true;
            Debug.Log("Starting wait");
            StartCoroutine(Wait(time));
        }

    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("finished wait");
        //m_CurrentState.Wait(false);
        Continue = true;
        m_CurrentState.UpdateLogic();
        m_StartedWait = false;
    }

    //public void UpdateNeed(HumanTask)
    //{

    //}
}

public enum HumanTask
{
    Eat, Sleep, Bathroom, None
}
