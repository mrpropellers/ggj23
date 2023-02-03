using Cinemachine;
using UnityEngine;

public class Window : MonoBehaviour
{
    [field: SerializeField]
    public Room Room { get; private set; }

    [SerializeField]
    private LayerMask m_HauntableLayerMask;

    private CinemachineVirtualCamera m_LookCam;
    private bool m_Viewing;

    private void Start()
    {
        m_LookCam = GetComponentInChildren<CinemachineVirtualCamera>(true);
    }

    private void Update()
    {
        if (m_Viewing)
        {
            // Horizontal look
            Quaternion newHorRotation = Quaternion.AngleAxis(InputHandler.Instance.MouseInputWithSensitivity.x * Time.deltaTime,
                                         Vector3.up);
            float currHorDeg = newHorRotation.eulerAngles.y;

            if (currHorDeg < 50f || currHorDeg > 310f)
            {
                m_LookCam.transform.localRotation *= newHorRotation;
            }

            Quaternion newVerRotation = Quaternion.AngleAxis(-InputHandler.Instance.MouseInputWithSensitivity.y * Time.deltaTime,
                                            Vector3.right);
            float currVerDeg = newVerRotation.eulerAngles.y;

            if (currVerDeg < 50f || currVerDeg > 310f)
            {
                m_LookCam.transform.localRotation *= newVerRotation;
            }

            m_LookCam.m_Lens.Dutch = Mathf.Lerp(-9f, 9f,
                Quaternion.Angle(Quaternion.Euler(0f, -50f, 0f), Quaternion.Euler(0f, currHorDeg, 0f)) / 100f);

            if (Physics.Raycast(m_LookCam.transform.position, m_LookCam.transform.forward, out RaycastHit hit, 100f,
                m_HauntableLayerMask))
            {
                InputHandler.Instance.CurrentHauntableObject = hit.collider.GetComponent<Hauntable>();
                InputHandler.Instance.CurrentHauntableObject.Hover();
            }
            else
            {
                InputHandler.Instance.CurrentHauntableObject = null;
            }
        }
    }

    public void LookInside()
    {
        m_Viewing = true;
        m_LookCam.gameObject.SetActive(true);
        UIManager.Instance.SetVignetteIntensity(0.52f, 2f);
    }

    public void StopLooking()
    {
        m_Viewing = false;
        m_LookCam.gameObject.SetActive(false);
        UIManager.Instance.SetVignetteIntensity(0f, 2f, true);
    }
}
