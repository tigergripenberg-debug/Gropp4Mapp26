using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    private void Start()
    {
        int playedTutorial = PlayerPrefs.GetInt("playedTutorial", 0);
        if (playedTutorial == 0)
        {
            ShowTutorial();
            PlayerPrefs.SetInt("playedTutorial", 1);
            PlayerPrefs.Save();
        }
    }
    public void ShowTutorial()
    {
        if (tutorialPanel.activeInHierarchy)
        {
            tutorialPanel.SetActive(false);
        }
        else
            tutorialPanel.SetActive(true);
    }
}