using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int turnsSinceClear;
    public int maxTurnsSinceClear;
    public GridManager gridManager;

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

    public void CheckIfBoardCleared(bool didClearRow)
    {
        if (didClearRow)
        {
            turnsSinceClear = 0;
        }
        else
        {
            turnsSinceClear++;
        }

        if (turnsSinceClear > maxTurnsSinceClear)
        {
            gridManager.MoveGridUp();
            turu
        }
    }
}
