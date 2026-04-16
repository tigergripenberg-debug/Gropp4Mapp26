using TMPro;
using UnityEngine;

public class timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text hightimeText;
    private SoundManager soundManager;
    public float time;
    private int hightime;
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
        }
        else
        {
            GridManager.Instance.TriggerGameOver();
        }

    }

    public static event System.Action<ScoreEventType> OnScoreChange;

    public void AddScore(int combo)
    {
        int points = combo * 100 * combo;
        time += points;
        timeText.text = time.ToString();

        ScoreEventType type = GetScoreEventType(combo);
        OnScoreChange?.Invoke(type);
        if (hightime < time)
        {
            hightime = Mathf.RoundToInt(time);
            hightimeText.text = "Best: " + hightime;
            PlayerPrefs.SetInt("Hightime", hightime);
            PlayerPrefs.Save();
        }
    }
    private ScoreEventType GetScoreEventType(int combo)
    {
        if (combo < 2) return ScoreEventType.Small;
        if (combo < 4) return ScoreEventType.Medium;
        if (combo < 6) return ScoreEventType.Big;
        return ScoreEventType.Jackpot;
    }

}