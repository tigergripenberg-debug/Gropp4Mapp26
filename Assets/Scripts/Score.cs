using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance;

    [Header("UI & References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;
    private SoundManager soundManager;
    
    [Header("Score Stats")]
    public int score;
    private int highscore;
    public int currentCombo = 0;
    public int blocksSinceLastClear = 0;

    public static event System.Action<ScoreEventType> OnScoreChange;
    public static event System.Action<string> OnScoreMessage;
    

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        scoreText.text = score.ToString();
        highscoreText.text = "Best: " + highscore;
    }

    public void RegisterBlockPlaced()
    {
        blocksSinceLastClear++;
        scoreText.text = score.ToString();
        CheckHighscore();
    }
    public void EvaluateComboState()
    {
         if(blocksSinceLastClear >= 3)
        {
            if (currentCombo > 0)
            {
                Debug.Log("Combo bruten!");
                currentCombo = 0;
            }
        }
    }
    

   public void CalculateAndAddScore(int linesCleared, bool isBoardEmpty)
    {
        currentCombo++;
        blocksSinceLastClear = 0; 
        int pointsForLines = 0;
        switch (linesCleared)
        {
            case 1: pointsForLines = 100; break;
            case 2: pointsForLines = 300; break; 
            case 3: pointsForLines = 600; break; 
            default: pointsForLines = 1000; break; 
        }
        float comboMultiplier = 1.0f + (currentCombo * currentCombo * 0.1f);
        int pointsToGive = Mathf.RoundToInt(pointsForLines * comboMultiplier);
        if (isBoardEmpty)
        {
            pointsToGive += 1000;
            Debug.Log("PERFECT CLEAR! +1000 bonuspoäng!");
        }
        score += pointsToGive;
        scoreText.text = score.ToString();
        ScoreEventType type = GetScoreEventType(currentCombo);
        CheckHighscore();
        string message = $"Multiplier: x{comboMultiplier:F1}\n Total {pointsToGive} points!";
        OnScoreChange?.Invoke(type);
        OnScoreMessage?.Invoke(message);
    }

    private void CheckHighscore()
    {
        if (highscore < score)
        {
            highscore = score;
            highscoreText.text = "Best: " + highscore;
            PlayerPrefs.SetInt("Highscore", highscore);
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