using System;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    
    [SerializeField] private GameObject tutorialTextPanel;
    [SerializeField] private TMP_Text tutorialText;
    [TextArea]
    [SerializeField] private String[] tutorialTexts;
    private int index = 0;
    //[SerializeField] private GameObject tutorialPanel;
    private void Start()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)){ CloseOverlay(); }
    }
    public void ShowTutorialText()
    {
        if (index >= tutorialTexts.Length)
        {
            index = tutorialTexts.Length-1;
        }
        tutorialText.SetText(tutorialTexts[index]);
        tutorialTextPanel.SetActive(true);
    }
    private void CloseOverlay()
    {
        tutorialTextPanel.SetActive(false);
        index++;
    }
    //     int playedTutorial = PlayerPrefs.GetInt("playedTutorial", 0);
    //     if (playedTutorial == 0 && SceneManager.GetActiveScene().buildIndex != 0)
    //     {
    //         ShowTutorial();
    //         PlayerPrefs.SetInt("playedTutorial", 1);
    //         PlayerPrefs.Save();
    //     }
    // }
    // public void ShowTutorial()
    // {
    //     if (tutorialPanel.activeInHierarchy)
    //     {
    //         tutorialPanel.SetActive(false);
    //     }
    //     else
    //         tutorialPanel.SetActive(true);
    // }
}