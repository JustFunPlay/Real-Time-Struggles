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
        for (int i = 0; i < playerUnits.Length; i++)
        {
            playerUnits[i].AddedUnit(Army.Blue);
        }
        for (int i = 0; i < armyOne.Length; i++)
        {
            armyOne[i].AddedUnit(Army.Red);
        }
        for (int i = 0; i < armyTwo.Length; i++)
        {
            armyTwo[i].AddedUnit(Army.Green);
        }
        for (int i = 0; i < armyThree.Length; i++)
        {
            armyThree[i].AddedUnit(Army.Magenta);
        }
        for (int i = 0; i < armyFour.Length; i++)
        {
            armyFour[i].AddedUnit(Army.Yellow);
        }
        for (int i = 0; i < armyFive.Length; i++)
        {
            armyFive[i].AddedUnit(Army.Cyan);
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