using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [Header("Settings")] 
    
    // ==========================================
    // Tiger: SPELPLANEN (2D-ARRAY) START
    // Variablerna som håller koll på det logiska och visuella 8x8-brädet.
    public int[,] gridLogic;
    public Transform[,] visualGrid;
    public int width { get; private set; } = 8;
    public int height { get; private set; } = 8;
    // ==========================================
    // Tiger: SPELPLANEN SLUT
    // ==========================================

    [SerializeField] private GameObject tilePrefab, blockPrefab;
    
    // ==== Tiger: PARTIKLAR (Referens) ====
    [SerializeField] private GameObject explosionParticlePrefab;
    
    // ==== Tiger: GRIDPUSH (Referenser) ====
    public int maxTurnsSinceClear = 0, turnsSinceClear = 0;
    public bool hasImmunity = false, linesClearedThisRound = false;
    
    [SerializeField] private Timer time;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private MenuController menuController;
    [SerializeField] private Vector2 originOffset = new Vector2(0, 2f);
    public static Transform PlacedBlockParent;
    [SerializeField] private BlockColorPalette currentPalette;
    public int clearingRoutines = 0;
    public bool isClearing => clearingRoutines > 0;
    public string Whydied { private set; get; }
    private List<Transform> previewBlocks = new List<Transform>();
    [SerializeField] private float previewPulseScale = 1.2f;
    [SerializeField] private float previewPulseDuration = 0.35f;
    public bool GameOver { get; private set; }

    void Awake()
    {
        Instance = this;
        
        // ==========================================
        // Tiger: SPELPLANEN (SKAPA ARRAYER) START
        // Här initierar vi arrayerna baserat på width och height (8x8) 
        // när spelet startar.
        gridLogic = new int[width, height];
        visualGrid = new Transform[width, height];
        // ==========================================
        // Tiger: SPELPLANEN SLUT
        // ==========================================
        
        if (PlacedBlockParent == null)
        {
            var go = new GameObject("PlacedBlocks");
            PlacedBlockParent = go.transform;
        }
    }

    void Start()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
        GenerateGrid(); // Tiger: Genererar det visuella brädet
        AdjustCameraToScreen();
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

    public List<Vector2Int> GetPreviewClears(Shape shape, Vector2Int gridPos)
    {
        List<Vector2Int> cellsToClear = new List<Vector2Int>();
        bool[,] simulated = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                simulated[x, y] = gridLogic[x, y] == 1;
            }
        }
        Vector2Int origin = shape.GetOriginCell();

        foreach (Vector2Int cell in shape.cells)
        {
            int x = gridPos.x + (cell.x - origin.x);
            int y = gridPos.y + (cell.y - origin.y);

            if (!IsInsideGrid(x, y))
                return cellsToClear;

            simulated[x, y] = true;
        }
        for (int y = 0; y < height; y++)
        {
            bool full = true;

            for (int x = 0; x < width; x++)
            {
                if (!simulated[x, y])
                {
                    full = false;
                    break;
                }
            }
            if (full)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!cellsToClear.Contains(pos))
                        cellsToClear.Add(pos);
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            bool full = true;

            for (int y = 0; y < height; y++)
            {
                if (!simulated[x, y])
                {
                    full = false;
                    break;
                }
            }
            if (full)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!cellsToClear.Contains(pos))
                        cellsToClear.Add(pos);
                }
            }
        }
        return cellsToClear;
    }

    public void ClearPreviewEffects()
    {
        foreach (Transform block in previewBlocks)
        {
            if (block == null)
                continue;

            block.DOKill();

            block.DOScale(1f, 0.1f);
        }

        previewBlocks.Clear();
    }

    public void ShowClearPreview(List<Vector2Int> clears)
    {
        ClearPreviewEffects();

        foreach (Vector2Int pos in clears)
        {
            Transform block = visualGrid[pos.x, pos.y];

            if (block == null)
                continue;

            if (previewBlocks.Contains(block))
                continue;

            previewBlocks.Add(block);

            block.DOKill();

            block.DOScale(previewPulseScale, previewPulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
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
                    Debug.Log($"{occupied / (width * height) * 100} %");
                }
            }
        }
        return (float)occupied / total;
    }

    // ==========================================
    // Tiger: SPELPLANEN (POSITIONERING) START
    // Dessa två funktioner konverterar mellan grid-koordinater (x,y) och världens koordinater (Vector2).
    // Samt justerar offset så att det ska vara skön position av spelplanen för spelaren.
    public Vector2 GetWorldPosition(int x, int y)
    {
        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 0) / 2f;

        return new Vector2(
            x - xOffset,
            y - yOffset
        ) + originOffset;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 0) / 2f;
        Vector2 adjusted = (Vector2)worldPos - originOffset;
        int x = Mathf.RoundToInt(adjusted.x + xOffset);
        int y = Mathf.RoundToInt(adjusted.y + yOffset);

        return new Vector2Int(x, y);
    }
    // ==========================================
    // Tiger: SPELPLANEN (POSITIONERING) SLUT
    // ==========================================

    public void RestartGame() 
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
        menuController.ReloadActiveScene();
    }
    
    public void ResetGameOver()
    {
        GameOver = false;
    }

    // ==========================================
    // Tiger: SPELPLANEN (GENERERING) START
    // Skapar alla visuella rutor när spelet startar.
    // Inkluderar även koden för Varningszonen som jag inte har gjort.
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

                // Skapar den vita faden och klipper den via SpriteMask
                if (y == 0)
                {
                    SpriteRenderer baseTileSR = newTile.transform.GetChild(0).GetComponent<SpriteRenderer>();

                    Texture2D fadeTex = new Texture2D(1, 2);
                    fadeTex.wrapMode = TextureWrapMode.Clamp;
                    fadeTex.filterMode = FilterMode.Bilinear;
                    fadeTex.SetPixel(0, 0, new Color(1f, 1f, 1f, 1f));
                    fadeTex.SetPixel(0, 1, new Color(1f, 1f, 1f, 0f));
                    fadeTex.Apply();
                    Sprite fadeSprite = Sprite.Create(fadeTex, new Rect(0, 0, 1, 2), new Vector2(0.5f, 0.5f), 1f);

                    GameObject fadeOverlay = new GameObject("FadeOverlay");
                    fadeOverlay.transform.SetParent(baseTileSR.transform);
                    fadeOverlay.transform.localPosition = new Vector3(0, 0, -0.01f);
                    
                    float scaleX = baseTileSR.sprite.bounds.size.x / fadeSprite.bounds.size.x;
                    float scaleY = baseTileSR.sprite.bounds.size.y / fadeSprite.bounds.size.y;
                    fadeOverlay.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                    SpriteRenderer sr = fadeOverlay.AddComponent<SpriteRenderer>();
                    sr.sprite = fadeSprite;
                    sr.color = Color.red; 
                    sr.sortingLayerName = baseTileSR.sortingLayerName;
                    sr.sortingOrder = baseTileSR.sortingOrder + 1;

                    SpriteMask mask = baseTileSR.gameObject.AddComponent<SpriteMask>();
                    mask.sprite = baseTileSR.sprite;
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                    UnityEngine.Rendering.SortingGroup sg = newTile.AddComponent<UnityEngine.Rendering.SortingGroup>();
                    sg.sortingLayerName = baseTileSR.sortingLayerName;
                }
            }
        }
    }
    // ==========================================
    // Tiger: SPELPLANEN (GENERERING) SLUT
    // ==========================================

    // ==========================================
    // Tiger: GRIDPUSH LOGIK START
    // Håller koll på hur många turer spelaren gjort utan att
    // spränga linjer, och flyttar ner brädet om det gått för lång tid.
    public void OnTurnFinished()
    {
        if (GridTimerScript.Instance.getFrozenStatus()) return;
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

        // Om gränsen nås, aktivera MoveGrid (GridPush)
        if (turnsSinceClear > maxTurnsSinceClear)
        {
            MoveGrid();
            turnsSinceClear = 0;
            GridTimerScript.Instance.resetValue();
        }
    }
    // ==========================================
    // Tiger: GRIDPUSH LOGIK SLUT
    // ==========================================

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
        // Handlar om GridPush-logiken: Om vi spränger linjer, nollställ timer och ge immunitet.
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
        GameOver = true;
        Debug.Log("Game Over");
        MenuController.gameIsPaused = true;
        StartCoroutine(ShowGameOverRoutine());
    }
    
    private IEnumerator ShowGameOverRoutine()
    {
        FillBoardAtGameOver();
        yield return new WaitForSeconds(0.5f);
        if (menuController != null)
        {
            OnGameOverPlayPop?.Invoke(SFXSounds.pop_sound);

            menuController.ShowGameOverPanel();
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
        Whydied = "No more space to place blocks";
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

    // ==========================================
    // Tiger: GRIDPUSH RÖRELSE START
    // MoveGrid tar bort bottenraden och flyttar ner 
    // alla befintliga block ett steg i arrayen och i världen.
    public void MoveGrid()
    {
        //Inte jag
        if (IsGameOver())
        {
            Whydied = "Blocks moved beyond the bottom";
            TriggerGameOver();
            return;
        }

        // Förstör rad y=0 (längst ner)
        for (int x = 0; x < width; x++)
        {
            if (visualGrid[x, 0] != null)
            {
                Destroy(visualGrid[x, 0].gameObject);
            }
            visualGrid[x, 0] = null;
            gridLogic[x, 0] = 0;
        }
        
        // Flytta ner allt annat
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                visualGrid[x, y] = visualGrid[x, y + 1];
                gridLogic[x, y] = gridLogic[x, y + 1];

                if (visualGrid[x, y] != null)
                {
                    visualGrid[x, y].transform.DOMove(GetWorldPosition(x, y), 1.5f).SetEase(Ease.InOutElastic);
                    OnGridMovedPlayPop?.Invoke(SFXSounds.pop_sound);
                }
            }
        }
        
        // Rensa gamla topp-raden
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
    // ==========================================
    // Tiger: GRIDPUSH RÖRELSE SLUT
    // ==========================================

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
        clearingRoutines++;
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
            
            // Tiger: Partiklar triggas här!
            SpawnParticles(block); 
            
            yield return new WaitForSeconds(0.05f);
        }
        clearingRoutines--;
    }

    private IEnumerator ClearColCoroutine(int x)
    {
        clearingRoutines++;
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
            
            // Tiger: Partiklar triggas här!
            SpawnParticles(block); 
            
            yield return new WaitForSeconds(0.05f);
        }
        clearingRoutines--;
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
    
    public void SpawnPlacedBlock(int x, int y, int colorIndex)
    {
        Vector3 worldPos = GetWorldPosition(x, y);
        var block = Instantiate(blockPrefab, worldPos, Quaternion.identity, PlacedBlockParent);
        var sr = block.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "PlacedBlocks";
        sr.color = currentPalette.colors[colorIndex];
        block.GetComponent<NewBlock>().colorIndex = colorIndex;
        visualGrid[x, y] = block.transform;
    }
    
    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    
    public void RefreshBlockColors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Transform block = visualGrid[x, y];
                if (block == null)
                    continue;
                NewBlock nb = block.GetComponent<NewBlock>();
                if (nb == null)
                    continue;
                SpriteRenderer sr =
                    block.GetComponent<SpriteRenderer>();
                sr.color =
                    currentPalette.colors[nb.colorIndex];
            }
        }
    }
    
    public void SetPalette(BlockColorPalette newPalette)
    {
        currentPalette = newPalette;
    }
    
    public void AdjustCameraToScreen()
    {
        float targetWidth = width + 2f;

        float aspectRatio = Screen.width / (float)Screen.height;

        float requiredCameraSize = targetWidth / 2f / aspectRatio;

        Camera.main.orthographicSize = Screen.width > Screen.height ? requiredCameraSize * 2 : requiredCameraSize;

        Camera.main.transform.position = new Vector3(0, 1f, -10f);
    }
    
    // ==========================================
    // Tiger: PARTIKLAR (INSTANSIERING) START
    // Läser av färgen på blocket som precis sprängdes och 
    // skapar en explosion med exakt samma färg.
    private void SpawnParticles(Transform block)
    {
        if (explosionParticlePrefab == null) return;

        // Skapa partikeln på blockets position
        GameObject particles = Instantiate(explosionParticlePrefab, block.position, Quaternion.identity);

        // Hämta färgen från blocket och ge den till partikeln
        SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            ParticleSystem ps = particles.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = sr.color;
            }
        }
    }
    // ==========================================
    // Tiger: PARTIKLAR (INSTANSIERING) SLUT
    // ==========================================
}