using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQBuilding : Factory
{
    [Header("Resources")]
    public int supplies;
    public int maxPower;
    public int currentPower;

    [Header("Healing")]
    public bool canHeal;
    public float healCooldown;
    public int healPerTick;
    public int costPerHeal;

    public override void AddedUnit(Army army_)
    {
        PlayerTroopManager.instance.HQs.Add(this);
        base.AddedUnit(army_);
        canHeal = true;
    }
    public void HealAllBuildings()
    {
        if (canHeal)
        {
            canHeal = false;
            foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
            {
                if (unit.army == army && unit.GetComponent<Building>())
                {
                    StartCoroutine(unit.GetComponent<Building>().EmergencyRepair(healPerTick, costPerHeal));
                }
            }
            StartCoroutine(WaitForHeal());
        }
    }
    IEnumerator WaitForHeal()
    {
        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
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
    public static bool HasPower(int newPower, Army army)
    {
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == army)
            {
                if (newPower >= 0)
                {
                    if (hq.currentPower + newPower <= hq.maxPower)
                        return true;

                }
                else
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
    protected override void OnDeath()
    {
        PlayerTroopManager.instance.HQs.Remove(this);
        GameEnd.instance.CheckGameEnd();
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army == army && unit != this)
                unit.OnTakeDamage(10000);
        }
        base.OnDeath();
    }
}
