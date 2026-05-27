using UnityEngine;
using UnityEngine.UI;
<<<<<<< Updated upstream

public class gridtimerscript : MonoBehaviour
=======
public class GridTimerScript : MonoBehaviour
>>>>>>> Stashed changes
{
    private Slider ring;
<<<<<<< Updated upstream
    private bool frozen = false;
    [SerializeField] private Image fillcolor;

=======
    private bool frozen;
    [SerializeField] private Image fillColor;
    [SerializeField] private Image counterObj;
    [SerializeField] private Sprite[] counterSprites;
    private int maxValue = 3;
>>>>>>> Stashed changes
    void Awake()
    {
        ring = GetComponent<Slider>();
        resetValue();
    }
<<<<<<< Updated upstream
=======
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
   
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        if (frozen) return;
        switch (ring.value)
=======
        bool isAcuallyImmune = frozen || (GridManager.Instance != null && (GridManager.Instance.hasImmunity || GridManager.Instance.linesClearedThisRound));
        if (isAcuallyImmune)
        {
            counterObj.sprite = counterSprites[3];
            fillColor.color = Color.cyan;
            return;
        }
        switch ((int)ring.value)
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
}
=======
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
>>>>>>> Stashed changes
