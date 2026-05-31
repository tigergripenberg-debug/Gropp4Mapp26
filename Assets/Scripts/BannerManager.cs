using System.Collections;
using TMPro;
using UnityEngine;

public class BannerManager : MonoBehaviour
{

    [SerializeField] GameObject bonusCanvas;
    [SerializeField] private TMP_Text bonusText;
    private Score score;


    public void ShowBanner(ScoreEventType type, string message)
    {

        switch (type)
        {
            case ScoreEventType.Small:
                ShowBannerText(message);
                break;
            case ScoreEventType.Medium:
                ShowBannerText(message);
                break;
            case ScoreEventType.Big:
                ShowBannerText(message);
                break;
            case ScoreEventType.Jackpot:
                ShowBannerText(message);
                break;
        }
    }

    private void OnEnable()
    {
        Score.OnScoreMessage += ShowBannerText;
    }

    private void OnDisable()
    {
        Score.OnScoreMessage -= ShowBannerText;
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