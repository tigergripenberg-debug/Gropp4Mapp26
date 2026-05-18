using UnityEngine;
using Random = UnityEngine.Random;

public class ShapeBehaviour : MonoBehaviour
{
    Vector2Int grabOffset, currentGridPosition;
    private Vector3 dragOffset = new Vector3(0f, 1f, 0f);
    private Vector3 clickOffset; 
    private Vector3 startPosition;
    private Vector3 previewScale = new Vector3(0.6f, 0.6f, 1f);
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private SpriteRenderer[] childSR;
    private Color shapeColor;
    public int colorIndex;
    public Color[] possibleColors; 
    public Shape ShapeData { get; private set; }
    private GameObject ghost;
    [SerializeField] GameObject blockPrefab;
    public static event System.Action<SFXSounds> OnBlockPlacement;

    public void Initialize(Shape shape, Color[] colors, GameObject prefab)
    {
        blockPrefab = prefab;
        ShapeData = shape;
        possibleColors = colors;
        BuildShape();
        childSR = GetComponentsInChildren<SpriteRenderer>();
        SetRandomColor();
        SetAsActive();
        startPosition = transform.position;
        transform.localScale = previewScale;
    }

    private void BuildShape()
    {
        Vector2Int origin = ShapeData.GetOriginCell();
        foreach (var cell in ShapeData.cells)
        {
            GameObject block = Instantiate(blockPrefab, transform);
            block.transform.localPosition = new Vector3(
                cell.x - origin.x,
                cell.y - origin.y,
                0f
            );
        }
    }

