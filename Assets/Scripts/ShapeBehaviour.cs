using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;
public class ShapeBehaviour : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 startPosition;
    private Vector3 previewScale = new Vector3(0.6f, 0.6f, 1f);
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private SpriteRenderer[] childSR;
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
        var blockColor = possibleColors[Random.Range(0, possibleColors.Length)];
        foreach (SpriteRenderer sr in childSR)
        {
            sr.color = blockColor;
        }
    }
    private Vector3 GetSnappedPosition()
    {
        Transform reference = transform.GetChild(0);
        Vector2Int gridPos = GridManager.Instance.WorldToGrid(reference.position);
        Vector2 snappedWorld = GridManager.Instance.GetWorldPosition(gridPos.x, gridPos.y);
        Vector3 offset = transform.position - reference.position;
        return new Vector3(snappedWorld.x, snappedWorld.y, 0) + offset;
    }
    private void OnMouseDown()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        offset = transform.position - mouseWorld;
        transform.localScale = normalScale;
        CreateGhost();
    }

    private void CreateGhost()
    {
        Vector2 center = ShapeData.GetCenter();
        ghost = new GameObject("Ghost");
        foreach (var cell in ShapeData.cells)
        {
            GameObject block = Instantiate(blockPrefab, ghost.transform);
            block.transform.localPosition = new Vector3(cell.x - center.x, cell.y - center.y, 0f);
            var sr = block.GetComponent<SpriteRenderer>();
            sr.color = new Color(1f, 1f, 1f, 0.5f);
            sr.sortingLayerName = "Ghost";
        }
        
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + offset;
        UpdateGhost();
    }

    private void UpdateGhost()
    {
        Vector3 snappedPos = GetSnappedPosition();
        Vector2Int origin = GetGridOriginFromSnappedPosition(snappedPos);
        ghost.transform.position = snappedPos;
        bool valid = GridManager.Instance.CanPlaceShapeAtPosition(ShapeData,origin.x, origin.y);
        SetGhostColor(valid);
    }
    
    Vector2Int GetGridOriginFromSnappedPosition(Vector3 snappedPos)
    {
        Vector2 center = ShapeData.GetCenter();
        Vector2Int gridPos = GridManager.Instance.WorldToGrid(snappedPos);
        int originX = gridPos.x - Mathf.FloorToInt(center.x);
        int originY = gridPos.y - Mathf.FloorToInt(center.y);
        return new Vector2Int(originX, originY);
    }

    private void SetGhostColor(bool valid)
    {
        Color color = valid ? Color.green : Color.red;
        foreach (Transform child in ghost.transform)
        {
            child.GetComponent<SpriteRenderer>().color = color;
        }
    }

    void OnMouseUp()
    {
        Destroy(ghost);
        transform.position = GetSnappedPosition();
        if (GridManager.Instance.CanShapeFit(this))
        {
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