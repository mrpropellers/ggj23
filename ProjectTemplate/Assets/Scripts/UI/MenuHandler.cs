using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuHandler : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public GameObject MainMenuUi;

    public TextMeshProUGUI GameStateText;

    private static bool IsDitheringEnabled = true;

    private void Awake()
    {
        Time.timeScale = 0f;
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
        IsGamePaused = false;
        MainMenuUi.SetActive(false);
        GameStateText.text = "Resume";
        Time.timeScale = 1f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Resume ()
    {
        MainMenuUi.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    void Pause ()
    {
        if (!MainMenuUi.activeSelf){
            MainMenuUi.SetActive(true);
            
            Time.timeScale = 0f;
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
