using UnityEngine;
using Random = UnityEngine.Random;


public class ShapeBehaviour : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 startPosition;
    private Vector3 previewScale = new Vector3(0.6f, 0.6f, 1f);
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private SpriteRenderer[] childSR;
    public Color[] possibleColors;
    public static event System.Action<SFXSounds> OnBlockPlacement;
    
    public void Initialize(Color[] colors)
    {
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
    
    private void OnMouseDown()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        offset = transform.position - mouseWorld;
        transform.localScale = normalScale;
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + offset;
    }
    
    void OnMouseUp()
    {
        Vector2Int snappedGrid = GridManager.Instance.WorldToGrid(transform.position);
        Vector2 snappedWorld = GridManager.Instance.GetWorldPosition(snappedGrid.x, snappedGrid.y);
        transform.position = new Vector3(snappedWorld.x, snappedWorld.y, 0f);
        bool isValid = true;
        foreach (Transform child in transform)
        {
            Vector2Int childPos = GridManager.Instance.WorldToGrid(child.position);
            int childX = childPos.x;
            int childY = childPos.y;
            if (childX < 0 || childX >= 8 || childY < 0 || childY >= 8)
            {
                isValid = false;
                break;
            }
            if (GridManager.Instance.gridLogic[childX, childY] == 1)
            {
                isValid = false;
                break;
            }
        }

        if (isValid)
        {
            SetAsPlaced();
            foreach (Transform child in transform)
            {
                Vector2Int childPos = GridManager.Instance.WorldToGrid(child.position);
                int childX = childPos.x;
                int childY = childPos.y;
                GridManager.Instance.gridLogic[childX, childY] = 1;
                GridManager.Instance.visualGrid[childX, childY] = child;
                child.name = $"Block X:{childX} Y:{childY}";
                OnBlockPlacement?.Invoke(SFXSounds.placement_sound);
            }
            GetComponent<Collider2D>().enabled = false;
            GridManager.Instance.CheckForMatches();
            BlockSpawner.Instance.BlockPlaced();
            GridManager.Instance.CheckIfPlayable();
        }
        else
        {
            SetAsActive();
            transform.position = startPosition;
            transform.localScale = previewScale;
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