using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQBuilding : Factory
{
    [Header("Resources")]
    public int supplies;

    public override void AddedUnit(Army army_)
    {
        PlayerTroopManager.instance.HQs.Add(this);
        base.AddedUnit(army_);
    }

    public static bool GetSupplies(int requiredAmount, Army army)
    {
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == army)
            {
                if (hq.supplies >= requiredAmount)
                    return true;
            }
        }
        return false;
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
