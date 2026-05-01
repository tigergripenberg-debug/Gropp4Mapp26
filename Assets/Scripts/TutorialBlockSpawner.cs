using System;
using UnityEngine;

public class TutorialBlockSpawner : MonoBehaviour
{
    public static TutorialBlockSpawner Instance;
    private int blocksUsed = 0;
    public GameObject[] blockPrefabs;
    public Transform[] spawnPoints;

    private void Start()
    {
        Instance = this;
        SpawnNewBlock();
    }


    private void SpawnNewBlock()
    {
        int index = 0;
        GridTimerScript.Instance.resetValue();
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject spawnedBlock1 = Instantiate(blockPrefabs[index++], spawnPoint.position, Quaternion.identity);
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            foreach (Transform child in spawnedBlock1.transform)
            {
                if (child.localPosition.x < minX) minX = child.localPosition.x;
                if (child.localPosition.x > maxX) maxX = child.localPosition.x;
                if (child.localPosition.y < minY) minY = child.localPosition.y;
                if (child.localPosition.y > maxY) maxY = child.localPosition.y;
            }

            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;
            Vector3 offset = new Vector3(centerX * 0.6f, centerY * 0.6f, 0f);
            spawnedBlock1.transform.position -= offset;
        }
    }
    
    public void BlockPlaced()
    {
        blocksUsed++;
        if (Score.Instance != null)
        {
            Score.Instance.RegisterBlockPlaced();
        }
        if (Timer.Instance != null)
        {
            Timer.Instance.RegisterBlockPlaced();
        }
        
        GridTimerScript.Instance.decreaseValue();
        
        if (blocksUsed >= 3)
        {
            blocksUsed = 0;
            GridManager.Instance.OnTurnFinished();
            SpawnNewBlock();
        }
        
        GridManager.Instance.CheckIfPlayable();
    }
}
