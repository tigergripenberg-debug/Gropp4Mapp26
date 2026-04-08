using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [Header("Settings")] public int width = 8;
    public int height = 8;
    public GameObject tilePrefab;
    private Score score;
    public int[,] gridLogic;
    public Transform[,] visualGrid;
    public int turnsSinceClear = 0;
    public int maxTurnsSinceClear = 3;
    [SerializeField] GameObject gameOverCanvas;

    void Awake()
    {
        Instance = this;
        gridLogic = new int[width, height];
        visualGrid = new Transform[width, height];
    }

    void Start()
    {
        score = GameObject.FindGameObjectWithTag("Scorer").GetComponent<Score>();
        GenerateGrid();
    }
    
    void OnDrawGizmos()
    {
        if (visualGrid == null) return;
        Gizmos.color = Color.green;
        for (int x = 0; x <= width; x++)
        {
            Vector2 start = GetWorldPosition(x, 0);
            Vector2 end = GetWorldPosition(x, height);
            Gizmos.DrawLine(start, end);
        }
        for (int y = 0; y <= height; y++)
        {
            Vector2 start = GetWorldPosition(0, y);
            Vector2 end = GetWorldPosition(width, y);
            Gizmos.DrawLine(start, end);
        } 
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = GetWorldPosition(x, y);
                Gizmos.color = (visualGrid[x, y] != null) ? Color.red : Color.blue;
                Gizmos.DrawSphere(pos, 0.05f);
            }
        }
    }
    
    public Vector2 GetWorldPosition(int x, int y)
    {
        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 0) / 2f;

        return new Vector2(
            x - xOffset,
            y - yOffset + 2f
        );
    }
    
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 0) / 2f;

        int x = Mathf.RoundToInt(worldPos.x + xOffset);
        int y = Mathf.RoundToInt(worldPos.y + yOffset - 2f);

        return new Vector2Int(x, y);
    }

    public void RestartGame() //använder onclick event i unity
    {
        score.score = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void OnTurnFinished()
    {
        bool didClear = CheckForMatches();

        if (didClear)
        {
            turnsSinceClear = 0;
        }
        else
        {
            turnsSinceClear++;
        }

        Debug.Log("Turns since clear: " + turnsSinceClear);

        if (turnsSinceClear >= maxTurnsSinceClear)
        {
            Debug.Log("GRID PUSH!");
            MoveGridUp();
            turnsSinceClear = 0;
        }
    }

    public bool CheckForMatches()
    {
        bool didClear = false;

        List<int> rowsToClear = new List<int>();
        List<int> columnsToClear = new List<int>();

        // Check rows
        for (int y = 0; y < height; y++)
        {
            bool full = true;
            for (int x = 0; x < width; x++)
            {
                if (gridLogic[x, y] == 0)
                {
                    full = false;
                    break;
                }
            }

            if (full) rowsToClear.Add(y);
        }

        // Check columns
        for (int x = 0; x < width; x++)
        {
            bool full = true;
            for (int y = 0; y < height; y++)
            {
                if (gridLogic[x, y] == 0)
                {
                    full = false;
                    break;
                }
            }

            if (full) columnsToClear.Add(x);
        }

        int totalLines = rowsToClear.Count + columnsToClear.Count;

        if (totalLines > 0)
        {
            foreach (var row in rowsToClear)
                if (ClearRow(row))
                    didClear = true;

            foreach (var col in columnsToClear)
                if (ClearColumn(col))
                    didClear = true;

            score.AddScore(totalLines);
        }

        return didClear;
    }

    public void MoveGridUp()
    {
        if (IsGameOver())
        {
            Debug.Log("Game Over");
            gameOverCanvas.SetActive(true);
            return;
        }
        for (int x = 0; x < width; x++)
        {
            if (visualGrid[x, height - 1] != null)
            {
                Destroy(visualGrid[x, height - 1].gameObject);
                visualGrid[x, height - 1] = null;
            }
            gridLogic[x, height - 1] = 0;
        }
        for (int y = height - 1; y > 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                visualGrid[x, y] = visualGrid[x, y - 1];
                gridLogic[x, y] = gridLogic[x, y - 1];

                if (visualGrid[x, y] != null)
                {
                    visualGrid[x, y].position = GetWorldPosition(x, y);
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            visualGrid[x, 0] = null;
            gridLogic[x, 0] = 0;
        }
        GenerateBottomRow();
    }
    void GenerateBottomRow()
    {
        for (int x = 0; x < width; x++)
        {
            Debug.Log("Generating new row");
            visualGrid[x, 0] = null;
            gridLogic[x, 0] = 0;
        }
    }
    bool IsGameOver()
    {
        for (int x = 0; x < width; x++)
        {
            if (visualGrid[x, height - 1] != null)
                return true;
        }
        return false;
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

    public bool ClearRow(int y)
    {
        bool cleared = false;

        for (int x = 0; x < width; x++)
        {
            gridLogic[x, y] = 0;

            if (visualGrid[x, y] != null)
            {
                Destroy(visualGrid[x, y].gameObject);
                visualGrid[x, y] = null;
                cleared = true;
            }
        }

        return cleared;
    }

    public bool ClearColumn(int x)
    {
        bool cleared = false;

        for (int y = 0; y < height; y++)
        {
            gridLogic[x, y] = 0;

            if (visualGrid[x, y] != null)
            {
                Destroy(visualGrid[x, y].gameObject);
                visualGrid[x, y] = null;
                cleared = true;
            }
        }
        return cleared;
    }
}