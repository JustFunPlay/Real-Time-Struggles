using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupGame : MonoBehaviour
{
    public UnitBase[] playerUnits;
    public UnitBase[] armyOne;
    public UnitBase[] armyTwo;
    public UnitBase[] armyThree;
    public UnitBase[] armyFour;
    public UnitBase[] armyFive;
    public AiArmyManager[] ais;

    void Start()
    {
        List<Army> aiArmies = new List<Army>();
        for (int i = 0; i < playerUnits.Length; i++)
        {
            playerUnits[i].AddedUnit(DifficultyLoader.instance.playerArmy);
        }
        
        for (int i = 0; i < 5; i++)
        {
            while (aiArmies.Count == i)
            {
                Army newArmy;
                int nmbr = Random.Range(0, 6);
                switch (nmbr)
                {
                    case 0:
                        newArmy = Army.Blue;
                        break;
                    case 1:
                        newArmy = Army.Cyan;
                        break;
                    case 2:
                        newArmy = Army.Green;
                        break;
                    case 3:
                        newArmy = Army.Magenta;
                        break;
                    case 4:
                        newArmy = Army.Red;
                        break;
                    default:
                        newArmy = Army.Yellow;
                        break;
                }
                if (newArmy != DifficultyLoader.instance.playerArmy && !aiArmies.Contains(newArmy))
                    aiArmies.Add(newArmy);
            }
        }

        for (int i = 0; i < armyOne.Length; i++)
        {
            armyOne[i].AddedUnit(aiArmies[0]);
        }
        for (int i = 0; i < armyTwo.Length; i++)
        {
            armyTwo[i].AddedUnit(aiArmies[1]);
        }
        for (int i = 0; i < armyThree.Length; i++)
        {
            armyThree[i].AddedUnit(aiArmies[2]);
        }
        for (int i = 0; i < armyFour.Length; i++)
        {
            armyFour[i].AddedUnit(aiArmies[3]);
        }
        for (int i = 0; i < armyFive.Length; i++)
        {
            armyFive[i].AddedUnit(aiArmies[4]);
        }
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            hq.supplies = DifficultyLoader.instance.startingCash;
        }
        foreach (AiArmyManager ai in ais)
        {
            int i = Random.Range(0, 3);
            switch (DifficultyLoader.instance.difficulty)
            {
                case Difficulty.Easy:
                    switch (i)
                    {
                        case 0:
                            ai.StartAI(3, 1, 4);
                            break;
                        case 1:
                            ai.StartAI(1, 3, 2.7f);
                            break;
                        default:
                            ai.StartAI(2, 2, 3.3f);
                            break;

                    }
                    break;
                case Difficulty.Normal:
                    switch (i)
                    {
                        case 0:
                            ai.StartAI(4, 6, 2.5f, 4);
                            break;
                        case 1:
                            ai.StartAI(5, 5, 2.5f, 4);
                            break;
                        default:
                            ai.StartAI(6, 4, 2.5f, 4);
                            break;

                    }
                    break;
                case Difficulty.Hard:
                    switch (i)
                    {
                        case 0:
                            ai.StartAI(7, 9, 1.5f, 5);
                            break;
                        case 1:
                            ai.StartAI(8, 8, 1.5f, 5);
                            break;
                        default:
                            ai.StartAI(9, 7, 1.5f, 5);
                            break;

                    }
                    break;
                default:
                    ai.StartAI(10, 10, 0.5f, 6);
                    break;
            }
        }
    }
}