using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : UnitBase
{
    [Header("Building")]
    public int buildCost;
    public int powerCost;
    [Range(1, 10)] public int requiredTrips;

    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == army_)
                hq.totalPower += powerCost;
        }
    }

    protected override void OnDestroy()
    {
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == army)
                hq.totalPower -= powerCost;
        }
        StopAllCoroutines();
        base.OnDestroy();
    }
    public IEnumerator EmergencyRepair(int heal, int cost)
    {
        while (currentHP < maxHP && HQBuilding.GetSupplies(10, army))
        {
            currentHP = Mathf.Min(currentHP + heal, maxHP);
            HQBuilding.ChangeSupplies(-cost, army);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
