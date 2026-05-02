using UnityEngine;
using Random = UnityEngine.Random;


public class Shape
{
    public Vector2Int[] cells;
    
    public Shape(Vector2Int[] cells)
    {
        this.cells = cells;
    }
    
    public Vector2 GetCenter()
    {
        Vector2 min = Vector2.positiveInfinity;
        Vector2 max = Vector2.negativeInfinity;
        foreach (var cell in cells)
        {
            if (cell.x < min.x) min.x = cell.x;
            if (cell.y < min.y) min.y = cell.y;
            if (cell.x > max.x) max.x = cell.x;
            if (cell.y > max.y) max.y = cell.y;
        }
        return (min + max) / 2f;
    }
}
