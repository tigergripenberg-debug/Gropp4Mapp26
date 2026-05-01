using UnityEngine;

public class ShapeLibrary
{
    public static Shape Square = new Shape(new Vector2Int[]
    {
        new(0,0), new(1,0),
        new(0,1), new(1,1),
    });
    public static Shape LShape = new Shape(new Vector2Int[]
    {
        new(2,0),
        new(0,1),
        new(1,1), new(2,1),
    });
    public static Shape IShape = new Shape(new Vector2Int[]
    {
        new(0, 0),
        new(1, 0),
        new(2, 0),
        new(3, 0),
    });
    public static Shape ZShape = new Shape(new Vector2Int[]
    {
        new(0, 0), new(1, 0),
                    new(1, 1), new(2, 1)
    });
    public static Shape SShape = new Shape(new Vector2Int[]
    {
                    new(1,0), new(2,0),
        new(0,1), new(1,1),
    });
    public static Shape TShape = new Shape(new Vector2Int[]
    {
        new(0,0),new(1,0),new(2,0),
                new(1, 1)
    });
}
