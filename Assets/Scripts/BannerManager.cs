using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

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

    // IEnumerator ShowRoutine(string text)
    // {
    //     bonusText.text = text;
    //     bonusCanvas.SetActive(true);
    //     yield return new WaitForSeconds(1.5f);
    //     bonusCanvas.SetActive(false);
    // }
    IEnumerator ShowRoutine(string text)
    {
        bonusCanvas.SetActive(true);
        bonusText.text = text;
        RectTransform rect =
            bonusText.GetComponent<RectTransform>();
        CanvasGroup canvasGroup =
            bonusText.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup =
                bonusText.gameObject.AddComponent<CanvasGroup>();
        }

        // Reset state
        canvasGroup.alpha = 0f;

        Vector2 startPos = Vector2.zero;
        rect.anchoredPosition = startPos;

        Sequence seq = DOTween.Sequence();

        // Fade in
        seq.Append(
            canvasGroup.DOFade(1f, 0.2f)
        );

        // Move upward while visible
        seq.Join(
            rect.DOAnchorPosY(100f, 1f)
        );

        // Fade out
        seq.Append(
            canvasGroup.DOFade(0f, 0.4f)
        );

        yield return seq.WaitForCompletion();

        bonusCanvas.SetActive(false);
    }
}