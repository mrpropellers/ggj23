using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuHandler : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public GameObject MainMenuUi;

    public TextMeshProUGUI GameStateText;

    private static bool IsDitheringEnabled = true;

    [SerializeField]
    private EventSystem m_EventSystem;
    private GameObject m_LastSelectedObject;

    [SerializeField]
    private GameObject m_DitheringShader;

    [SerializeField]
    private GameObject m_StartCamera;

    private void Awake()
    {
        IsGamePaused = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (IsGamePaused){
                Resume();
            }
            else {
                Pause();
            }
        }



        if (IsGamePaused)
        {
            Debug.Log(m_EventSystem.currentSelectedGameObject);
            if (m_EventSystem.currentSelectedGameObject == null)
            {
                m_EventSystem.SetSelectedGameObject(m_LastSelectedObject);
            }
            else
            {
                m_LastSelectedObject = m_EventSystem.currentSelectedGameObject;
            }
        }
    }

    public void UpdateSelectedButton(GameObject newButton)
    {
        m_EventSystem.SetSelectedGameObject(newButton);
        m_LastSelectedObject = newButton;
    }

    public void StartGame ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        IsGamePaused = false;
        m_StartCamera.SetActive(false);
        // MainMenuUi.SetActive(false);
        UIManager.Instance.MenuTransitions(false);
        UIManager.Instance.GameTimeStopwatch.Begin();
        StartCoroutine(WaitForInitialPrompt());
    }

    private IEnumerator WaitForInitialPrompt()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.ShowHint("kill them all...", 3);
    }

    void Resume ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // MainMenuUi.SetActive(false);
        UIManager.Instance.MenuTransitions(false);
        IsGamePaused = false;
        UIManager.Instance.GameTimeStopwatch.Unpause();
    }

    void Pause ()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        if (!MainMenuUi.activeSelf){
            GameStateText.text = "resume";
            MainMenuUi.SetActive(true);
            UIManager.Instance.MenuTransitions(true);
            IsGamePaused = true;
            UIManager.Instance.GameTimeStopwatch.Pause();
        }
    }

    public void ExitGame ()
    {
        Debug.Log("Exiting Game!");
        Application.Quit();
    }

    #region OptionMenuRegion

    public void SetVolume (float volume)
    {
        Debug.Log(volume);
    }

    public void ToggleDithering ()
    {
        IsDitheringEnabled = !IsDitheringEnabled;
        m_DitheringShader.SetActive(IsDitheringEnabled);
    }

    #endregion
}
