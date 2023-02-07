using Cinemachine;
using Humans;
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

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Transform m_HouseCenter;

    [SerializeField]
    private GameObject m_RootieRig;

    private CinemachineTrackedDolly m_DollyCam;
    private readonly string k_MoveAnim = "moving";
    private bool m_WasMoving;
    private bool m_WasRight;

    private void Start()
    {
        m_DollyCam = m_VirtualDollyCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
        if (InputHandler.Instance.LookingInside || MenuHandler.IsGamePaused)
        {
            m_DollyCart.m_Speed = 0f;
        }
    }

    private void FixedUpdate()
    {
        var isMoving = !Mathf.Approximately(m_DollyCart.m_Speed, 0);
        if (!InputHandler.Instance.LookingInside)
        {
            m_DollyCart.m_Speed = Mathf.Lerp(m_DollyCart.m_Speed, m_MoveSpeedMax * InputHandler.Instance.MovementInput.x,m_AccelerationRate);
            m_DollyCam.m_PathPosition = m_DollyCart.m_Position;

            transform.position = m_DollyCart.transform.position + Vector3.up;
            transform.LookAt(m_HouseCenter.position, Vector3.up);
        }

        // sorry i panicked and forgot how to rotate towards direction of movement
        if (!m_WasMoving && isMoving)
        {
            if (m_DollyCart.m_Speed < Mathf.Epsilon)
            {
                m_RootieRig.transform.rotation *= Quaternion.Euler(0, -90, 0);
                m_WasRight = false;
            }
            else if (m_DollyCart.m_Speed > Mathf.Epsilon)
            {
                m_RootieRig.transform.rotation *= Quaternion.Euler(0, 90, 0);
                m_WasRight = true;
            }
        }
        else if (m_WasMoving && !isMoving)
        {
            if (m_WasRight)
            {
                m_RootieRig.transform.rotation *= Quaternion.Euler(0, -90, 0);
            }
            else
            {
                m_RootieRig.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }

        }
        m_Animator.SetBool(k_MoveAnim, isMoving);
        m_WasMoving = isMoving;
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
