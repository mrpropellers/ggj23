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

    [SerializeField]
    private float m_CameraHorizontalLook = 1f;

    private CinemachineTrackedDolly m_DollyCam;
    private GameObject m_CurrentWindow;
    private CinemachineVirtualCamera m_LookCam;

    private Vector2 m_MovementInput;
    private Vector2 m_MouseInput;
    private bool m_LookingInside;

    private void Start()
    {
        m_DollyCam = m_VirtualDollyCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
        m_MovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (m_CurrentWindow != null && Input.GetMouseButtonDown(0) && !m_LookingInside)
        {
            m_LookingInside = true;
            m_LookCam = m_CurrentWindow.GetComponentInChildren<CinemachineVirtualCamera>(true);
            m_LookCam.gameObject.SetActive(true);
            m_DollyCart.m_Speed = 0f;
            UIManager.Instance.SetVignetteIntensity(0.52f, 2f);
        }

        if (m_CurrentWindow != null && Input.GetMouseButtonDown(1) && m_LookingInside)
        {
            m_LookingInside = false;
            m_LookCam.gameObject.SetActive(false);
            m_LookCam = null;
            UIManager.Instance.SetVignetteIntensity(0f, 2f, true);
        }

        if (m_LookingInside)
        {
            Quaternion newRotation = m_LookCam.transform.localRotation *
                                     Quaternion.AngleAxis(m_MouseInput.x * m_CameraHorizontalLook * Time.deltaTime,
                                         Vector3.up);
            float currDeg = newRotation.eulerAngles.y;

            if (currDeg < 50f || currDeg > 310f)
            {
                m_LookCam.transform.localRotation = newRotation;
            }

            m_LookCam.m_Lens.Dutch = Mathf.Lerp(-9f, 9f,
                Quaternion.Angle(Quaternion.Euler(0f, -50f, 0f), Quaternion.Euler(0f, currDeg, 0f)) / 100f);
        }
    }

    private void FixedUpdate()
    {
        if (!m_LookingInside)
        {
            m_DollyCart.m_Speed = Mathf.Lerp(m_DollyCart.m_Speed, m_MoveSpeedMax * m_MovementInput.x,
                Time.deltaTime * m_AccelerationRate);
            m_DollyCam.m_PathPosition = m_DollyCart.m_Position;

            transform.position = m_DollyCart.transform.position + Vector3.up;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Window"))
        {
            m_CurrentWindow = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Window"))
        {
            m_CurrentWindow = null;
        }
    }
}
