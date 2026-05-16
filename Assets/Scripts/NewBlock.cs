using UnityEngine;

public class NewBlock : MonoBehaviour
{
    private SpriteRenderer sr;
    public Shape ShapeData { get; private set; }
    public int colorIndex;
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
}