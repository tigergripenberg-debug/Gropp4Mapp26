using UnityEngine;
using UnityEngine.UI;

public class gridtimerscript : MonoBehaviour
{
    private Slider ring;
    private bool frozen = false;
    [SerializeField] private Image fillcolor;

    void Awake()
    {
        ring = GetComponent<Slider>();
        resetValue();
    }
    public void freeze(bool state)
    {
        if (state)
        {
            fillcolor.color = Color.turquoise;
            frozen = true;
        }
        else
        {
            frozen = false;
            Refresh();
        }
    }
    public void decreaseValue()
    {
        if (ring.value <= 0 || frozen)
            return;

        ring.value--;
        Refresh();
    }
    public void resetValue()
    {
        if (frozen) return;
        ring.value = 3;
        Refresh();
    }
    public void increaseValue()
    {
        if (frozen) return;
        if (ring.value >= 3)
            return;

        ring.value++;
        Refresh();
    }

    private void Refresh()
    {
        if (frozen) return;
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
