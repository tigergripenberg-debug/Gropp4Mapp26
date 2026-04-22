using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 touchOffset;
    private Vector3 previewSize = new Vector3(0.6f, 0.6f, 1f);
    private Vector3 normalSize = new Vector3(1f, 1f, 1f);

    void Start()
    {
        startPosition = transform.position;
        transform.localScale = previewSize;
    }

    void OnMouseDown()
    {
        if (MenuController.gameIsPaused) return;

        transform.localScale = normalSize;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        touchOffset = transform.position - mousePos;
    }

    void OnMouseDrag()
    {
        if (MenuController.gameIsPaused) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = new Vector3(mousePos.x + touchOffset.x, mousePos.y + touchOffset.y + 2f, -1f);
    }
    
    public static event System.Action<SFXSounds> OnBlockPlacement;

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

        }
        else
        {
            transform.position = startPosition;
            transform.localScale = previewSize;
        }
    }
}