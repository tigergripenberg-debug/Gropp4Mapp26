using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static bool gameIsPaused = false;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderBoardPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        gameIsPaused = false;
    }
    public void TimeGame()
    {
        SceneManager.LoadScene(2);
        gameIsPaused = false;
    }

    public void StartTutorial()
    {
        // TODO OPEN TUTORIAL SCENE
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
    public void OpenLeaderBoardPanel()
    {
        if (leaderBoardPanel.activeInHierarchy)
        {
            CloseLeaderBoardPanel();
            return;
        }
        leaderBoardPanel.SetActive(true);
        leaderBoardPanel.GetComponent<leaderboardpanel>().Refresh();
    }

    public void CloseLeaderBoardPanel()
    {
        leaderBoardPanel.SetActive(false);
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
