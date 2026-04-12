using UnityEngine;
using UnityEngine.UI;

public class gridtimerscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Slider ring;
    [SerializeField] private Image fillcolor;

    void Start()
    {
        ring = GetComponent<Slider>();
    }
    public void decreaseValue()
    {
        if (ring.value <= 0)
            return;

        ring.value--;
    }
    public void increaseValue()
    {
        if (ring.value >= 3)
            return;

        ring.value++;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(":" + ring.value);
        switch (ring.value)
        {
            case 1:
                fillcolor.color = Color.red;
                break;
            case 2:
                fillcolor.color = Color.yellow;
                break;
            case 3:
                fillcolor.color = Color.green;
                break;
            default:
                break;
        }
    }
}
