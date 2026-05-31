using UnityEngine;
using Random = UnityEngine.Random;


public class Shape
{
    public string Name;
    public Vector2Int[] cells;
    public int CellCount => cells.Length;
    
    public Shape(string name, Vector2Int[] cells)
    {
        Name = name;
        this.cells = cells;
    }

    public Vector2Int GetOriginCell()
    {
        Vector2Int best = cells[0];

        foreach (var cell in cells)
        {
            if (cell.y < best.y || (cell.y == best.y && cell.x < best.x))
            {
                best = cell;
            }
        }

        return best;
    }
}
