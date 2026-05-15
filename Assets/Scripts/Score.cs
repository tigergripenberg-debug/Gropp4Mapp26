using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance;

    [Header("UI & References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;

    [Header("Score Stats")]
    public int score;
    private int highscore;
    public int currentCombo = 0;
    public int blocksSinceLastClear = 0;

    public static event System.Action<ScoreEventType> OnScoreChange;
    public static event System.Action<string> OnScoreMessage;
    public static event System.Action<int> OnComboChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        scoreText.text = score.ToString();
        highscoreText.text = "Best: " + highscore;
    }

    public void RestoreScoreData(int savedScore, int savedCombo, int savedBlocksSinceLastClear)
    {
        score = savedScore;
        currentCombo = savedCombo;
        blocksSinceLastClear = savedBlocksSinceLastClear;
        scoreText.text = score.ToString();
    }

    public void RegisterBlockPlaced()
    {
        // 1. Varje gång ett block läggs ökar vi räknaren.
        // Om vi precis sprängde en rad är denna redan satt till -1, och blir nu 0.
        blocksSinceLastClear++;

        // 2. Har vi lagt 3 block utan att rensa något? Då dör kombon!
        if (blocksSinceLastClear >= 3 && currentCombo > 0)
        {
            currentCombo = 0;
            OnComboChanged?.Invoke(currentCombo);

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.ExitComboMusic();
            }
            // Debug.Log("Combo bruten! Du lade 3 block utan att spränga en rad.");
        }
        else if (currentCombo > 0 && blocksSinceLastClear >= 0)
        {
            int blocksLeft = 3 - blocksSinceLastClear;
            // Debug.Log("Kombon lever! Du har " + blocksLeft + " block till på dig.");
        }

        scoreText.text = score.ToString();
        CheckHighscore();
    }

    public void CalculateAndAddScore(int linesCleared, bool isBoardEmpty)
    {
        currentCombo++;

        // --- MAGIN HÄNDER HÄR ---
        // Vi sätter den till -1! Eftersom RegisterBlockPlaced körs millisekunden
        // efter detta, kommer räknaren nollställas till exakt 0.
        blocksSinceLastClear = -1;

        OnComboChanged?.Invoke(currentCombo);

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
            // Debug.Log("PERFECT CLEAR! +1000 bonuspoäng!");
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