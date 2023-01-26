using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDifficultyAndLevel : MonoBehaviour
{
    public void SelectArmy(int playerArmy)
    {
        switch (playerArmy)
        {
            case 0:
                DifficultyLoader.instance.playerArmy = Army.Blue;
                break;
            case 1:
                DifficultyLoader.instance.playerArmy = Army.Cyan;
                break;
            case 2:
                DifficultyLoader.instance.playerArmy = Army.Green;
                break;
            case 3:
                DifficultyLoader.instance.playerArmy = Army.Magenta;
                break;
            case 4:
                DifficultyLoader.instance.playerArmy = Army.Red;
                break;
            default:
                DifficultyLoader.instance.playerArmy = Army.Yellow;
                break;
        }
    }
    public void SelectDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                DifficultyLoader.instance.difficulty = Difficulty.Easy;
                break;
            case 1:
                DifficultyLoader.instance.difficulty = Difficulty.Normal;
                break;
            default:
                DifficultyLoader.instance.difficulty = Difficulty.Hard;
                break;
        }
    }
    public void StartingCash(int startingCash)
    {
        switch (startingCash)
        {
            case 0:
                DifficultyLoader.instance.startingCash = 500;
                break;
            case 1:
                DifficultyLoader.instance.startingCash = 1000;
                break;
            case 2:
                DifficultyLoader.instance.startingCash = 2000;
                break;
            case 3:
                DifficultyLoader.instance.startingCash = 5000;
                break;
            default:
                DifficultyLoader.instance.startingCash = 10000;
                break;
        }
    }
}
