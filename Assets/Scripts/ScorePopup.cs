using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void Setup(string message, Color color)
    {
        text.text = message;
        text.color = new Color(
            color.r,
            color.g,
            color.b,
            0f);

        transform.localScale = Vector3.one * 0.8f;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            text.DOFade(1f, 0.15f)
        );

        seq.Join(
            transform.DOMoveY(
                transform.position.y + 1.5f,
                1f)
        );

        seq.Join(
            transform.DOScale(1f, 0.2f)
        );

        seq.Append(
            text.DOFade(0f, 0.35f)
        );

        seq.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}