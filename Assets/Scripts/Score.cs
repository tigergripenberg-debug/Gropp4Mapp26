using DG.Tweening;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance;

    [Header("UI & References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private Transform popupSpawnPoint;

    [Header("Score Stats")]
    [SerializeField] private Color scoreColor = Color.white;
    public int score;
    private int highscore;
    public int currentCombo = 0;
    public int blocksSinceLastClear = 0;

    public static event System.Action<ScoreEventType> OnScoreChange;
    public static event System.Action<string> OnScoreMessage;
    public static event System.Action<int> OnComboChanged;
    private new Sequence animation;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        animation = DOTween.Sequence();
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        scoreText.text = score.ToString();
        highscoreText.text = "Best: " + highscore;
        UpdateMultiplierText();
    }
    private void Scorer(int number, ScoreEventType type)
    {
        Color tmpclr = scoreColor;
        DOTween.SetTweensCapacity(500, 50);
        float shakeStrength = 0.2f;
        switch (type)
        {
            case ScoreEventType.Small:
                shakeStrength = 0.2f;
                scoreColor = Color.yellow;
                break;
            case ScoreEventType.Medium:
                shakeStrength = 0.4f;
                scoreColor = Color.orange;
                break;
            case ScoreEventType.Big:
                shakeStrength = 0.8f;
                scoreColor = Color.red;
                break;
            case ScoreEventType.Jackpot:
                scoreColor = Color.purple;
                shakeStrength = 1.6f;
                break;
        }
        Camera.main.transform.DOShakePosition(1f, shakeStrength, 5, 20);
        int currentScore = int.Parse(scoreText.text);
        for (int i = currentScore; i <= number; i++)
        {
            scoreText.transform.DOScale(2f, 1f);
            scoreText.color = scoreColor;
            scoreText.text = (currentScore + 1).ToString();
            scoreText.transform.DOScale(1f, 1f);
            scoreText.color = tmpclr;
            scoreColor = tmpclr;
        }
    }
    
    private void SpawnScorePopup(string message, Color color)
    {
        GameObject popup = Instantiate(
            scorePopupPrefab,
            popupSpawnPoint.position,
            Quaternion.identity,
            popupSpawnPoint);

        ScorePopup popupScript =
            popup.GetComponent<ScorePopup>();

        popupScript.Setup(message, scoreColor);
    }

    public void RestoreScoreData(int savedScore, int savedCombo, int savedBlocksSinceLastClear)
    {
        score = savedScore;
        currentCombo = savedCombo;
        blocksSinceLastClear = savedBlocksSinceLastClear;
        scoreText.text = score.ToString();
        UpdateMultiplierText();
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
            UpdateMultiplierText();

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.ExitComboMusic();
            }
            Debug.Log("Combo bruten! Du lade 3 block utan att spränga en rad.");
        }
        else if (currentCombo > 0 && blocksSinceLastClear >= 0)
        {
            int blocksLeft = 3 - blocksSinceLastClear;
            Debug.Log("Kombon lever! Du har " + blocksLeft + " block till på dig.");
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
        UpdateMultiplierText();
        

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
        ThemeManager.Instance.UpdatePaletteFromScore(score);
        ScoreEventType type = GetScoreEventType(currentCombo);
        Scorer(score, type);
        CheckHighscore();

        string message = $"+ {pointsToGive}";
        OnScoreChange?.Invoke(type);
        OnScoreMessage?.Invoke(message);
        SpawnScorePopup(message, scoreColor);
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
    private void UpdateMultiplierText()
    {
        multiplierText.text = "x" + currentCombo;

        float targetAlpha = currentCombo > 1 ? 1f : 0f;

        multiplierText.DOFade(targetAlpha, 0.25f);
    }

    private ScoreEventType GetScoreEventType(int value)
    {
        if (value < 2) return ScoreEventType.Small;
        if (value < 4) return ScoreEventType.Medium;
        if (value < 6) return ScoreEventType.Big;
        return ScoreEventType.Jackpot;
    }
}