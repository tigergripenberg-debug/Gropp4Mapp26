using System;
using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float driftStrength = 2f;
    private float randomOffset;

    private void Start()
    {
        randomOffset = UnityEngine.Random.Range(0f, 100f);
        StartCoroutine(BubbleAnimation());
    }

    private void Update()
    {
        float drift = Mathf.Sin(Time.time + randomOffset) * driftStrength;
        Vector3 movement = new Vector3(drift, floatSpeed, 0f);
        transform.position += movement * Time.deltaTime;
    }


    private IEnumerator BubbleAnimation()
    {
        float duration = 0.8f;
        float time = 0;
        float shrinkDuration = 0.8f;
        Vector2 startScale = Vector2.zero;
        Vector2 endScale = new Vector2(2f, 2f);
        Vector2 endPosition = new Vector2(1f, 1f);

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        while (time < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(endScale, Vector2.zero, time / shrinkDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
        Destroy(gameObject);
    }
}
