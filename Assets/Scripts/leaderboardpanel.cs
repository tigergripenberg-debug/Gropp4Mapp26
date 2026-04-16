using TMPro;
using UnityEngine;

public class leaderboardpanel : MonoBehaviour
{
    [SerializeField] private TMP_Text highscoretext;
    [SerializeField] private TMP_Text hightimetext;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Refresh()
    {
        highscoretext.text = "Normal: " + PlayerPrefs.GetInt("Highscore", 0).ToString();
        hightimetext.text = "Timed: " + PlayerPrefs.GetInt("Hightime", 0).ToString();
    }

}
