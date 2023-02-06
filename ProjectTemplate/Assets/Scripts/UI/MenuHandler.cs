using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public GameObject MainMenuUi;

    public TextMeshProUGUI GameStateText;

    private static bool IsDitheringEnabled = true;

    private void Awake()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused){
                Resume();
            }
            else {
                Pause();
            }
        }
    }

    public void StartGame ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        IsGamePaused = false;
        MainMenuUi.SetActive(false);
        GameStateText.text = "Resume";
    }

    void Resume ()
    {
        MainMenuUi.SetActive(false);
        IsGamePaused = false;
    }

    void Pause ()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        if (!MainMenuUi.activeSelf){
            MainMenuUi.SetActive(true);

            IsGamePaused = true;
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
        Debug.Log($"Dithering: {IsDitheringEnabled}");
    }

    #endregion
}
