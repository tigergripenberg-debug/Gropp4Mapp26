using UnityEditor;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class ShapeGeneratorWindow : EditorWindow
{
    private const int GridSize = 9;

    private bool[,] grid = new bool[GridSize, GridSize];

    private string shapeName = "NewShape";

    [MenuItem("Tools/Shape Generator")]
    public static void Open()
    {
        GetWindow<ShapeGeneratorWindow>("Shape Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Shape Generator", EditorStyles.boldLabel);

        shapeName = EditorGUILayout.TextField("Shape Name", shapeName);

        GUILayout.Space(10);

        DrawGrid();

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Shape Code"))
        {
            GenerateCode();
        }

        if (GUILayout.Button("Clear"))
        {
            grid = new bool[GridSize, GridSize];
        }
    }

    private void DrawGrid()
    {
        for (int y = GridSize - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();

            for (int x = 0; x < GridSize; x++)
            {
                GUI.backgroundColor =
                    grid[x, y]
                    ? Color.green
                    : Color.gray;

                if (GUILayout.Button(
                    "",
                    GUILayout.Width(35),
                    GUILayout.Height(35)))
                {
                    grid[x, y] = !grid[x, y];
                }
            }

            GUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;
    }

    private void GenerateCode()
    {
        List<Vector2Int> cells = new();

        int center = GridSize / 2;

        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                if (grid[x, y])
                {
                    // Center-origin coordinates
                    cells.Add(new Vector2Int(
                        x - center,
                        y - center));
                }
            }
        }

        if (cells.Count == 0)
        {
            Debug.LogWarning("No cells selected.");
            return;
        }

        StringBuilder sb = new();

        sb.AppendLine(
$@"public static readonly Shape {shapeName} = new Shape(new Vector2Int[]
{{");

        foreach (var cell in cells.OrderByDescending(c => c.y).ThenBy(c => c.x))
        {
            sb.AppendLine(
$@"    new({cell.x}, {cell.y}),");
        }

        sb.AppendLine("});");

        string code = sb.ToString();

        EditorGUIUtility.systemCopyBuffer = code;

        Debug.Log("Shape code copied to clipboard:\n\n" + code);
    }
}