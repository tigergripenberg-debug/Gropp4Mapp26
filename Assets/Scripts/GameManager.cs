using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int height;
    private int width;

    private void Start()
    {
        Instance = this;
        height = GridManager.Instance.height;
        width = GridManager.Instance.width;
        if (PlayerPrefs.HasKey("save"))
        {
            LoadGame();
        }
        else
        {
            StartNewGame();
        }
    }

    private int[] FlattenGrid()
    {
        int[] flat = new int[width * height];
        int index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                flat[index++] = GridManager.Instance.gridLogic[x, y];
            }
        }
        return flat;
    }

    private void SaveGame()
    {
        SaveData data = new SaveData();
        data.width = width;
        data.height = height;
        data.grid = FlattenGrid();
        data.score = Score.Instance.score;
        data.currentShapes = BlockSpawner.Instance.currentShapes.Select(s => s.Name).ToArray();
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("save", json);
        PlayerPrefs.Save();
        Debug.Log("Game Saved");
        Debug.Log(json);
    }

    private void LoadGame()
    {
        if (!PlayerPrefs.HasKey("save"))
        {
            Debug.Log("No Game found");
            StartNewGame();
            return;
        }
        string json = PlayerPrefs.GetString("save");
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        GridManager.Instance.ClearExistingBoardVisuals();
        GridManager.Instance.gridLogic = new int[width, height];
        RestoreGrid(data);
        var score = data.score;
        BlockSpawner.Instance.RestoreShapes(data.currentShapes);
        Debug.Log("Game Loaded");
        Debug.Log(json);
    }

    private void RestoreGrid(SaveData data)
    {
        GridManager.Instance.gridLogic = new int[width, height];
        int index = 0;
        for (int y = 0; y < data.height; y++)
        {
            for (int x = 0; x < data.width;x++)
            {
                int value = data.grid[index++];
                GridManager.Instance.gridLogic[x, y] = value;
                if (value == 1)
                {
                    GridManager.Instance.SpawnPlacedBlock(x, y);
                }
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("save");
        PlayerPrefs.Save();
        Debug.Log("Game Deleted");
    }

    public void StartNewGame()
    {
        DeleteSave();
        GridManager.Instance.ClearExistingBoardVisuals();
        BlockSpawner.Instance.ClearCurrentShapes();
        GridManager.Instance.ResetGridLogic();
        Score.Instance.score = 0;
        BlockSpawner.Instance.SpawnShapes();
    }
}
