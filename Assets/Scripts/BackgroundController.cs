using System;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public Transform bg1, bg2;
    public float speed;
    private float width;


    private void Start()
    {
        width = bg1.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        bg1.position += Vector3.left * (speed * Time.deltaTime);
        bg2.position += Vector3.left * (speed * Time.deltaTime);
        if (bg1.position.x <= -width)
        {
            bg1.position = new Vector3(bg2.position.x + width, bg1.position.y, bg1.position.z);
        }

        if (bg2.position.x <= -width)
        {
            bg2.position = new Vector3(bg1.position.x + width, bg2.position.y, bg2.position.z);
        }
    }
}
