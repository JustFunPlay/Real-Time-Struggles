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
        foreach (UnitBase unit in playerUnits)
        {
            unit.AddedUnit(Army.Blue);
        }
        foreach (UnitBase unit in armyOne)
        {
            unit.AddedUnit(Army.Red);
        }
        foreach (UnitBase unit in armyTwo)
        {
            unit.AddedUnit(Army.Green);
        }
        foreach (UnitBase unit in armyThree)
        {
            unit.AddedUnit(Army.Magenta);
        }
        foreach (UnitBase unit in armyFour)
        {
            unit.AddedUnit(Army.Yellow);
        }
        foreach (UnitBase unit in armyFive)
        {
            unit.AddedUnit(Army.Cyan);
        }
    }
}
