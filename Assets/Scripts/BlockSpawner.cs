using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    private int BlocksUsed = 0;
    public static BlockSpawner Instance;

    [Header("Settings")]
    public GameObject[] blockPrefabs;
    [SerializeField] private gridtimerscript gridtimerscript;
    public Transform[] spawnPoints;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnNewBlock();
    }

    public void SpawnNewBlock()
    {
        gridtimerscript.resetValue();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int randomIndex = Random.Range(0, blockPrefabs.Length);

            int attempts = 0;

            while (GridManager.Instance.CanBlockFit(blockPrefabs[randomIndex]) == false)
            {
                randomIndex = Random.Range(0, blockPrefabs.Length);
                attempts++;

                if (attempts > 50)
                {
                    Debug.Log("Inget block får plats! Här ska vi trigga Game Over senare.");
                    break;
                }
            }

            Instantiate(blockPrefabs[randomIndex], spawnPoints[i].position, Quaternion.identity);
        }
    }
    public void BlockPlaced()
    {
        BlocksUsed++;
        gridtimerscript.decreaseValue();
        if (BlocksUsed >= 3)
        {
            BlocksUsed = 0;
            GridManager.Instance.OnTurnFinished();
            SpawnNewBlock();
        }
        GridManager.Instance.CheckIfPlayable();
    }
}