using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] GameObject[] blockPrefab;
    [SerializeField] Transform[] spawnPoints;
    public GameObject tilePrefab;
    private List<GameObject> currentBlocks = new List<GameObject>();
    public int blocksLeftIndex = 0;

    private void Start()
    {
        currentBlocks = new List<GameObject>();
        SpawnBlocks();
    }

    private void Update()
    {
        if (blocksLeftIndex == 0)
        {
            SpawnBlocks();
            blocksLeftIndex = spawnPoints.Length;
        }
    }

    private void SpawnBlocks()
    {
        ClearOldBlocks();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject prefab = GetRandomBlock();
            GameObject block = Instantiate(prefab, spawnPoints[i].position, Quaternion.identity);
            currentBlocks.Add(block);
        }
    }

    public void RemoveBlock(GameObject block)
    {
        if (currentBlocks.Contains(block))
        {
            currentBlocks.Remove(block);
        }
    }
    
    private GameObject GetRandomBlock()
    {
        int index = Random.Range(0, blockPrefab.Length);
        return blockPrefab[index];
    }
    private void ClearOldBlocks()
    {
        if (currentBlocks == null) return;
        foreach (GameObject block in currentBlocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }
        currentBlocks.Clear();
    }
}