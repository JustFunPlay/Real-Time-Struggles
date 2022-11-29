using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : Building
{
    [Header("spawning units")]
    public Troop[] troops;
    public Transform spawnpoint;
    public Transform exitPoint;

    List<Troop> queue = new List<Troop>();
    bool isBuilding;
    HQBuilding hq;
    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        foreach (HQBuilding hq_ in PlayerTroopManager.instance.HQs)
        {
            if (hq_.army == army)
            {
                hq = hq_;
                break;
            }
        }
    }


    public void BuildNewTroop(int troopIndex)
    {
        if (hq.supplies >= troops[troopIndex].cost)
        {
            HQBuilding.ChangeSupplies(troops[troopIndex].cost, army);
            if (!isBuilding)
                StartCoroutine(BuildingTroop(troops[troopIndex]));
            else
                queue.Add(troops[troopIndex]);
        }
    }

    IEnumerator BuildingTroop(Troop troop)
    {
        isBuilding = true;
        yield return new WaitForSeconds(troop.buildTime);
        TroopMovement newTroop = Instantiate(troop.troop, spawnpoint.position, spawnpoint.rotation);
        newTroop.AddedUnit(army);
        newTroop.MoveToPosition(exitPoint.position);
        isBuilding = false;
        if (queue.Count > 0)
        {
            StartCoroutine(BuildingTroop(queue[0]));
            queue.RemoveAt(0);
        }
    }

}

[System.Serializable]
public class Troop
{
    public TroopMovement troop;
    public int cost;
    public float buildTime;
}