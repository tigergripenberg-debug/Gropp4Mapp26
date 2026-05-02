using System.Collections.Generic;
using UnityEngine;

public class ShapeLibrary
{
    public static readonly Shape Square = new Shape(new Vector2Int[]
    {
        new(1,0), new(1,1),
        new(0,0), new(0,1),
    });
    public static readonly Shape LShape = new Shape(new Vector2Int[]
    {
        new(-1,1),
        new(-1,0),
        new(-1,-1), new(0,-1),
    });
    public static readonly Shape IShape = new Shape(new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(2, 0),
    });
    public static readonly Shape IShapeStanding = new Shape(new Vector2Int[]
    {
        new(0, -1),
        new(0, 0),
        new(0, 1),
        new(0, 2),
    });
    public static readonly Shape ZShape = new Shape(new Vector2Int[]
    {
        new(0, 0), new(1, 0),
                    new(1, 1), new(2, 1)
    });
    public static readonly Shape SShape = new Shape(new Vector2Int[]
    {
                    new(1,0), new(2,0),
        new(0,1), new(1,1),
    });
    public static readonly Shape TShape = new Shape(new Vector2Int[]
    {
        new(0,0),new(1,0),new(2,0),
                new(1, 1)
    });
}
