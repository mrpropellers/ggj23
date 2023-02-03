using Cinemachine;
using UnityEngine;

public class Window : MonoBehaviour
{
    [field: SerializeField] public Room Room { get; private set; }

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
            Quaternion newRotation = m_LookCam.transform.localRotation *
                                     Quaternion.AngleAxis(InputHandler.Instance.MouseInputWithSensitivity.x * Time.deltaTime,
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
