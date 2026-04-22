using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance;

    [Header("UI & References")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text hightimeText;
    private SoundManager soundManager;
    
    [Header("Time Stats")]
    public float time;
    private int hightime;
    public int currentCombo = 0;
    public int blocksSinceLastClear = 0;

    public static event System.Action<ScoreEventType> OnScoreChange;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        hightime = PlayerPrefs.GetInt("Hightime", 0);
        time = 100f;
        timeText.text = time.ToString();
        hightimeText.text = "Best: " + hightime;
    }

    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            timeText.text = Mathf.RoundToInt(time).ToString();
            
            CheckHightime(); 
        }
        else
        {
            GridManager.Instance.TriggerGameOver();
        }
    }

    public void RegisterBlockPlaced()
    {
        blocksSinceLastClear++;

        if (blocksSinceLastClear > 3)
        {
            if (currentCombo > 0)
            {
                Debug.Log("Time Combo bruten!");
            }
            currentCombo = 0;
        }
    }

    public void CalculateAndAddTime(int linesCleared, bool isBoardEmpty)
    {
        Debug.Log("Timer: Jag tog emot signalen! Lägger till tid nu...");
        currentCombo++;
        blocksSinceLastClear = 0;

        int timeToAdd = (linesCleared * linesCleared) * 10; 

        timeToAdd = timeToAdd * currentCombo;
        Debug.Log($"Tid tillagd! Sprängde {linesCleared} rader. Combo x{currentCombo} -> +{timeToAdd} sekunder");

        if (isBoardEmpty)
        {
            timeToAdd += 50;
            Debug.Log("PERFECT CLEAR! +50 sekunder!");
        }

        time += timeToAdd;
        timeText.text = Mathf.RoundToInt(time).ToString();

        ScoreEventType type = GetScoreEventType(linesCleared + (currentCombo - 1));
        OnScoreChange?.Invoke(type);
    }

    private void CheckHightime()
    {
        int currentTime = Mathf.RoundToInt(time);
        if (hightime < currentTime)
        {
            hightime = currentTime;
            hightimeText.text = "Best: " + hightime;
            PlayerPrefs.SetInt("Hightime", hightime);
            PlayerPrefs.Save();
        }
    }

    private ScoreEventType GetScoreEventType(int value)
    {
        if (value < 2) return ScoreEventType.Small;
        if (value < 4) return ScoreEventType.Medium;
        if (value < 6) return ScoreEventType.Big;
        return ScoreEventType.Jackpot;
    }
}