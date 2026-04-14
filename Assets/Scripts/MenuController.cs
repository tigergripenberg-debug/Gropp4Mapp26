using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        
    }

    public void OpenPausePanel()
    {
        OpenSettingsPanel();
        Block.canMoveBlocks = false;
    }

    public void ClosePausePanel()
    {
        settingsPanel.SetActive(false);
        Block.canMoveBlocks = true;
    }

    public void GoToStartMenu()
    {
        SceneManager.LoadScene(0);
    }

}
