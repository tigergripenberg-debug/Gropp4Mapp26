using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static bool gameIsPaused = false;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject leaderBoardPanel;

    [SerializeField] private float mpEntryLength = 0.4f;

    private Image backgroundImg;
    private Color backgorundColor;

    private void Awake()
    {
        menuPanel.transform.position = menuPanel.transform.parent.position + new Vector3(0, -1500, 0);
        backgroundImg = settingsPanel.GetComponent<Image>();

        backgorundColor = backgroundImg.color;
        backgorundColor.a = 0f;
    }

    public void StartGame()
    {
        Debug.Log("changing to scene 1");
        SceneManager.LoadScene(1);
        gameIsPaused = false;
    }
    public void TimeGame()
    {
        SceneManager.LoadScene(2);
        gameIsPaused = false;
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel.activeInHierarchy)
        {
            CloseSettingsPanel();
            return;
        }
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
    public void OpenLeaderBoardPanel()
    {
        if (leaderBoardPanel.activeInHierarchy)
        {
            CloseLeaderBoardPanel();
            return;
        }
        leaderBoardPanel.SetActive(true);
        leaderBoardPanel.GetComponent<leaderboardpanel>().Refresh();
    }

    public void CloseLeaderBoardPanel()
    {
        leaderBoardPanel.SetActive(false);
    }

    public void OpenPausePanel()
    {
        OpenSettingsPanel();
        gameIsPaused = true;
        PausePanelEnterAnim();
    }

    private void PausePanelEnterAnim()
    {
        var sequenceX = DOTween.Sequence();
        var sequenceY = DOTween.Sequence();

        menuPanel.transform.DOMove(menuPanel.transform.parent.position, mpEntryLength).SetEase(Ease.OutBack);
        backgroundImg.DOFade(0.6f, mpEntryLength);

        sequenceX.Append(menuPanel.transform.DOScaleX(0.8f, 0.22f));
        sequenceX.Append(menuPanel.transform.DOScaleX(1.1f, 0.03f));
        sequenceX.Append(menuPanel.transform.DOScaleX(1f, 0.06f));

        sequenceY.Append(menuPanel.transform.DOScaleY(1.2f, 0.22f));
        sequenceY.Append(menuPanel.transform.DOScaleY(0.9f, 0.03f));
        sequenceY.Append(menuPanel.transform.DOScaleY(1f, 0.06f));
    }

    public void ClosePausePanel()
    {
        StartCoroutine(PausePanelExitAnim());
    }

    private IEnumerator PausePanelExitAnim()
    {
        backgroundImg.DOFade(0f, 0.2f);
        yield return menuPanel.transform.DOScale(0.1f, 0.2f).SetEase(Ease.InBack).WaitForCompletion();
        settingsPanel.SetActive(false);
        gameIsPaused = false;
        menuPanel.transform.position = menuPanel.transform.parent.position + new Vector3(0, -1500, 0);
        menuPanel.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void GoToStartMenu()
    {
        SceneManager.LoadScene(0);
    }

}
