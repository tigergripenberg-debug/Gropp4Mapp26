using DG.Tweening;
using UnityEngine;

public class hand : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private Sprite handopen, handclose;
    [SerializeField] private float duration = 2f;
    private new Sequence animation;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }
    public void animate(Vector3 startPos, Vector3 end)
    {
        Stop();
        gameObject.SetActive(true);
        animation = DOTween.Sequence();
        transform.position = startPos;
        animation.AppendCallback(() => HandOpen(false));
        animation.Append(transform.DOMove(end, duration).SetEase(Ease.InOutSine));
        animation.AppendCallback(() => HandOpen(true));
        animation.Append(transform.DOMove(startPos, duration).SetEase(Ease.InOutSine));
        animation.SetLoops(-1);

    }
    public void Stop()
    {
        if (animation != null && animation.IsActive())
        { animation.Kill(); }
        gameObject.SetActive(false);
        animation = null;
    }

    private void HandOpen(bool state)
    {
        if (sr != null) { sr.sprite = state ? handopen : handclose; }
    }
}
