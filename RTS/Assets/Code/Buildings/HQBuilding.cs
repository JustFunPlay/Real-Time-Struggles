using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQBuilding : Factory
{
    [Header("Resources")]
    public int supplies;

    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        PlayerTroopManager.instance.HQs.Add(this);
    }

    public static void ChangeSupplies(int supplies, Army army)
    {
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == army)
                hq.supplies += supplies;
        }
    }
}
