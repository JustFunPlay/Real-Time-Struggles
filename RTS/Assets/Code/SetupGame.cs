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
    public AiSetup[] aiSetups;

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
        foreach (AiSetup ai in aiSetups)
        {
            ai.armyManager.StartAI(ai.ecoScore, ai.agroScore, ai.costMod, ai.squadCap);
        }
    }
}

[System.Serializable]
public class AiSetup
{
    public AiArmyManager armyManager;
    public float ecoScore;
    public float agroScore;
    public float costMod;
    public int squadCap;
}