using UnityEngine;
using UnityEngine.UI;

public class gridtimerscript : MonoBehaviour
{
    private Slider ring;
    [SerializeField] private Image fillcolor;

    void Awake()
    {
        ring = GetComponent<Slider>();
        resetValue();
    }
    public void decreaseValue()
    {
        if (ring.value <= 0)
            return;

        ring.value--;
        Refresh();
    }
    public void resetValue()
    {
        ring.value = 3;
        Refresh();
    }
    public void increaseValue()
    {
        if (ring.value >= 3)
            return;

        ring.value++;
        Refresh();
    }

    private void Refresh()
    {
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
        }
    }
}
