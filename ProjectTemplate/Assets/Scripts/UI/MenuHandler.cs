using System;
using GGJ23.Audio;
using System.Collections;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField]
    StudioEventEmitter m_TitleMusic;

    private bool m_IsGameStarted;

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
            //Debug.Log(m_EventSystem.currentSelectedGameObject);
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
        m_TitleMusic.Stop();
        FmodHelper.TurnOnInGameMusic();
        StartCoroutine(FmodHelper.AttenuateBgmTo(0f, 4f));

        if (!m_IsGameStarted)
        {
            m_IsGameStarted = true;
            StartCoroutine(WaitForInitialPrompt());
        }

        IsGamePaused = false;
        m_StartCamera.SetActive(false);
        // MainMenuUi.SetActive(false);
        UIManager.Instance.MenuTransitions(false);
        UIManager.Instance.GameTimeStopwatch.Begin();
    }

    private IEnumerator WaitForInitialPrompt()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.ShowHint("kill them all...", 3);
    }

    void Resume ()
    {
        // Putting stopwatch first because menu animations seem prone to throwing exceptions before getting to the end
        // of the resume function
        UIManager.Instance.GameTimeStopwatch.Unpause();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // MainMenuUi.SetActive(false);
        UIManager.Instance.MenuTransitions(false);
        IsGamePaused = false;
    }

    void Pause ()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        if (!MainMenuUi.activeSelf){
            UIManager.Instance.GameTimeStopwatch.Pause();
            GameStateText.text = "resume";
            MainMenuUi.SetActive(true);
            UIManager.Instance.MenuTransitions(true);
            IsGamePaused = true;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame ()
    {
        Debug.Log("Exiting Game!");
        Application.Quit();
    }

    #region OptionMenuRegion

    public void SetBgmVolume (float volume)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(FmodHelper.PARAM_BGM_VOLUME, volume);
    }

    public void SetSfxVolume(Slider volumeSlider)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(FmodHelper.PARAM_SFX_VOLUME, volumeSlider.normalizedValue);
    }

    public void ToggleDithering ()
    {
        IsDitheringEnabled = !IsDitheringEnabled;
        m_DitheringShader.SetActive(IsDitheringEnabled);
    }

    public void ToggleFullscreen(Toggle checkbox)
    {
        Screen.fullScreen = checkbox.isOn;
    }

    #endregion
}
