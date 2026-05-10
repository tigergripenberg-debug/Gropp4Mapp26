using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance;

    [Header("Settings")]
    public GameObject[] blockPrefabs;
    public Transform[] spawnPoints;
    public GameObject blockPrefab;
    [SerializeField] public Color[] blockColors;
    private Shape[] shapes = ShapeLibrary.allShapes;
    public List<Shape> currentShapes = new();

    void Awake()
    {
        Instance = this;
    }
    
    private Shape GetWeightedRandomShape()
    {
        float fill = GridManager.Instance.GetBoardFillPercentage();
        Shape[] validShapes = shapes.Where(s => GridManager.Instance.CanBlockFit(s)).ToArray();
        if (validShapes.Length == 0) return null;
        int maxCells = validShapes.Max(s => s.CellCount);
        
        List<float> weights = new();
        float totalWeight = 0f;
        foreach (Shape shape in validShapes)
        {
            float weight = Mathf.Lerp(shape.CellCount, (maxCells + 1) - shape.CellCount, fill);
            weights.Add(weight);
            totalWeight += weight;
        }
        for (int i = 0; i < validShapes.Length; i++) // this forloop is strictly for debuglog to see block % spawnrate, not necessary at release
        {
            float chance = (weights[i] / totalWeight) * 100;
            Debug.Log(
                $"{validShapes[i].Name} " +
                $"Weight: {weights[i]:F2} " +
                $"Chance: {chance:F1}");
        }
        float random = Random.value * totalWeight;
        for (int i = 0; i < validShapes.Length; i++)
        {
            random -= weights[i];
            if (random <= 0f)
            {
                Debug.Log("Shapes list length" + currentShapes.Count);
                return validShapes[i];
            }
        } 
        return validShapes[0];
    }

    private GameObject CreateShapeVisuals(Shape shape, Transform spawnpoint)
    {
        GameObject shapeGO = new GameObject("Shape");
        shapeGO.transform.SetParent(transform);
        ShapeBehaviour shapeBehaviour = shapeGO.AddComponent<ShapeBehaviour>();
        BoxCollider2D col = shapeGO.AddComponent<BoxCollider2D>();
        col.enabled = false;
        shapeGO.transform.position = spawnpoint.position;
        Vector2Int origin = shape.GetOriginCell();
        foreach (var cell in shape.cells)
        {
            GameObject block = Instantiate(blockPrefab, shapeGO.transform);
            Vector3 localPos = new Vector3(cell.x - origin.x, cell.y - origin.y, 0f);
            block.transform.localPosition = localPos;
        }

        if (shapeGO.transform.childCount > 0)
        {
            float minX = shapeGO.transform.GetChild(0).localPosition.x;
            float maxX = minX;
            float minY = shapeGO.transform.GetChild(0).localPosition.y;
            float maxY = minY;

            foreach (Transform child in shapeGO.transform)
            {
                Vector3 pos = child.localPosition;
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;
            }
            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;
            Vector3 offset = new Vector3(centerX * 0.6f, centerY * 0.6f, 0f);
            shapeGO.transform.position -= offset;
        }
        shapeBehaviour.Initialize(shape,blockColors,blockPrefab);
        shapeBehaviour.FitColliderToShape();
        col.enabled = true;
        return shapeGO;
    }
    
    public void SpawnShapes()
    {
        currentShapes.Clear();
        for (var i = 0; i < spawnPoints.Length; i++)
        {
            Shape shape = GetWeightedRandomShape();
            currentShapes.Add(shape);
            CreateShapeVisuals(shape, spawnPoints[i]);
        }
    }
    
    public void RestoreShapes(string[] shapeNames)
    {
        currentShapes.Clear();
        var count = MathF.Min(shapeNames.Length, spawnPoints.Length);
        for (int i = 0; i < count; i++)
        {
            Shape shape = ShapeLibrary.GetByName(shapeNames[i]);
            if (shape == null) continue;
            currentShapes.Add(shape);
            CreateShapeVisuals(shape, spawnPoints[i]);
        }
    }
    
    public void BlockPlaced()
    {
        //BlocksUsed++;
        Debug.Log(currentShapes.Count);
        if (Score.Instance != null)
        {
            Score.Instance.RegisterBlockPlaced();
        }
        if (Timer.Instance != null)
        {
            Timer.Instance.RegisterBlockPlaced();
        }

        if (!GridManager.Instance.linesClearedThisRound && !GridManager.Instance.hasImmunity)
        {
            GridTimerScript.Instance.decreaseValue();
        }
        
        if (currentShapes.Count <= 0)
        {
            //BlocksUsed = 0;
            GridManager.Instance.OnTurnFinished();
            SpawnShapes();
        }
        
        GridManager.Instance.CheckIfPlayable();
    }

    public void ClearCurrentShapes()
    {
        currentShapes.Clear();
    }
}