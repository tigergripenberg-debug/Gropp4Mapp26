using UnityEngine;

public class Block : MonoBehaviour
{

    
    public bool IsPickedUp { get; private set; } = false;
    public bool IsPlaced { get; private set; } = false;
    private Vector3 startPosition;
    private Vector3 touchOffset;
    // Storlekar för att göra blocken mindre när de inte är i handen, och större när de är det.
    private Vector3 previewSize = new Vector3(0.6f, 0.6f, 1f);
    private Vector3 normalSize = new Vector3(1f, 1f, 1f);

    void Start()
    {
        startPosition = transform.position;
        transform.localScale = previewSize;
    }

    // När blocket klickas på, sätts IsPickedUp till true och blocket följer musen/fingret. 
    // När det släpps, kontrolleras om det är i en giltig position. 
    // Om det är det, placeras blocket i rutnätet och IsPlaced sätts till true. Om inte, återställs blockets position.
    void OnMouseDown()
    {
        IsPickedUp = true;

        if (MenuController.gameIsPaused) return;

        transform.localScale = normalSize;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        touchOffset = transform.position - mousePos;
    }
    // När blocket dras, uppdateras dess position baserat på musens/fingrets position och touchOffset.
    void OnMouseDrag()
    {
        if (MenuController.gameIsPaused) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = new Vector3(mousePos.x + touchOffset.x, mousePos.y + touchOffset.y + 2f, -1f);
    }

    // När blocket släpps, kontrolleras om det är i en giltig position.
    // Räknar ut om det är inom griden och inte överlappar med andra block.
    void OnMouseUp()
    {
        IsPickedUp = false;
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
        // Om positionen är giltig, uppdateras gridLogic och visualGrid i GridManager, 
        // blocket placeras permanent och matchningar kontrolleras.
        // Kollidern inaktiveras så att man inte kan dra det igen.
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
                //OnBlockPlacement?.Invoke(SFXSounds.placement_sound);
            }
            IsPlaced = true;
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