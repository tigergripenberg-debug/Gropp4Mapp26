using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [Header("Settings")]
    public int[,] gridLogic;
    public Transform[,] visualGrid;
    [SerializeField] private GameObject tilePrefab, gameOverCanvas;
    private int width = 8, height = 8;
    private int maxTurnsSinceClear = 0, turnsSinceClear = 0;
    private bool hasImmunity = false, linesClearedThisRound = false;
    [SerializeField] private Score score;
    [SerializeField] private timer time;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private gridtimerscript gridtimerscript;

    void Awake()
    {
        Instance = this;
        gridLogic = new int[width, height];
        visualGrid = new Transform[width, height];
    }

    void Start()
    {
        GenerateGrid();
        AdjustCameraToScreen();
    }

    void OnDrawGizmos() //används för att rita upp grid i debugmode
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
        if (score != null)
        {
            score.score = 0;
        }
        else
        {
            time.time = 100f;
        }
        MenuController.gameIsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                GameObject newTile = Instantiate(tilePrefab);
                newTile.transform.position = GetWorldPosition(x, y);
                newTile.name = $"Tile X:{x} Y:{y}";
                newTile.transform.SetParent(transform);
            }
        }
    }

    public void OnTurnFinished()
    {
        if (linesClearedThisRound)
        {
            hasImmunity = true;
            turnsSinceClear = 0;

            gridtimerscript.instance.resetValue();
            gridtimerscript.instance.freeze(true);

            Debug.Log("Rad sprängd! Nästa runda är helt immun.");
        }

        else if (hasImmunity)
        {
            hasImmunity = false;
            turnsSinceClear = 0;

            gridtimerscript.instance.freeze(false);
            gridtimerscript.instance.resetValue();
            
            Debug.Log("Immun runda! Brädet rör sig inte. Nästa runda är vi sårbara igen.");
        }

        else
        {
            turnsSinceClear++;
        }

        linesClearedThisRound = false;

        Debug.Log("Turns since clear: " + turnsSinceClear);

        if (turnsSinceClear > maxTurnsSinceClear)
        {
            Debug.Log("GRID PUSH!");
            MoveGrid();
            turnsSinceClear = 0;
            gridtimerscript.instance.resetValue();
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
            linesClearedThisRound = true;

            foreach (var row in rowsToClear)
                if (ClearRow(row))
                    didClear = true;

            foreach (var col in columnsToClear)
                if (ClearColumn(col))
                    didClear = true;
            if (score != null)
            {
                score.AddScore(totalLines);
            }
            else
            {
                time.AddScore(totalLines);
            }
        }

        return didClear;
    }

    public void TriggerGameOver()
    {
        Debug.Log("Game Over");
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            MenuController.gameIsPaused = true;
        }
    }

    public void CheckIfPlayable()
    {
        //Kollar alla pusselbitar i spelet.
        Block[] allBlocks = FindObjectsByType<Block>(FindObjectsSortMode.None);
        bool canPlayAnything = false;
        int waitingBlocksCount = 0;

        foreach (Block b in allBlocks)
        {
            //Kollar alla blocken nere i spawnen för att se om de går att spela eller inte.
            if (b.GetComponent<Collider2D>().enabled == true)
            {
                waitingBlocksCount++;
                if (CanBlockFit(b.gameObject))
                {
                    canPlayAnything = true;
                    break;
                }
            }
        }
        //Om det finns block kvar eller inte men de inte går att spela så triggas game over.
        if (waitingBlocksCount > 0 && canPlayAnything == false)
        {
            TriggerGameOver();
        }
    }

    public void MoveGrid()
    {
        if (IsGameOver())
        {
            TriggerGameOver();
            return;
        }

        // Destroy bottom row (y = 0)
        for (int x = 0; x < width; x++)
        {
            if (visualGrid[x, 0] != null)
            {
                Destroy(visualGrid[x, 0].gameObject);
            }

            visualGrid[x, 0] = null;
            gridLogic[x, 0] = 0;
        }

        // Move everything DOWN
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                visualGrid[x, y] = visualGrid[x, y + 1];
                gridLogic[x, y] = gridLogic[x, y + 1];

                if (visualGrid[x, y] != null)
                {
                    visualGrid[x, y].position = GetWorldPosition(x, y);
                }
            }
        }

        // Clear TOP row (now empty)
        for (int x = 0; x < width; x++)
        {
            visualGrid[x, height - 1] = null;
            gridLogic[x, height - 1] = 0;
        }

        GenerateNewRow();
    }

    void GenerateNewRow()
    {
        for (int x = 0; x < width; x++)
        {
            Debug.Log("Generating new row");
            visualGrid[x, height - 1] = null;
            gridLogic[x, height - 1] = 0;
        }
    }
    bool IsGameOver()
    {
        for (int x = 0; x < width; x++)
        {
            if (visualGrid[x, 0] != null)
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
    public void AdjustCameraToScreen()
    {
        //Lägger till 2 rutor i marginal på varje sida av griden.
        float targetWidth = width + 2f;

        //Räknar ut mobilens aspect ratio (bredd/höjd).
        float aspectRatio = Screen.width / (float)Screen.height;

        //Räknar ut vilken ortografisk storlek kameran behöver ha för att visa hela griden i bredd.
        float requiredCameraSize = targetWidth / 2f / aspectRatio;

        Camera.main.orthographicSize = requiredCameraSize;

        // Sätter kamerans position så att den är centrerad på griden.
        Camera.main.transform.position = new Vector3(0, 1f, -10f);
    }
}