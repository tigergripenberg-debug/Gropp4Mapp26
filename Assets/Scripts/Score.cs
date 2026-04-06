using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    public int score;
    void Start()
    {
        scoreText.text = "" + score;
    }

    public void addScore(int combo)
    {
        int points = (combo * 100) * combo;
        score += points;
        scoreText.text = "" + score;
    }
}
