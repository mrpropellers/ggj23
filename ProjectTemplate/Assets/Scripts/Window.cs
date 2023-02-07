using Cinemachine;
using UnityEngine;

public class Window : MonoBehaviour
{
    [field: SerializeField]
    public Room Room { get; internal set; }

    [SerializeField]
    private LayerMask m_HauntableLayerMask;

    private CinemachineVirtualCamera m_LookCam;
    private bool m_Viewing;
    private bool m_FirstTimeViewing = true;
    private float m_VerticalRotation;

    private void Start()
    {
        m_LookCam = GetComponentInChildren<CinemachineVirtualCamera>(true);
    }

    private void Update()
    {
        if (m_Viewing)
        {
            if (m_FirstTimeViewing)
            {
                m_FirstTimeViewing = false;
                UIManager.Instance.ShowHint("hold [space] to spread your roots...", 4);
            }

            Quaternion currentRot = m_LookCam.transform.localRotation;

            // Horizontal look
            float horizontalRotation = InputHandler.Instance.MouseInputWithSensitivity.x * Time.deltaTime;
            float newHorRot = horizontalRotation + currentRot.eulerAngles.y;

            // Clamp horizontal looking
            if (newHorRot > 50f && newHorRot < 310f)
            {
                float snapToRight = Mathf.Abs(newHorRot - 50f);
                float snapToLeft = Mathf.Abs(newHorRot - 310);
                bool right = snapToRight < snapToLeft;

                newHorRot = right ? 50f : 310f;
            }

            m_VerticalRotation -= InputHandler.Instance.MouseInputWithSensitivity.y * Time.deltaTime;
            m_VerticalRotation = Mathf.Clamp(m_VerticalRotation, -30f, 30f);

            m_LookCam.transform.localEulerAngles =
                new Vector3(m_VerticalRotation, newHorRot, 0f);

            m_LookCam.m_Lens.Dutch = Mathf.Lerp(-9f, 9f,
                Quaternion.Angle(Quaternion.Euler(0f, -50f, 0f), Quaternion.Euler(0f, newHorRot, 0f)) / 100f);

            // Old mouse controls. Re-enable later if wanted? Seems like it would be annoying to integrate with the keyboard controls
            // if (Physics.Raycast(m_LookCam.transform.position, m_LookCam.transform.forward, out RaycastHit hit, 100f,
            //     m_HauntableLayerMask))
            // {
            //     InputHandler.Instance.CurrentHauntableObject = hit.collider.GetComponent<Hauntable>();
            //     InputHandler.Instance.CurrentHauntableObject.Hover();
            // }
            // else
            // {
            //     InputHandler.Instance.CurrentHauntableObject = null;
            // }
        }
    }

    public void LookInside()
    {
        m_Viewing = true;
        Room.LookingInRoom = true;
        m_LookCam.gameObject.SetActive(true);
        UIManager.Instance.SetVignetteIntensity(0.52f, 2f);
    }

    public void StopLooking()
    {
        m_Viewing = false;
        Room.LookingInRoom = false;
        m_LookCam.gameObject.SetActive(false);
        UIManager.Instance.SetVignetteIntensity(0f, 2f, true);
    }
}
