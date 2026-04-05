using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Inställningar")] 
    private static int width = 8;
    private static int height = 8;
    public GameObject tilePrefab;
    public int[,] grid;
    public GameObject[,] tileObjects = new GameObject[width,height];
    
    void Start()
    {
        grid = new int[width, height];
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

    public void CheckAndClearLines()
    {
        for (int y = 0; y < height; y++)
        {
            bool full = true;
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == 0)
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                ClearRow(y);
            }
        }

        for (int x = 0; x < width; x++)
        {
            bool full = true;
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 0)
                {
                    full = false;
                    break;
                }
            }
            if (full)
                {
                ClearColumn(x);
                }
        }
    }

    private void ClearColumn(int x)
    {
        for (int y = 0; y < height; y++)
        {
            grid[x, y] = 0;
            if (tileObjects[x, y] != null)
            {
                Destroy(tileObjects[x, y]);
                tileObjects[x, y] = null;
            }
        }
    }

    private void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            grid[x, y] = 0;
            if (tileObjects[x, y] != null)
            {
                Destroy(tileObjects[x, y]);
                tileObjects[x, y] = null;
            }
        }
    }
}