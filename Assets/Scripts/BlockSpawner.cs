using System;
using System.Collections;
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
    [SerializeField] private BlockColorPalette currentPalette;
    
    private Shape[] shapes = ShapeLibrary.allShapes;
    public List<Shape> currentShapes = new();

    private Shape[] smallShapes;
    private Shape[] mediumShapes;
    private Shape[] largeShapes;

    void Awake()
    {
        Instance = this;
        smallShapes = shapes.Where(s => s.CellCount <= 3).ToArray();
        mediumShapes = shapes.Where(s => s.CellCount == 4).ToArray();
        largeShapes = shapes.Where(s => s.CellCount >= 5).ToArray();
    }
    
    private List<Shape> GenerateSkillBasedDraft()
    {
        List<Shape> draft = new List<Shape>();

        draft.Add(GetPerfectSolutionShape());

        float fill = GridManager.Instance != null ? GridManager.Instance.GetBoardFillPercentage() : 0f;
        int chanceSmall = fill < 0.3f ? 15 : (fill < 0.6f ? 30 : 50);
        int chanceMedium = fill < 0.3f ? 45 : (fill < 0.6f ? 55 : 45);

        for (int i = 0; i < 2; i++)
        {
            int roll = Random.Range(0, 100);
            if (roll < chanceSmall) draft.Add(smallShapes[Random.Range(0, smallShapes.Length)]);
            else if (roll < chanceSmall + chanceMedium) draft.Add(mediumShapes[Random.Range(0, mediumShapes.Length)]);
            else draft.Add(largeShapes[Random.Range(0, largeShapes.Length)]);
        }

        int safetyNet = 0;
        while (!IsDraftPlayable(draft) && safetyNet < 5)
        {
            draft[1] = smallShapes[Random.Range(0, smallShapes.Length)];
            draft[2] = smallShapes[Random.Range(0, smallShapes.Length)];
            safetyNet++;
        }
        return draft.OrderBy(x => Random.value).ToList();
    }

    private Shape GetPerfectSolutionShape()
    {
        if (GridManager.Instance == null || GridManager.Instance.gridLogic == null) return shapes[Random.Range(0, shapes.Length)];

        int[,] logic = GridManager.Instance.gridLogic;
        int w = GridManager.Instance.width;
        int h = GridManager.Instance.height;
    
        int bestRow = -1, bestCol = -1;
        int maxFilled = -1;
        
        for(int y=0; y<h; y++) {
            int f = 0; for(int x=0; x<w; x++) if(logic[x,y]==1) f++;
            if(f > maxFilled && f < w) { maxFilled = f; bestRow = y; bestCol = -1; }
        }
        for(int x=0; x<w; x++) {
            int f = 0; for(int y=0; y<h; y++) if(logic[x,y]==1) f++;
            if(f > maxFilled && f < h) { maxFilled = f; bestRow = -1; bestCol = x; }
        }

        if (maxFilled == -1) return shapes[Random.Range(0, shapes.Length)];

        Shape bestShape = null;
        int bestScore = -1;

        var shuffledShapes = shapes.OrderBy(s => Random.value).ToList();

        foreach (Shape s in shuffledShapes)
        {
            for (int cx = 0; cx < w; cx++)
            {
                for (int cy = 0; cy < h; cy++)
                {
                    Vector2Int testPos = new Vector2Int(cx, cy);
                    if (GridManager.Instance.CanPlaceShapeAtPosition(s, testPos))
                    {
                        int cellsInTargetLine = 0;
                        Vector2Int origin = s.GetOriginCell();
                        
                        foreach (var cell in s.cells)
                        {
                            int px = cx + (cell.x - origin.x);
                            int py = cy + (cell.y - origin.y);
                            
                            if (bestRow != -1 && py == bestRow) cellsInTargetLine++;
                            if (bestCol != -1 && px == bestCol) cellsInTargetLine++;
                        }

                        if (cellsInTargetLine > 0)
                        {
                            int score = (cellsInTargetLine * 10) + s.CellCount;
                            
                            if (score > bestScore)
                            {
                                bestScore = score;
                                bestShape = s;
                            }
                        }
                    }
                }
            }
        }

        return bestShape != null ? bestShape : shapes[Random.Range(0, shapes.Length)];
    }
    
    public void RefreshActiveShapeColors()
    {
        ShapeBehaviour[] shapes =
            FindObjectsByType<ShapeBehaviour>(
                FindObjectsSortMode.None);

        foreach (var shape in shapes)
        {
            shape.RefreshColors(currentPalette);
        }
    }

    private bool IsDraftPlayable(List<Shape> draft)
    {
        if (GridManager.Instance == null) return true;
        foreach (Shape s in draft) if (GridManager.Instance.CanBlockFit(s)) return true;
        return false;
    }

    public void SpawnShapes()
    {
        
        currentShapes.Clear();
        List<Shape> draft = GenerateSkillBasedDraft();

        for (var i = 0; i < spawnPoints.Length; i++)
        {
            Shape shape = draft[i];
            Debug.Log(draft.Count);
            currentShapes.Add(shape);
            CreateShapeVisuals(shape, spawnPoints[i]);
        }
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
        
        shapeBehaviour.Initialize(shape, blockPrefab, currentPalette);
        shapeBehaviour.FitColliderToShape();
        col.enabled = true;
        return shapeGO;
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
        StartCoroutine(BlockPlacedRoutine());
    }
    
    public void SetPalette(BlockColorPalette palette)
    {
        currentPalette = palette;
    }

    private IEnumerator BlockPlacedRoutine()
    {
        if (Score.Instance != null) Score.Instance.RegisterBlockPlaced();
        if (Timer.Instance != null) Timer.Instance.RegisterBlockPlaced();

        if (!GridManager.Instance.linesClearedThisRound && !GridManager.Instance.hasImmunity)
        {
            GridTimerScript.Instance.decreaseValue();
        }

        while (GridManager.Instance.isClearing)
        {
            yield return null; 
        }

        if (currentShapes.Count <= 0)
        {
            GridManager.Instance.OnTurnFinished();
            SpawnShapes();
        }

        yield return null;

        GridManager.Instance.CheckIfPlayable();
    }

    public void ClearCurrentShapes()
    {
        currentShapes.Clear();
    }
}