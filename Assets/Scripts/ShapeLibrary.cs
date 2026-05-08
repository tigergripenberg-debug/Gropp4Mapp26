using UnityEditor;
using UnityEngine;

public class ShapeLibrary
{
    private static readonly Shape Square = new Shape("Square",new Vector2Int[]
    {
        new(1,0), new(1,1),
        new(0,0), new(0,1),
    });
    private static readonly Shape ZFacingLeft = new Shape("Z Left",new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(0, 0),
        new(1, 0),
    });
    private static readonly Shape ZFacingRight = new Shape("Z Right",new Vector2Int[]
    {
        new(0, 1),
        new(1, 1),
        new(-1, 0),
        new(0, 0),
    });
    private static readonly Shape TFacingUp = new Shape("T Up",new Vector2Int[]
    {
        new(0, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
    });
    private static readonly Shape TFacingDown = new Shape("T Down",new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(0, -1),
    });
    private static readonly Shape TFacingRight = new Shape("T Right",new Vector2Int[]
    {
        new(0, 1),
        new(0, 0),
        new(1, 0),
        new(0, -1),
    });
    private static readonly Shape TFacingLeft = new Shape("T Left",new Vector2Int[]
    {
        new(0, 1),
        new(-1, 0),
        new(0, 0),
        new(0, -1),
    });
    private static readonly Shape BigBlock = new Shape("Big Block",new Vector2Int[]
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
    private static readonly Shape TwoByThreeLaying = new Shape("2x3 Laying",new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
    });
    private static readonly Shape TwoByThreeStanding = new Shape("2x3 Standing",new Vector2Int[]
    {
        new(0, 1),
        new(1, 1),
        new(0, 0),
        new(1, 0),
        new(0, -1),
        new(1, -1),
    });

    private static readonly Shape TwoDiagonalRight = new Shape("2 Diagonal Right",new Vector2Int[]
    {
        new(0, 0),
        new(1, -1),
    });
    private static readonly Shape TwoDiagonalLeft = new Shape("2 Diagonal Left",new Vector2Int[]
    {
        new(0, 0),
        new(-1, -1),
    });
    private static readonly Shape FiveLongStanding = new Shape("5 Long Standing",new Vector2Int[]
    {
        new(0, 2),
        new(0, 1),
        new(0, 0),
        new(0, -1),
        new(0, -2),
    });
    private static readonly Shape FiveLongLaying = new Shape("5 Long Laying",new Vector2Int[]
    {
        new(-2, 0),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(2, 0),
    });
    private static readonly Shape TwoLongStanding = new Shape("2 Long Standing",new Vector2Int[]
    {
        new(0, 1),
        new(0, 0),
    });
    private static readonly Shape TwoLongLaying = new Shape("2 Long Laying",new Vector2Int[]
    {
        new(0, 1),
        new(0, 0),
    });
    private static readonly Shape LSmallFacingRight = new Shape("Small L  Right",new Vector2Int[]
    {
        new(0, 1),
        new(0, 0),
        new(1, 0),
    });
    private static readonly Shape LSmallFacingLeft = new Shape("Small L Left",new Vector2Int[]
    {
        new(0, 1),
        new(-1, 0),
        new(0, 0),
    });
    private static readonly Shape LBigFacingRightUp = new Shape("Big L Right Up",new Vector2Int[]
    {
        new(-1, 1),
        new(-1, 0),
        new(-1, -1),
        new(0, -1),
        new(1, -1),
    });
    private static readonly Shape LBigFacingLeftUp = new Shape("Big L Left Up",new Vector2Int[]
    {
        new(1, 1),
        new(1, 0),
        new(-1, -1),
        new(0, -1),
        new(1, -1),
    });
    private static readonly Shape LBigFacingRightDown = new Shape("Big L Right Down",new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),
        new(-1, 0),
        new(-1, -1),
    });
    private static readonly Shape LBigFacingLeftDown = new Shape("Big L Left Down",new Vector2Int[]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),
        new(1, 0),
        new(1, -1),
    });
    private static readonly Shape LMediumFacingLeftDown = new Shape("Medium L Left Down",new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(1, -1),
    });
    private static readonly Shape LMediumFacingRightDown = new Shape("Medium L Right Down",new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(-1, -1),
    });
    private static readonly Shape LMediumFacingRightUp = new Shape("Medium L Right Up",new Vector2Int[]
    {
        new(-1, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
    });
    private static readonly Shape LMediumFacingLeftUp = new Shape("Medium L Left Up",new Vector2Int[]
    {
        new(1, 1),
        new(-1, 0),
        new(0, 0),
        new(1, 0),
    });
    private static readonly Shape FourLongStanding = new Shape("4 Long Standing",new Vector2Int[]
    {
        new(0, 2),
        new(0, 1),
        new(0, 0),
        new(0, -1),
    });
    private static readonly Shape FourLongLaying = new Shape("4 Long Laying",new Vector2Int[]
    {
        new(-1, 0),
        new(0, 0),
        new(1, 0),
        new(2, 0),
    });
    private static readonly Shape ThreeDiagonalLeft = new Shape("3 Diagonal Left",new Vector2Int[]
    {
        new(-1, 1),
        new(0, 0),
        new(1, -1),
    });
    private static readonly Shape ThreeDiagonalRight = new Shape("3 Diagonal Right",new Vector2Int[]
    {
        new(1, 1),
        new(0, 0),
        new(-1, -1),
    });

    private static readonly Shape One = new Shape("One",new Vector2Int[]
    {
        new(0, 0)
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
        ThreeDiagonalRight,
        One
    };
}