    public void FitColliderToShape()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();
        Vector2 min = Vector2.positiveInfinity;
        Vector2 max = Vector2.negativeInfinity;
        foreach (Transform child in transform)
        {
            Vector3 pos = child.localPosition;
            if (pos.x < min.x) min.x = pos.x;
            if (pos.y < min.y) min.y = pos.y;
            if (pos.x > max.x) max.x = pos.x;
            if (pos.y > max.y) max.y = pos.y;
        }
        Vector2 size = max - min;
        col.size = size + new Vector2(2f, 2f); 
        col.offset = (min + max) / 2f;
    }

    private void SetRandomColor()
    {
        colorIndex = Random.Range(0, possibleColors.Length);
        shapeColor = possibleColors[colorIndex];
        foreach (SpriteRenderer sr in childSR)
        {
            sr.color = shapeColor;
            NewBlock nv = sr.GetComponent<NewBlock>();
            if (nv != null)
            {
                nv.colorIndex = colorIndex;
            }
        }
    }

    private void CreateGhost()
    {
        Vector2Int origin = ShapeData.GetOriginCell();
        ghost = new GameObject("Ghost");
        foreach (var cell in ShapeData.cells)
        {
            GameObject block = Instantiate(blockPrefab, ghost.transform);
            block.transform.localPosition = new Vector3(cell.x - origin.x, cell.y - origin.y, 0f);
            var sr = block.GetComponent<SpriteRenderer>();
            sr.color = new Color(shapeColor.r, shapeColor.g, shapeColor.b, 0f);
            sr.sortingLayerName = "Ghost";
        }
    }

    private void UpdateGhostVisibility()
    {
        Vector2Int origin = ShapeData.GetOriginCell();
        int i = 0;
        foreach (Vector2Int cell in ShapeData.cells)
        {
            Transform block = ghost.transform.GetChild(i);
            int x = currentGridPosition.x + (cell.x - origin.x);
            int y = currentGridPosition.y + (cell.y - origin.y);
            bool insideGrid = GridManager.Instance.IsInsideGrid(x, y);
            block.gameObject.SetActive(insideGrid);
            i++;
        }
    }

    private void UpdateGhost()
    {
        Vector2 world = GridManager.Instance.GetWorldPosition(
            currentGridPosition.x,
            currentGridPosition.y
        );
        ghost.transform.position = new Vector3(world.x, world.y, 0f);
        bool valid = GridManager.Instance.CanPlaceShapeAtPosition(ShapeData, currentGridPosition);
        SetGhostColor(valid);
        UpdateGhostVisibility();
    }

    private void SetGhostColor(bool valid)
    {
        foreach (Transform child in ghost.transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            Color c = sr.color;
            c.a = valid ? 0.5f : 0f;
            sr.color = c;
        }
    }
  private Vector2Int GetSmartGridPosition(Vector2Int rawGridPos)
    {
        int w = GridManager.Instance.width;
        int h = GridManager.Instance.height;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (mouseWorld.y < -2.5f || mouseWorld.y > 5f || mouseWorld.x < -4.5f || mouseWorld.x > 4.5f)
        {
            return rawGridPos; 
        }
      
        Vector2Int smartPos = rawGridPos;
        Vector2Int origin = ShapeData.GetOriginCell();

        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var cell in ShapeData.cells)
        {
            int localX = cell.x - origin.x;
            int localY = cell.y - origin.y;
            if (localX < minX) minX = localX;
            if (localX > maxX) maxX = localX;
            if (localY < minY) minY = localY;
            if (localY > maxY) maxY = localY;
        }

        if (smartPos.x + minX < 0) smartPos.x = -minX;
        if (smartPos.x + maxX >= w) smartPos.x = (w - 1) - maxX;
        
        if (smartPos.y + minY < 0) smartPos.y = -minY;
        if (smartPos.y + maxY >= h) smartPos.y = (h - 1) - maxY;

        return smartPos;
    }
    private Vector2Int GetMagneticGridPosition(Vector2Int rawGridPos)
    {
        Vector2Int smartPos = GetSmartGridPosition(rawGridPos);

        if (GridManager.Instance.CanPlaceShapeAtPosition(ShapeData, smartPos))
        {
            return smartPos;
        }

        Vector2Int bestFit = smartPos;
        float shortestDistance = float.MaxValue;
        bool foundAlternative = false;

        Vector2Int[] searchOffsets = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(0, -1), 
            new Vector2Int(1, 0), new Vector2Int(-1, 0), 
            new Vector2Int(1, 1), new Vector2Int(-1, -1), 
            new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };

        foreach (Vector2Int offset in searchOffsets)
        {
            Vector2Int testPos = smartPos + offset;
            testPos = GetSmartGridPosition(testPos);

            if (GridManager.Instance.CanPlaceShapeAtPosition(ShapeData, testPos))
            {
                float dist = Vector2Int.Distance(smartPos, testPos);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    bestFit = testPos;
                    foundAlternative = true;
                }
            }
        }

        if (foundAlternative)
        {
            return bestFit;
        }

        return smartPos;
    }
    private void OnMouseDown()
    {
        transform.localScale = normalScale;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2 min = Vector2.positiveInfinity;
        Vector2 max = Vector2.negativeInfinity;
        
        foreach (Transform child in transform)
        {
            if (child.name == "Ghost") continue; 
            Vector3 pos = child.localPosition;
            if (pos.x < min.x) min.x = pos.x;
            if (pos.y < min.y) min.y = pos.y;
            if (pos.x > max.x) max.x = pos.x;
            if (pos.y > max.y) max.y = pos.y;
        }

        float centerX = (min.x + max.x) / 2f;
        float bottomY = min.y; 
        clickOffset = new Vector3(-centerX, -bottomY, 0f); 

        transform.position = mouseWorld + clickOffset + dragOffset;

        CreateGhost();
        
        Vector3 pieceWorld = transform.position; 
        Vector2Int rawPos = GridManager.Instance.WorldToGrid(pieceWorld);
        
        currentGridPosition = GetMagneticGridPosition(rawPos);
        
        UpdateGhost();
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        
        transform.position = mouseWorld + clickOffset + dragOffset;
        
        Vector3 pieceWorld = transform.position; 
        Vector2Int rawPos = GridManager.Instance.WorldToGrid(pieceWorld);
        
        // Snäpp det magnetiskt mot andra block och kanter
        currentGridPosition = GetMagneticGridPosition(rawPos);
        UpdateGhost();
    }

    void OnMouseUp()
    {
        if (ghost != null)
        {
            Destroy(ghost);
        }
        
        bool valid = GridManager.Instance.CanPlaceShapeAtPosition(ShapeData, currentGridPosition);
        
        if (valid)
        {
            Vector2 world = GridManager.Instance.GetWorldPosition(currentGridPosition.x, currentGridPosition.y);
            transform.position = new Vector3(world.x, world.y, 0f);
            
            GridManager.Instance.PlaceShape(this);
            GetComponent<Collider2D>().enabled = false;
            
            bool didClear = GridManager.Instance.CheckForMatches();
            if (didClear)
            {
                GridManager.Instance.linesClearedThisRound = true;
            }
            
            BlockSpawner.Instance.BlockPlaced();
            SetAsPlaced();
            OnBlockPlacement?.Invoke(SFXSounds.placement_sound);
        }
        else
        {
            transform.position = startPosition;
            transform.localScale = previewScale;
            SetAsActive();
        }
    }

    private void SetAsActive()
    {
        foreach (var sr in childSR) sr.sortingLayerName = "Blocks";
    }
    
    private void SetAsPlaced()
    {
        foreach (var sr in childSR) sr.sortingLayerName = "PlacedBlocks";
        transform.SetParent(GridManager.PlacedBlockParent, true);
    }
}