using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : UnitBase
{
    [Header("Building")]
    public int buildCost;
    [Range(1, 10)] public int requiredTrips;
}
