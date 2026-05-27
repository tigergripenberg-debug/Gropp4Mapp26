using UnityEngine;
<<<<<<< Updated upstream

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    private void Start()
    {
        int playedTutorial = PlayerPrefs.GetInt("playedTutorial", 0);
        if (playedTutorial == 0)
        {
            ShowTutorial();
            PlayerPrefs.SetInt("playedTutorial", 1);
            PlayerPrefs.Save();
        }
    }
    public void ShowTutorial()
    {
        if (tutorialPanel.activeInHierarchy)
        {
            tutorialPanel.SetActive(false);
        }
        else
            tutorialPanel.SetActive(true);
=======
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Setup")]
    [SerializeField] private GameObject handObject; 
    
    [Header("Landningsplatser för Runda 1")]
    public Transform[] round1Targets; 

    [Header("Landningsplatser för Runda 2")]
    public Transform[] round2Targets; 

    private GameObject[] activeBlocks;
    private hand handScript;
    private int currentRound = 1; // Håller koll på vilken runda vi är på

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (handObject != null) handScript = handObject.GetComponent<hand>();
    }

    public void OnBlocksReady(GameObject[] spawnedBlocks)
    {
        activeBlocks = spawnedBlocks;
        if (SceneManager.GetActiveScene().name == "TutorialGame")
        {
            StartCoroutine(TutorialSequence());
        }
    }

    private IEnumerator TutorialSequence()
    {
        // Välj rätt landningsplatser baserat på vilken runda vi spelar
        Transform[] currentTargets = currentRound == 1 ? round1Targets : round2Targets;

        for (int i = 0; i < activeBlocks.Length; i++)
        {
            if (activeBlocks[i] == null) continue;

            ShapeBehaviour blockScript = activeBlocks[i].GetComponent<ShapeBehaviour>();
            if (blockScript == null) continue;

            // Aktivera handen igen ifall den gömdes i förra rundan
            if (handObject != null) handObject.SetActive(true);

            Collider2D col = activeBlocks[i].GetComponent<Collider2D>();
            Vector3 startPos = col != null ? col.bounds.center : activeBlocks[i].transform.position;

            Vector3 endPos = currentTargets.Length > i && currentTargets[i] != null 
                ? currentTargets[i].position 
                : Vector3.zero;

            if (handScript != null) handScript.animate(startPos, endPos);

            while (!blockScript.tutorialPlaced)
            {
                yield return null; 
            }

            if (handScript != null) handScript.Stop();
            yield return new WaitForSeconds(0.5f);
        }

        // --- ALLA 3 BLOCK PLACERADE FÖR DENNA RUNDA ---
        if (handObject != null) handObject.SetActive(false); // Göm handen tillfälligt

        if (currentRound == 1)
        {
            Debug.Log("Runda 1 är klar! Väntar på explosionen...");
            yield return new WaitForSeconds(2.5f); // Låt explosionen spelas upp

            Debug.Log("Startar Runda 2...");
            currentRound = 2; 
            TutorialBlockSpawner.Instance.StartSecondRound(); // Säg åt spawnern att skicka nya block
        }
        else if (currentRound == 2)
        {
            Debug.Log("Hela tutorialen är klar! Laddar MainGame...");
            yield return new WaitForSeconds(1.5f); 
            SceneManager.LoadScene("MainGame"); 
        }
>>>>>>> Stashed changes
    }
}