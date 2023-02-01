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

    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();

        IdleState = new Idle(this);
        MovingState = new Moving(this, m_TargetFollowPoint);
        TaskState = new Task(this);

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
}

public class HumanTask
{
    
}
