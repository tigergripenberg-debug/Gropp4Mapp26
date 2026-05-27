using DG.Tweening;
using UnityEngine;

public class hand : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private Sprite handopen, handclose;
    [SerializeField] private float duration = 1.8f;
    private Sequence animationSequence;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        gameObject.SetActive(false); // Gömd tills tutorialen startar
    }

    public void animate(Vector3 startPos, Vector3 end)
    {
        Stop();
        gameObject.SetActive(true);
        
        animationSequence = DOTween.Sequence();
        
        // Startläge: Handen är öppen och startar vid blocket
        transform.position = startPos;
        SetHandState(true); // Öppen

        // 1. Kläm åt om blocket (stäng handen)
        animationSequence.AppendCallback(() => SetHandState(false));
        animationSequence.AppendInterval(0.2f); 

        // 2. Lyft handen uppåt lite i Y-led (visar dragOffset-lyftet!)
        Vector3 liftPos = startPos + new Vector3(0f, 1f, 0f);
        animationSequence.Append(transform.DOMove(liftPos, 0.3f).SetEase(Ease.OutQuad));

        // 3. Dra blocket till målet på griden (behåll lyft-höjden)
        Vector3 finalTarget = end + new Vector3(0f, 1f, 0f);
        animationSequence.Append(transform.DOMove(finalTarget, duration).SetEase(Ease.OutCubic));
        animationSequence.AppendInterval(0.1f);

        // 4. Släpp blocket (öppna handen)
        animationSequence.AppendCallback(() => SetHandState(true));
        animationSequence.AppendInterval(0.2f);

        // 5. Åk tillbaka till startpositionen tom och öppen
        animationSequence.Append(transform.DOMove(startPos, duration * 0.8f).SetEase(Ease.InOutSine));
        
        animationSequence.SetLoops(-1); // Loopa för evigt
    }

    public void Stop()
    {
        if (animationSequence != null && animationSequence.IsActive())
        { 
            animationSequence.Kill(); 
        }
        gameObject.SetActive(false);
        animationSequence = null;
    }

    private void SetHandState(bool isOpen)
    {
        if (sr != null && handopen != null && handclose != null) 
        { 
            sr.sprite = isOpen ? handopen : handclose; 
        }
    }
}