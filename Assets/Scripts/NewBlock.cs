using UnityEngine;

public class NewBlock : MonoBehaviour
{
    private SpriteRenderer sr;
    public Shape ShapeData { get; private set; }

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