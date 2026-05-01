using UnityEngine;
using UnityEngine.UI;

public class GridTimerScript : MonoBehaviour
{
    public static GridTimerScript Instance;
    private Slider ring;
    private bool frozen;
    [SerializeField] private Image fillColor;
    private int maxValue = 3;

    void Awake()
    {
        Instance = this;
        ring = GetComponent<Slider>();
        resetValue();
    }

    void OnEnable()
    {
        Score.OnComboChanged += HandleComboChanged;
    }

    void OnDisable()
    {
        Score.OnComboChanged -= HandleComboChanged;
    }
    
    private void HandleComboChanged(int combo)
    {
        if (combo > 0)
        {
            Debug.Log("Freezing with combo");
            freeze(true);
            resetValue(); 
        }
        else
        {
            Debug.Log("Unfreezing");
            freeze(false);
        }
    }
    
    public void freeze(bool state)
    {
        frozen = state;
        Refresh();
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
        ring.value = maxValue;
        Refresh();
    }

    private void Refresh()
    {
        if (frozen)
        {
            fillColor.color = Color.cyan;
            return;
        }
        switch ((int)ring.value)
        {
            case 1:
                fillColor.color = Color.red;
                break;
            case 2:
                fillColor.color = Color.yellow;
                break;
            case 3:
                fillColor.color = Color.green;
                break;
        }
    }
}
