using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
 
using TMPro;
using System.Collections;
public class MenuController : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public static event System.Action<SFXSounds> OnPause;
    public static event System.Action<SFXSounds> OnUnpause;

    [SerializeField] private GameObject settingsPanel, menuPanel, leaderBoardPanel, gameOverPanel;
    [SerializeField] private float mpEntryLength = 0.4f;
    [SerializeField] private TMP_Text gameOverText;

    private Image backgroundImg, gameOverImg;
    private Vector3 menuPanelOriginPos, panelEntryStartPos;
    [SerializeField] private Vector3 entryPosOffset = new Vector3(0, -1500, 0);

    [SerializeField] private Animator Transition;
    [SerializeField] private float fadeTime;

    private void Awake()
    {
        menuPanelOriginPos = menuPanel.transform.position;
        panelEntryStartPos = menuPanelOriginPos + entryPosOffset;
        menuPanel.transform.position = panelEntryStartPos;
        backgroundImg = settingsPanel.GetComponent<Image>();
        backgroundImg.DOFade(0f, 0f);
        settingsPanel.SetActive(false);
        if (gameOverPanel != null)
        {
            gameOverImg = gameOverPanel.GetComponent<Image>();
            gameOverImg.DOFade(0f, 0f);
            gameOverPanel.SetActive(false);
        }


    }
 
    public void GoToStartMenu()
    {
        StartCoroutine(LoadSceneIndex(0));
    }

    public void StartGame()
    {
        Debug.Log("changing to scene 1");
        StartCoroutine(LoadSceneIndex(1));
        gameIsPaused = false;
    }
    public void TimeGame()
    {
        StartCoroutine(LoadSceneIndex(2));
        gameIsPaused = false;
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ReloadActiveScene()
    {
        StartCoroutine(LoadSceneIndex(SceneManager.GetActiveScene().buildIndex));
    }

    private IEnumerator LoadSceneIndex(int levelIndex)
    {
        Transition.SetTrigger("Start");

        yield return new WaitForSeconds(fadeTime);

        SceneManager.LoadScene(levelIndex);
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
        OnPause?.Invoke(SFXSounds.placement_sound);
    }

    private void PausePanelEnterAnim()
    {
        menuPanel.transform.DOMove(menuPanelOriginPos, mpEntryLength).SetEase(Ease.OutBack);

        backgroundImg.DOFade(0.6f, mpEntryLength);

        Sequence sequenceX = DOTween.Sequence();
        sequenceX.Append(menuPanel.transform.DOScaleX(0.8f, 0.22f));
        sequenceX.Append(menuPanel.transform.DOScaleX(1.1f, 0.03f));
        sequenceX.Append(menuPanel.transform.DOScaleX(1f, 0.06f));

        Sequence sequenceY = DOTween.Sequence();
        sequenceY.Append(menuPanel.transform.DOScaleY(1.2f, 0.22f));
        sequenceY.Append(menuPanel.transform.DOScaleY(0.9f, 0.03f));
        sequenceY.Append(menuPanel.transform.DOScaleY(1f, 0.06f));
    }

    public void ClosePausePanel()
    {
        OnUnpause?.Invoke(SFXSounds.placement_sound);
        PausePanelExitAnim();
        menuPanel.transform.DOScale(0.1f, 0.2f).SetEase(Ease.InBack).onComplete = () => { 
            settingsPanel.SetActive(false);
            gameIsPaused = false;
            menuPanel.transform.position = panelEntryStartPos;
            menuPanel.transform.localScale = new Vector3(1f, 1f, 1f);
        };
    }

    private void PausePanelExitAnim()
    {
        backgroundImg.DOFade(0f, 0.2f);
        menuPanel.transform.DOScale(0.1f, 0.2f).SetEase(Ease.InBack).WaitForCompletion();
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        gameIsPaused = true;
        if(string.IsNullOrEmpty(GridManager.Instance.Whydied))
            Debug.LogError("Reason to death not given by Whydied, instead was: " + GridManager.Instance.Whydied);

        gameOverText.text = GridManager.Instance.Whydied;
        GameOverPanelEnterAnim();
    }

    private void GameOverPanelEnterAnim()
    {
        Transform gameOverText = gameOverPanel.transform.Find("Game Over (TMP)");
        Transform restartButton = gameOverPanel.transform.Find("RestartGameButton");

        gameOverPanel.GetComponent<Image>().DOFade(0.5f, 0.15f);

        Sequence scaleTextSeq = DOTween.Sequence();
        scaleTextSeq.Append(gameOverText.DOScale(Vector3.one, 0f));
        scaleTextSeq.Append(gameOverText.DOScale(new Vector3(1.6f, 1.6f, 1f), 0.15f)).SetEase(Ease.OutBack);
        scaleTextSeq.Append(gameOverText.DOScale(Vector3.one, 0.20f)).SetEase(Ease.OutBounce);

        gameOverText.DOShakeRotation(0.45f, 30, 8, 60, true, ShakeRandomnessMode.Harmonic).SetEase(Ease.OutBack);
        /*Sequence rotateTextSeq = DOTween.Sequence();
        rotateTextSeq.Append(gameOverText.DORotate(Vector3.zero, 0f));
        rotateTextSeq.Append(gameOverText.DORotate(Vector3.back * 5f, 0.15f).SetEase(Ease.OutBack));
        rotateTextSeq.Append(gameOverText.DORotate(Vector3.forward * 5f, 0.20f).SetEase(Ease.OutBack));
        rotateTextSeq.Append(gameOverText.DORotate(Vector3.zero, 0.15f).SetEase(Ease.OutBack));*/

        restartButton.transform.position = panelEntryStartPos;
        Sequence moveButtonSeq = DOTween.Sequence();
        moveButtonSeq.Append(restartButton.DOLocalMove(Vector3.down * 450f, 0.75f)).SetEase(Ease.InOutBack).SetDelay(0.1f);
    }



}
