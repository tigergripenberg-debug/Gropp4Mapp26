using UnityEngine;
using UnityEngine.UI;

public class GridTimerScript : MonoBehaviour
{
    public static GridTimerScript Instance;

    private Slider ring;
    // Tror inte jag gjort detta så chansar inte.
    [SerializeField] private Image fillColor;
    [SerializeField] private Image counterObj;
    [SerializeField] private Sprite[] counterSprites;
    
    // ==========================================
    // Tiger: VARIABEL START

    private bool frozen;

    private int maxValue = 3;
    // ==========================================
    // Tiger: SLUT
    // ==========================================

    void Awake()
    {
        Instance = this;
        ring = GetComponent<Slider>();
        resetValue();
    }

    // ==========================================
    // Tiger: Start
    // Lyssnar på Score-skriptet. När en combo sker 
    // uppdateras timern automatiskt utan att skripten är hårt låsta till varandra.
    void OnEnable()
    {
        Score.OnComboChanged += HandleComboChanged;
    }

    void OnDisable()
    {
        Score.OnComboChanged -= HandleComboChanged;
    }
 
    // Funktioner för att låsa timern (Cyan) när spelaren 
    // gör poäng, och tina upp den (Green) när rundan går vidare.
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

    // Hanterar ringens faktiska värde. Om den inte är fryst,
    // tickar den neråt. Den kan också tvingas till maxVärde vid reset.
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

    public int getRingValue()
    {
        return (int)ring.value;
    }

    public void setRingValue(int value)
    {
        if (frozen)
        {
            return;
        }
        ring.value = value;
        Refresh();
    }

    public bool getFrozenStatus()
    {
        return frozen;
    }

    // Kärnan i feedbacken: Tittar på ringens värde och byter
    // både färg (grön, gul, röd, cyan) och siffra/ikon i UI:t.
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

    // Fångar upp Combos och agerar direkt genom att
    // göra spelaren immun (Cyan) och återställa värdet.
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
    // ==========================================
    // Tiger: SLUT
    // ==========================================
}