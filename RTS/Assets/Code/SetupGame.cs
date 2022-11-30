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
    }
}
