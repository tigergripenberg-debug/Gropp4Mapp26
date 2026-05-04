using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField] private Timer time;
    [SerializeField] private SoundManager soundManager;
    
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
        if (Score.Instance != null)
        {
            Score.Instance.score = 0;
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

            GridTimerScript.Instance.resetValue();
            GridTimerScript.Instance.freeze(true);

            Debug.Log("Rad sprängd! Nästa runda är helt immun.");
        }

        else if (hasImmunity)
        {
            hasImmunity = false;
            turnsSinceClear = 0;

            GridTimerScript.Instance.freeze(false);
            GridTimerScript.Instance.resetValue();

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
            GridTimerScript.Instance.resetValue();
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

            GridTimerScript.Instance.resetValue();
            GridTimerScript.Instance.freeze(true);

            foreach (var row in rowsToClear)
                if (ClearRow(row))
                    didClear = true;

            foreach (var col in columnsToClear)
                if (ClearCol(col))
                    didClear = true;

            bool isBoardEmpty = true;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (gridLogic[x, y] == 1)
                    {
                        isBoardEmpty = false;
                        break;
                    }
                }
                if (!isBoardEmpty) break;
            }
            if (Timer.Instance != null) Timer.Instance.CalculateAndAddTime(totalLines, isBoardEmpty);
            if (Score.Instance != null) Score.Instance.CalculateAndAddScore(totalLines, isBoardEmpty);
        }
        if (Score.Instance != null) Score.Instance.RegisterTurnResult(didClear);

        return didClear;
    }

    public void TriggerGameOver()
    {
        Debug.Log("Game Over");
        MenuController.gameIsPaused = true;
        StartCoroutine(ShowGameOverRoutine());
    }
    private IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
    }

    public bool CanPlaceShapeAtPosition(Shape shape, int originX, int originY)
    {
        Vector2Int origin = shape.GetOriginCell();
        foreach (Vector2Int cell in shape.cells)
        {
            int x = originX + (cell.x - origin.x);
            int y = originY + (cell.y - origin.y);
            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;
            if (gridLogic[x, y] == 1)
                return false;
        }
        return true;
    }
    
    public void PlaceShape(ShapeBehaviour shapeBehaviour)
    {
        Shape shape = shapeBehaviour.ShapeData;
        Vector2Int origin = shape.GetOriginCell();
        Vector2Int gridPos = WorldToGrid(shapeBehaviour.transform.position);
        int i = 0;
        foreach (Vector2Int cell in shape.cells)
        {
            int x = gridPos.x + (cell.x - origin.x);
            int y = gridPos.y + (cell.y - origin.y);
            Transform child = shapeBehaviour.transform.GetChild(i);
            gridLogic[x, y] = 1;
            visualGrid[x, y] = child;
            i++;
        }
    }
    
    public void CheckIfPlayable()
    {
        //Kollar alla pusselbitar i spelet.
        ShapeBehaviour[] allBlocks = FindObjectsByType<ShapeBehaviour>(FindObjectsSortMode.None);
        bool canPlayAnything = false;
        int waitingBlocksCount = 0;

        foreach (ShapeBehaviour b in allBlocks)
        {
            //Kollar alla blocken nere i spawnen för att se om de går att spela eller inte.
            if (b.GetComponent<Collider2D>().enabled == true)
            {
                waitingBlocksCount++;
                if (CanShapeFit(b.ShapeData))
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
        if (WouldPushCauseGameOver())
        {
            TriggerGameOver();
            return;
        }
        for (int x = 0; x < width; x++)
        {
            if (visualGrid[x, 0] != null)
            {
                Destroy(visualGrid[x, 0].gameObject);
            }

            visualGrid[x, 0] = null;
            gridLogic[x, 0] = 0;
        }
        List<Task> moveTasks = new List<Task>();
        for (int y = 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                visualGrid[x, y - 1] = visualGrid[x, y];
                gridLogic[x, y - 1] = gridLogic[x, y];

                if (visualGrid[x, y - 1] != null)
                {
                    Vector3 targetPosition = GetWorldPosition(x, y - 1);
                    moveTasks.Add(AnimateBlockDown(visualGrid[x, y - 1], targetPosition));
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            visualGrid[x, height - 1] = null;
            gridLogic[x, height - 1] = 0;
        }   
        GenerateNewRow();
    }

    bool WouldPushCauseGameOver()
    {
        for (int x = 0; x < width; x++)
        {
            if (visualGrid[x, 0] != null)
            {
                return true;
            }
        }
        return false;
    }
    
    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void GenerateNewRow()
    {
        for (int x = 0; x < width; x++)
        {
            GameObject newBlock = Instantiate(tilePrefab);
            newBlock.transform.position = GetWorldPosition(x, height - 1);
            visualGrid[x, height - 1] = newBlock.transform;
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

    public bool CanShapeFit(Shape shape) 
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool fitsHere = true;
                foreach (var cell in shape.cells)
                {
                    int testX = x + Mathf.RoundToInt(cell.x);
                    int testY = y + Mathf.RoundToInt(cell.y);
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

    private bool ClearRow(int y)
    {
        StartCoroutine(ClearRowCoroutine(y));
        return true;
    }

    public static event System.Action<SFXSounds> OnBlockClearedPlayPop;

    private IEnumerator ClearRowCoroutine(int y)
    {
        List<GameObject> blocksToDestroy = new List<GameObject>();

        for (int x = 0; x < width; x++)
        {
            gridLogic[x, y] = 0;
            if (visualGrid[x, y] != null)
            {
                blocksToDestroy.Add(visualGrid[x, y].gameObject);
                visualGrid[x, y] = null;
            }
        }
        foreach (GameObject block in blocksToDestroy)
        {
            OnBlockClearedPlayPop?.Invoke(SFXSounds.pop_sound);
            Destroy(block);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ClearColCoroutine(int x)
    {
        List<GameObject> blocksToDestroy = new List<GameObject>();

        for (int y = 0; y < height; y++)
        {
            gridLogic[x, y] = 0;
            if (visualGrid[x, y] != null)
            {
                blocksToDestroy.Add(visualGrid[x, y].gameObject);
                visualGrid[x, y] = null;
            }
        }
        foreach (GameObject block in blocksToDestroy)
        {
            OnBlockClearedPlayPop?.Invoke(SFXSounds.pop_sound);
            Destroy(block);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool ClearCol(int x)
    {
        StartCoroutine(ClearColCoroutine(x));
        return true;
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
    private async Task<bool> AnimateBlockDown(Transform block, Vector3 targetPos)
    {
        float duration = 0.07f; // Seconds to animate
        float elapsed = 0f;
        Vector3 startPos = block.position;

        while (elapsed < duration)
        {
            if (block == null) return false; // Block was cleared during move

            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            // Smooth interpolation
            block.position = Vector3.Lerp(startPos, targetPos, percent);

            // Yields back to Unity for one frame
            await Task.Yield();
        }

        if (block != null) block.position = targetPos;
        return true;
    }
}