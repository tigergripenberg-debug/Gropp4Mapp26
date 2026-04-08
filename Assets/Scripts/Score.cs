using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;
    private SoundManager soundManager;
    public int score;
    private int highscore;
    void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        scoreText.text = score.ToString();
        highscoreText.text = "Best: " + highscore;
    }
    

    public static event System.Action<string> OnScoreChange;
    
    public void AddScore(int combo, string soundName)
    {
        int points = (combo * 100) * combo;
        score += points;
        scoreText.text = score.ToString();
        OnScoreChange?.Invoke(soundName);
        if (highscore < score)
        {
            highscore = score;
            highscoreText.text = "Best: " + highscore;
            PlayerPrefs.SetInt("Highscore", highscore);
            PlayerPrefs.Save();
        }
    }
}