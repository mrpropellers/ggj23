using Cinemachine;
using UnityEngine;

public class TreeGoblinController : MonoBehaviour
{
    [SerializeField]
    private CinemachineDollyCart m_DollyCart;

    [SerializeField]
    private CinemachineVirtualCamera m_VirtualDollyCam;

    [SerializeField]
    private float m_MoveSpeedMax = 3f;

    [SerializeField]
    private float m_AccelerationRate = 10f;

    private CinemachineTrackedDolly m_DollyCam;

    private void Start()
    {
        m_DollyCam = m_VirtualDollyCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
        if (InputHandler.Instance.LookingInside)
        {
            m_DollyCart.m_Speed = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!InputHandler.Instance.LookingInside)
        {
            m_DollyCart.m_Speed = Mathf.Lerp(m_DollyCart.m_Speed, m_MoveSpeedMax * InputHandler.Instance.MovementInput.x,m_AccelerationRate);
            m_DollyCam.m_PathPosition = m_DollyCart.m_Position;

            transform.position = m_DollyCart.transform.position + Vector3.up;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Window"))
        {
            InputHandler.Instance.CurrentWindow = other.GetComponent<Window>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Window"))
        {
            InputHandler.Instance.CurrentWindow = null;
        }
    }
}
