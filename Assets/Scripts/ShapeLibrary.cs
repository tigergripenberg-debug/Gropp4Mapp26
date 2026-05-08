using UnityEngine;

public class ShapeLibrary
{
    public static readonly Shape Square = new Shape(new Vector2Int[]
    {
        new(1,0), new(1,1),
        new(0,0), new(0,1),
    });
    public static readonly Shape ZFacingLeft = new Shape(new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(0, 0),
        new(1, 0),
    });
    public static readonly Shape ZFacingRight = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(1, 1),
        new(-1, 0),
        new(0, 0),
    });
    public static readonly Shape TFacingUp = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
    });
    public static readonly Shape TFacingDown = new Shape(new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(0, -1),
    });
    public static readonly Shape TFacingRight = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(0, 0),
        new(1, 0),
        new(0, -1),
    });
    public static readonly Shape TFacingLeft = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(-1, 0),
        new(0, 0),
        new(0, -1),
    });
    public static readonly Shape BigBlock = new Shape(new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(-1, -1),
        new(0, -1),
        new(1, -1),
    });
    public static readonly Shape TwoByThreeLaying = new Shape(new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
    });
    public static readonly Shape TwoByThreeStanding = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(1, 1),
        new(0, 0),
        new(1, 0),
        new(0, -1),
        new(1, -1),
    });

    public static readonly Shape TwoDiagonalRight = new Shape(new Vector2Int[]
    {
        new(0, 0),
        new(1, -1),
    });
    public static readonly Shape TwoDiagonalLeft = new Shape(new Vector2Int[]
    {
        new(0, 0),
        new(-1, -1),
    });
    public static readonly Shape FiveLongStanding = new Shape(new Vector2Int[]
    {
        new(0, 2),
        new(0, 1),
        new(0, 0),
        new(0, -1),
        new(0, -2),
    });
    public static readonly Shape FiveLongLaying = new Shape(new Vector2Int[]
    {
        new(-2, 0),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(2, 0),
    });
    public static readonly Shape TwoLongStanding = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(0, 0),
    });
    public static readonly Shape TwoLongLaying = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(0, 0),
    });
    public static readonly Shape LSmallFacingRight = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(0, 0),
        new(1, 0),
    });
    public static readonly Shape LSmallFacingLeft = new Shape(new Vector2Int[]
    {
        new(0, 1),
        new(-1, 0),
        new(0, 0),
    });
    public static readonly Shape LBigFacingRightUp = new Shape(new Vector2Int[]
    {
        new(-1, 1),
        new(-1, 0),
        new(-1, -1),
        new(0, -1),
        new(1, -1),
    });
    public static readonly Shape LBigFacingLeftUp = new Shape(new Vector2Int[]
    {
        new(1, 1),
        new(1, 0),
        new(-1, -1),
        new(0, -1),
        new(1, -1),
    });
    public static readonly Shape LBigFacingRightDown = new Shape(new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),
        new(-1, 0),
        new(-1, -1),
    });
    public static readonly Shape LBigFacingLeftDown = new Shape(new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),
        new(1, 0),
        new(1, -1),
    });
    public static readonly Shape LMediumFacingLeftDown = new Shape(new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(1, -1),
    });
    public static readonly Shape LMediumFacingRightDown = new Shape(new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(-1, -1),
    });
    public static readonly Shape LMediumFacingRightUp = new Shape(new Vector2Int[]
    {
        new(-1, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
    });
    public static readonly Shape LMediumFacingLeftUp = new Shape(new Vector2Int[]
    {
        new(1, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
    });
    public static readonly Shape FourLongStanding = new Shape(new Vector2Int[]
    {
        new(0, 2),
        new(0, 1),
        new(0, 0),
        new(0, -1),
    });
    public static readonly Shape FourLongLaying = new Shape(new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(2, 0),
    });
    public static readonly Shape ThreeDiagonalLeft = new Shape(new Vector2Int[]
    {
        new(-1, 1),
        new(0, 0),
        new(1, -1),
    });
    public static readonly Shape ThreeDiagonalRight = new Shape(new Vector2Int[]
    {
        new(1, 1),
        new(0, 0),
        new(-1, -1),
    });

    public static readonly Shape[] allShapes =
    {
        Square,
        ZFacingLeft,
        ZFacingRight,
        TFacingDown,
        TFacingUp,
        TFacingLeft,
        TFacingRight,
        BigBlock,
        TwoByThreeLaying,
        TwoByThreeStanding,
        TwoDiagonalLeft,
        TwoDiagonalRight,
        FiveLongLaying,
        FiveLongStanding,
        FourLongLaying,
        FourLongStanding,
        TwoLongLaying,
        TwoLongStanding,
        LSmallFacingLeft,
        LSmallFacingRight,
        LBigFacingLeftDown,
        LBigFacingLeftUp,
        LBigFacingRightDown,
        LBigFacingRightUp,
        LMediumFacingLeftDown,
        LMediumFacingLeftUp,
        LMediumFacingRightDown,
        LMediumFacingRightUp,
        ThreeDiagonalLeft,
        ThreeDiagonalRight
    };
}
