using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Inställningar")]
    public int width = 8;
    public int height = 8;
    public GameObject tilePrefab;
    public int[,] gridLogic;
    
    void Start()
    {
        gridLogic = new int[width, height];
        GenerateGrid();
    }
    void GenerateGrid()
    {
        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 0) / 2f;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newTile = Instantiate(tilePrefab);
                newTile.transform.position = new Vector2(x - xOffset, y - yOffset + 2f);
                newTile.name = $"Tile X:{x} Y:{y}";
                newTile.transform.SetParent(transform);
            }
        }
    }
}