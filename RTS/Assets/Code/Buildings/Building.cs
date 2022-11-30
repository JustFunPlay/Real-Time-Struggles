using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : UnitBase
{
    [Header("Building")]
    public int buildCost;
    public int requiredTrips;
    public bool RequiresBuilding = true;
}
