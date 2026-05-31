using UnityEngine;
using UnityEngine.UI;

public class GridTimerScript : MonoBehaviour
{
    public static GridTimerScript Instance;
    private Slider ring;
    private bool frozen;
    [SerializeField] private Image fillColor;
    [SerializeField] private Image counterObj;
    [SerializeField] private Sprite[] counterSprites;
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

    public void SetColorCyan()
    {
        frozen = true;
        Refresh();
    }
    public void SetColorGreen()
    {
        frozen = false;
        Refresh();
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
        bool isAcuallyImmune = frozen || (GridManager.Instance != null && (GridManager.Instance.hasImmunity || GridManager.Instance.linesClearedThisRound));

        if (isAcuallyImmune)
        {
            counterObj.sprite = counterSprites[3];
            fillColor.color = Color.cyan;
            return;
        }
        switch ((int)ring.value)
        {
            case 1:
            counterObj.sprite = counterSprites[0];
                fillColor.color = Color.red;
                break;
            case 2:
                counterObj.sprite = counterSprites[1];
                fillColor.color = Color.yellow;
                break;
            case 3:
                counterObj.sprite = counterSprites[2];
                fillColor.color = Color.green;
                break;
        }
    }
    private void HandleComboChanged(int combo)
    {
        if (combo > 0)
        {
            SetColorCyan();
            resetValue(); 
        }
        else
        {
            SetColorGreen();
        }
    }
}
