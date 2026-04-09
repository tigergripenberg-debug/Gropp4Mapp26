using System.Collections;
using TMPro;
using UnityEngine;

public class BannerManager : MonoBehaviour
{
    
    [SerializeField] GameObject bonusCanvas;
    [SerializeField] private TMP_Text bonusText;

    public void ShowBanner(ScoreEventType type)
    {
        switch (type)
        {
            case ScoreEventType.Small:
                ShowBannerText("Nice!");
                break;
            case ScoreEventType.Medium:
                ShowBannerText("Medium!");
                break;
            case ScoreEventType.Big:
                ShowBannerText("Big!");
                break;
            case ScoreEventType.Jackpot:
                ShowBannerText("Jackpot!");
                break;
        }
    }
    
    private void OnEnable()
    {
        Score.OnScoreChange += ShowBanner;
    }

    private void OnDisable()
    {
        Score.OnScoreChange -= ShowBanner;
    }

    void ShowBannerText(string text)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(text));
    }

    IEnumerator ShowRoutine(string text)
    {
        bonusText.text = text;
        bonusCanvas.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        bonusCanvas.SetActive(false);
    }
    
}
