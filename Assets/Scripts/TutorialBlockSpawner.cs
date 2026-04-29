using UnityEngine;

public class TutorialBlockSpawner : MonoBehaviour
{
    public GameObject[] blockPrefabs;
    [SerializeField] private gridtimerscript gridtimerscript;
    public Transform[] spawnPoints;

    private void SpawnNewBlock()
    {
        gridtimerscript.resetValue();
        GameObject spawnedBlock1 = Instantiate(blockPrefabs[0], spawnPoints[0].position, Quaternion.identity);
        GameObject spawnedBlock2 = Instantiate(blockPrefabs[1], spawnPoints[1].position, spawnPoints[1].rotation);
        
    }
}
