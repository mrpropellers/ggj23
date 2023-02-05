using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public GameObject MainMenuUi;
    //public GameObject PauseMenuUi;

    public TextMeshProUGUI GameStateText;

    private void Awake()
    {
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed");
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
        Debug.Log("Pausing");
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
}
