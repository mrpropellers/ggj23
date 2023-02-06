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
        if (FreezeControls) return;

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
                        CurrentHauntableObject.Haunt();
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
                    CurrentWindow.StopLooking();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    CurrentWindow.Room.GrowRoots();
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    CurrentWindow.Room.StopGrowingRoots();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LookingInside = true;
                    CurrentWindow.LookInside();
                }
            }
        }
    }
}
