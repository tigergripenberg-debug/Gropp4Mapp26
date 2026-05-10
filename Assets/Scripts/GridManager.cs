using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [Header("Settings")] public int[,] gridLogic;
    public Transform[,] visualGrid;
    [SerializeField] private GameObject tilePrefab, gameOverCanvas, blockPrefab;
    public int width { get; private set; } = 8;
    public int height { get; private set; } = 8;
    public int maxTurnsSinceClear = 0, turnsSinceClear = 0;
    public bool hasImmunity = false, linesClearedThisRound = false;
    [SerializeField] private Timer time;
    [SerializeField] private SoundManager soundManager;
    public static Transform PlacedBlockParent;

    void Awake()
    {
        Instance = this;
        gridLogic = new int[width, height];
        visualGrid = new Transform[width, height];
        if (PlacedBlockParent == null)
        {
            var go = new GameObject("PlacedBlocks");
            PlacedBlockParent = go.transform;
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
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

    public void ClearExistingBoardVisuals()
    {
        if (PlacedBlockParent == null)
        {
            return;
        }

        foreach (Transform child in PlacedBlockParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void ResetGridLogic()
    {
        gridLogic = new int[width, height];
    }

    public float GetBoardFillPercentage()
    {
        int occupied = 0;
        int total = width * height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gridLogic[x, y] == 1)
                {
                    occupied++;
                    Debug.Log($"{occupied / (width*height) * 100} %");
                }
            }
        }
        return (float)occupied / total;
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 0) / 2f;

        return new Vector2(
            x - xOffset,
            y - yOffset
        );
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 0) / 2f;

        int x = Mathf.RoundToInt(worldPos.x + xOffset);
        int y = Mathf.RoundToInt(worldPos.y + yOffset);

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
        GameManager.Instance.DeleteSave();
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
            linesClearedThisRound = false;
            turnsSinceClear = 0;

            GridTimerScript.Instance.resetValue();
        }
        else if (hasImmunity)
        {
            hasImmunity = false;
            turnsSinceClear = 0;

            GridTimerScript.Instance.resetValue();
            GridTimerScript.Instance.freeze(false);
            Debug.Log("Runda klar: Immuniteten har löpt ut.");
        }
        else
        {
            turnsSinceClear++;
        }

        if (turnsSinceClear > maxTurnsSinceClear)
        {
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
            GridTimerScript.Instance.SetColorCyan();

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

        return didClear;
    }

    private void FillBoardAtGameOver()
    {
        if (!IsGameOver()) return;
        StartCoroutine(FillBoardRoutine());
    }

    private IEnumerator FillBoardRoutine()
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (visualGrid[x, y] == null)
                {
                    Vector3 position = GetWorldPosition(x, y);
                    var tile = Instantiate(blockPrefab, position, Quaternion.identity);
                    var sr = tile.GetComponent<SpriteRenderer>();
                    sr.color = Color.gray6;
                    visualGrid[x, y] = tile.transform;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public static event System.Action<SFXSounds> OnGameOverPlayPop;

    public void TriggerGameOver()
    {
        Debug.Log("Game Over");
        MenuController.gameIsPaused = true;
        StartCoroutine(ShowGameOverRoutine());
    }
    private IEnumerator ShowGameOverRoutine()
    {
        FillBoardAtGameOver();
        yield return new WaitForSeconds(0.5f);
        if (gameOverCanvas != null)
        {
            OnGameOverPlayPop?.Invoke(SFXSounds.pop_sound);
            gameOverCanvas.SetActive(true);
        }
    }

    public void CheckIfPlayable()
    {
        ShapeBehaviour[] allBlocks = FindObjectsByType<ShapeBehaviour>(FindObjectsSortMode.None);
        foreach (ShapeBehaviour b in allBlocks)
        {
            if (!b.GetComponent<Collider2D>().enabled)
                continue;
            if (CanFitAnywhere(b.ShapeData))
                return;
        }
        TriggerGameOver();
    }

    public bool CanFitAnywhere(Shape shape)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CanPlaceShapeAtPosition(shape, new Vector2Int(x, y)))
                    return true;
            }
        }
        return false;
    }

    public static event System.Action<SFXSounds> OnGridMovedPlayPop;


    public void MoveGrid()
    {
        if (IsGameOver())
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
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                visualGrid[x, y] = visualGrid[x, y + 1];
                gridLogic[x, y] = gridLogic[x, y + 1];

                if (visualGrid[x, y] != null)
                {
                    visualGrid[x, y].transform.DOMove(GetWorldPosition(x, y), 1f).SetEase(Ease.InOutElastic);
                    OnGridMovedPlayPop?.Invoke(SFXSounds.pop_sound);

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

    public bool CanBlockFit(Shape shape)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool fitsHere = true;
                foreach (Vector2Int cell in shape.cells)
                {
                    int testX = x + cell.x;
                    int testY = y + cell.y;
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
        List<Transform> blocksToDestroy = new List<Transform>();

        for (int x = 0; x < width; x++)
        {
            if (visualGrid[x, y] != null)
            {
                blocksToDestroy.Add(visualGrid[x, y]);
                visualGrid[x, y] = null;
            }
            gridLogic[x, y] = 0;
        }
        foreach (Transform block in blocksToDestroy)
        {
            OnBlockClearedPlayPop?.Invoke(SFXSounds.pop_sound);
            Destroy(block.gameObject);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ClearColCoroutine(int x)
    {
        List<Transform> blocksToDestroy = new List<Transform>();

        for (int y = 0; y < height; y++)
        {
            if (visualGrid[x, y] != null)
            {
                blocksToDestroy.Add(visualGrid[x, y]);
                visualGrid[x, y] = null;
            }
            gridLogic[x, y] = 0;
        }
        foreach (Transform block in blocksToDestroy)
        {
            OnBlockClearedPlayPop?.Invoke(SFXSounds.pop_sound);
            Destroy(block.gameObject);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public bool CanPlaceShapeAtPosition(Shape shape, Vector2Int gridPos)
    {
        Vector2Int origin = shape.GetOriginCell();
        foreach (Vector2Int cell in shape.cells)
        {
            int x = gridPos.x + (cell.x - origin.x);
            int y = gridPos.y + (cell.y - origin.y);
            if (!IsInsideGrid(x, y))
                return false;
            if (gridLogic[x, y] == 1)
                return false;
        }
        return true;
    }

    public void PlaceShape(ShapeBehaviour shapeBehaviour)
    {
        if (shapeBehaviour.transform.childCount == 0)
        {
            Debug.LogError("Shape has no children! Not built yet.");
            return;
        }
        Shape shape = shapeBehaviour.ShapeData;
        Vector2Int gridPos = WorldToGrid(shapeBehaviour.transform.position);
        Vector2Int origin = shape.GetOriginCell();
        int i = 0;
        foreach (Vector2Int cell in shape.cells)
        {
            int x = gridPos.x + (cell.x - origin.x);
            int y = gridPos.y + (cell.y - origin.y);
            if (!IsInsideGrid(x, y))
                continue;
            if (gridLogic[x, y] == 1)
                continue;
            gridLogic[x, y] = 1;
            Transform block = shapeBehaviour.transform.GetChild(i);
            block.SetParent(PlacedBlockParent);
            block.position = GetWorldPosition(x, y);
            visualGrid[x, y] = block;
            i++;
        }

        BlockSpawner.Instance.currentShapes.Remove(shape);
        Destroy(shapeBehaviour.gameObject);
    }

    private bool ClearCol(int x)
    {
        StartCoroutine(ClearColCoroutine(x));
        return true;
    }
    public void SpawnPlacedBlock(int x, int y)
    {
        Vector3 worldPos = GetWorldPosition(x, y);
        var block = Instantiate(blockPrefab, worldPos, Quaternion.identity, PlacedBlockParent);
        block.GetComponent<SpriteRenderer>().sortingLayerName = "PlacedBlocks";
        visualGrid[x,y] = block.transform;
    }
    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
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