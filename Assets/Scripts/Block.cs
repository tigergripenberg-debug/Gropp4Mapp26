using UnityEngine;

public class Block : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 touchOffset;

    void Start()
    {
        
        startPosition = transform.position;
    }
    void OnMouseDown()
    {
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; 

        touchOffset = transform.position - mousePos;
    }

   void OnMouseDrag()
    {
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        transform.position = new Vector3(mousePos.x + touchOffset.x, mousePos.y + touchOffset.y + 2f, -1f);
    }

    void OnMouseUp()
    {
        float xOffset = (8 - 1) / 2f; 
        float yOffset = (8 - 0) / 2f; 

        
        int gridX = Mathf.RoundToInt(transform.position.x + xOffset);
        int gridY = Mathf.RoundToInt(transform.position.y + yOffset - 2f);

        float snappedX = gridX - xOffset;
        float snappedY = gridY - yOffset + 2f;

        
        transform.position = new Vector3(snappedX, snappedY, -1f);

        
        bool isValid = true;

        
        foreach (Transform child in transform)
        {
            
            int childX = Mathf.RoundToInt(child.position.x + xOffset);
            int childY = Mathf.RoundToInt(child.position.y + yOffset - 2f);

           
            if (childX < 0 || childX >= 8 || childY < 0 || childY >= 8)
            {
                
                isValid = false;
                break; 
            }
        }

        
        if (isValid)
        {
            
        }
        else
        {
            
            transform.position = startPosition;
        }
    }
}
