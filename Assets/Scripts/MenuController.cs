using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static bool gameIsPaused = false;

    [SerializeField] private GameObject settingsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        gameIsPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel.activeInHierarchy)
        {
            CloseSettingsPanel();
            return;
        }
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void OpenPausePanel()
    {
        OpenSettingsPanel();
        gameIsPaused = true;
    }

    public void ClosePausePanel()
    {
        settingsPanel.SetActive(false);
        gameIsPaused = false;
    }

    public void GoToStartMenu()
    {
        SceneManager.LoadScene(0);
    }

}
