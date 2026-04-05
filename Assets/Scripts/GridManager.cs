using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    void Awake()
    {
        Instance = this;

        gridLogic = new int[width, height];
        visualGrid = new Transform[width, height];

        GenerateGrid();
    }

    [Header("Inställningar")]
    public int width = 8;
    public int height = 8;
    public GameObject tilePrefab;
    private Score score;
    public int[,] gridLogic;
    public Transform[,] visualGrid;

    void Start()
    {
        score = GameObject.FindGameObjectWithTag("Scorer").GetComponent<Score>();
        GenerateGrid();
    }

    void GenerateGrid()
    {

        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 0) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                GameObject newTile = Instantiate(tilePrefab);

                newTile.transform.position = new Vector2(x - xOffset, y - yOffset + 2f);

                newTile.name = $"Tile X:{x} Y:{y}";

                newTile.transform.SetParent(transform);
            }
        }
    }
    public void CheckForMatches()
    {
        List<int> rowsToClear = new List<int>();
        List<int> columnsToClear = new List<int>();

        for (int y = 0; y < height; y++)
        {
            bool isRowFull = true;
            for (int x = 0; x < width; x++)
            {
                if (gridLogic[x, y] == 0)
                {
                    isRowFull = false;
                }
            }
            if (isRowFull) rowsToClear.Add(y);
        }
        for (int x = 0; x < width; x++)
        {
            bool isColumnFull = true;
            for (int y = 0; y < height; y++)
            {
                if (gridLogic[x, y] == 0)
                {
                    isColumnFull = false;

                }
            }
            if (isColumnFull) columnsToClear.Add(x);
        }

        int totalLines = rowsToClear.Count + columnsToClear.Count;
        if (totalLines > 0)
        {
            foreach (var row in rowsToClear)
            {
                ClearRow(row);
            }
            foreach (var column in columnsToClear)
            {
                ClearColumn(column);
            }
            score.addScore(totalLines);
        }
    }

    public bool CanBlockFit(GameObject blockPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool fitsHere = true;

                foreach (Transform child in blockPrefab.transform)
                {
                    int testX = x + Mathf.RoundToInt(child.localPosition.x);
                    int testY = y + Mathf.RoundToInt(child.localPosition.y);

                    if (testX < 0 || testX >= width || testY < 0 || testY >= height || gridLogic[testX, testY] == 1)
                    {
                        fitsHere = false;
                        break;
                    }
                }

                if (fitsHere)
                {
                    return true;
                }
            }
        }

        return false;
    }
    void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            gridLogic[x, y] = 0;
            if (visualGrid[x, y] != null)
            {

                Destroy(visualGrid[x, y].gameObject);
                visualGrid[x, y] = null;
            }
        }
    }
    void ClearColumn(int x)
    {
        for (int y = 0; y < height; y++)
        {
            gridLogic[x, y] = 0;
            if (visualGrid[x, y] != null)
            {
                Destroy(visualGrid[x, y].gameObject);
                visualGrid[x, y] = null;
            }
        }
    }

}