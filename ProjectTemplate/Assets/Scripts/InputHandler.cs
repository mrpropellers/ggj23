using GGJ23.Audio;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    public Vector2 MovementInput { get; private set; }

    public Window CurrentWindow { get; set; }
    public bool LookingInside { get; private set; }
    public Hauntable CurrentHauntableObject { get; set; }
    public bool FreezeControls { get; set; }
    public bool IsMoving => Vector2.Distance(MovementInput, Vector2.zero) < Mathf.Epsilon;
    float m_BgmAttenuateDirection;

    [SerializeField]
    [Range(0f, 1f)]
    float m_BgmAttenuationWhenPeeking = 0.3f;

    [SerializeField]
    [Range(0f, 1f)]
    float m_BgmAttenuationWhenRooting = 0.5f;

    [SerializeField]
    [Range(0f, 1f)]
    float m_BgmAttenuationWhenKilling = 0.8f;

    [SerializeField]
    [Range(0f, 5f)]
    float m_BgmAttenuationTime = 2f;


    [SerializeField]
    private float m_CameraHorizontalLook = 1f;

    private Vector2 m_MouseInput;
    public Vector2 MouseInputWithSensitivity => m_MouseInput * m_CameraHorizontalLook;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (FreezeControls || MenuHandler.IsGamePaused) return;

        MovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // If near a window...
        if (CurrentWindow != null)
        {
            if (LookingInside)
            {
                // Controls for window mode
                if (CurrentHauntableObject != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (CurrentWindow.Room.HasHauntableHumans)
                        {
                            var bgmAttenuation = CurrentHauntableObject.IsKill
                                ? m_BgmAttenuationWhenKilling
                                : m_BgmAttenuationWhenRooting;
                            StartCoroutine(FmodHelper.AttenuateBgmTo(
                                bgmAttenuation, m_BgmAttenuationTime));

                            CurrentHauntableObject.Haunt();
                        }
                        else
                        {
                            UIManager.Instance.ShowHint("no one is in range to haunt...", 4f);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        CurrentWindow.Room.SelectNextHauntableRight();
                    }
                    else if (Input.GetKeyDown(KeyCode.A))
                    {
                        CurrentWindow.Room.SelectNextHauntableLeft();
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    LookingInside = false;
                    FmodHelper.SetPlayerIsSneaking(true);
                    StartCoroutine(FmodHelper.AttenuateBgmTo(0f, m_BgmAttenuationTime));
                    CurrentWindow.StopLooking();
                    UIManager.Instance.StatsTransitions(true);
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    CurrentWindow.Room.GrowRoots();
                    StartCoroutine(FmodHelper.AttenuateBgmTo(m_BgmAttenuationWhenRooting, m_BgmAttenuationTime));
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    CurrentWindow.Room.StopGrowingRoots();
                    StartCoroutine(FmodHelper.AttenuateBgmTo(m_BgmAttenuationWhenPeeking, m_BgmAttenuationTime));
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    FmodHelper.SetPlayerIsSneaking(false);
                    StartCoroutine(
                        FmodHelper.AttenuateBgmTo(m_BgmAttenuationWhenPeeking, m_BgmAttenuationTime));
                    LookingInside = true;
                    CurrentWindow.LookInside();
                    UIManager.Instance.StatsTransitions(false);
                }
            }
        }
    }
}
