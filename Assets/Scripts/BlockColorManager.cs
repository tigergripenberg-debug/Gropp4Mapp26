using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance;
    [SerializeField] private BlockColorPalette[] blockPalettes;
    BlockColorPalette currentPalette;
    private int currentMilestone = 0;
    private readonly int defaultPalette = 0;

    private void Start()
    {
        Instance = this;
        SetPalette(blockPalettes[defaultPalette]);
    }

    private void SetPalette(BlockColorPalette palette)
    {
        currentPalette = palette;
        BlockSpawner.Instance.SetPalette(palette);
        GridManager.Instance.SetPalette(palette);
        GridManager.Instance.RefreshBlockColors();
        BlockSpawner.Instance.RefreshActiveShapeColors();
    }
    public void UpdatePaletteFromScore(int score)
    {
        int milestone = score / 1000;
        if (milestone == currentMilestone)
            return;
        currentMilestone = milestone;
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, blockPalettes.Length);
        }
        while (
            blockPalettes[randomIndex] ==
            currentPalette);
        SetPalette(blockPalettes[randomIndex]);
    }
}