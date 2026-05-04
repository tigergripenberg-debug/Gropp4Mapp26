using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel, hand;
    [SerializeField] private GameObject[] blocks;
    private hand handscript;
    private void Start()
    {
        handscript = hand.GetComponent<hand>();
        if (SceneManager.GetActiveScene().name == "TutorialGame")
        {
            StartCoroutine(TutorialSequence());
        }
        int playedTutorial = PlayerPrefs.GetInt("playedTutorial", 0);
        if (playedTutorial == 0 && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().name != "TutorialGame")
        {
            ShowTutorial();
            PlayerPrefs.SetInt("playedTutorial", 1);
            PlayerPrefs.Save();
        }
    }
    private IEnumerator TutorialSequence()
    {
        for (int i = 0; i < blocks.Length && i < 3; i++)
        {
            Block blockScript = blocks[i].GetComponent<Block>();
            startHand(blocks[i].transform.position, i);

            // Wait until this block is placed
            while (!blockScript.IsPlaced)
            {

                yield return null; // Check each frame without freezing
            }
        }
        handscript.Stop();
    }
    private void startHand(Vector2 startPosition, int index)
    {
        foreach (GameObject item in blocks)
        {
            item.GetComponent<BoxCollider2D>().enabled = false;
        }
        blocks[index].GetComponent<BoxCollider2D>().enabled = true;
        Vector2 blockTarget = new Vector2(-3.5f, 0f);
        if (index == 1)
        {
            blockTarget = new(-1.5f, 0);
        }
        else if (index == 2)
        {
            blockTarget = new(-0.5f, 0);
        }
        handscript.animate(startPos: startPosition, end: blockTarget);
    }


    private void ShowTutorial()
    {
        if (tutorialPanel.activeInHierarchy)
        {
            tutorialPanel.SetActive(false);
        }
        else
            tutorialPanel.SetActive(true);
    }
    public void LoadTutorial()
    {
        SceneManager.LoadScene("TutorialGame");
    }
}