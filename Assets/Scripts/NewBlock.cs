using UnityEngine;

public class NewBlock : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        if (sr != null)
            sr.color = color;
    }
}