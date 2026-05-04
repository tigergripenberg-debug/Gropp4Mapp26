using UnityEngine;
using Random = UnityEngine.Random;

public class ShapeBehaviour : MonoBehaviour
{
    Vector2Int grabOffset, currentGridPosition;
    private Vector3 dragOffset = new Vector3(0f,1f,0f);
    private Vector3 startPosition;
    private Vector3 previewScale = new Vector3(0.6f, 0.6f, 1f);
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private SpriteRenderer[] childSR;
    private Color shapeColor;
    public Color[] possibleColors; // colors are set inside of blockspawner
    public Shape ShapeData { get; private set;  }
    private GameObject ghost;
    [SerializeField] GameObject blockPrefab;
    public static event System.Action<SFXSounds> OnBlockPlacement;
    public void Initialize(Shape shape, Color[] colors, GameObject prefab)
    {
        blockPrefab = prefab;
        ShapeData = shape;
        possibleColors = colors;
        childSR = GetComponentsInChildren<SpriteRenderer>();
        SetRandomColor();
        SetAsActive();
        startPosition = transform.position;
        transform.localScale = previewScale;
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
        col.size = size + new Vector2(2f,2f); // small padding (important)
        col.offset = (min + max) / 2f;
    }
    private void SetRandomColor()
    {
        shapeColor = possibleColors[Random.Range(0, possibleColors.Length)];
        foreach (SpriteRenderer sr in childSR)
        {
            sr.color = shapeColor;
        }
    }

    private void OnMouseDown()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2Int mouseGrid = GridManager.Instance.WorldToGrid(mouseWorld);
        Vector2Int shapeGrid = GridManager.Instance.WorldToGrid(transform.position);
        grabOffset = shapeGrid - mouseGrid;
        transform.localScale = normalScale;
        CreateGhost();
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
            sr.color = new Color(shapeColor.r,shapeColor.g,shapeColor.b,0.5f);
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
    
    private void OnMouseDrag()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + dragOffset;
        Vector3 pieceWorld = transform.position;
        currentGridPosition = GridManager.Instance.WorldToGrid(pieceWorld);
        UpdateGhost();
    }

    private void UpdateGhost()
    {
        Vector2 world = GridManager.Instance.GetWorldPosition(
            currentGridPosition.x,
            currentGridPosition.y
        );
        ghost.transform.position = new Vector3(world.x, world.y, 0f);
        bool valid = GridManager.Instance.CanPlaceShapeAtPosition(
            ShapeData,
            currentGridPosition.x,
            currentGridPosition.y
        );
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
    void OnMouseUp()
    {
        if (ghost != null)
        {
            Destroy(ghost);
        }
        bool valid = GridManager.Instance.CanPlaceShapeAtPosition(
            ShapeData,
            currentGridPosition.x,
            currentGridPosition.y
        );
        if (valid)
        {
            Vector2 world = GridManager.Instance.GetWorldPosition(
                currentGridPosition.x,
                currentGridPosition.y
            );
            transform.position = new Vector3(world.x, world.y, 0f);
            GridManager.Instance.PlaceShape(this);
            GetComponent<Collider2D>().enabled = false;
            GridManager.Instance.CheckForMatches();
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
    }
}