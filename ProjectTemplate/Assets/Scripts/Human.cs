using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Human : MonoBehaviour
{
    [SerializeField]
    private Transform m_TargetFollowPoint;

    private NavMeshAgent m_Agent;

    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (m_TargetFollowPoint != null)
        {
            m_Agent.destination = m_TargetFollowPoint.position;
        }
    }
}
