using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : Building
{
    [Header("spawning units")]
    public Troop[] troops;
    public Transform spawnpoint;
    public Transform exitPoint;

    public List<Troop> queue = new List<Troop>();
    public Troop troopInProgress;
    public int timeLeftToBuild;
    public bool isBuilding;

    public ParticleSystem[] buildingSmokes;

    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
    }


    public void BuildNewTroop(int troopIndex)
    {
        if (HQBuilding.GetSupplies(troops[troopIndex].cost, army))
        {
            HQBuilding.ChangeSupplies(-troops[troopIndex].cost, army);
            if (!isBuilding)
                StartCoroutine(BuildingTroop(troops[troopIndex]));
            else
                queue.Add(troops[troopIndex]);
        }
    }

    IEnumerator BuildingTroop(Troop troop)
    {
        isBuilding = true;
        troopInProgress = troop;
        for (int i = 0; i < buildingSmokes.Length; i++)
        {
            buildingSmokes[i].Play();
        }
        for (float f = troop.buildTime; f > 0; f -= 0.5f)
        {
            timeLeftToBuild = Mathf.CeilToInt(f);
            yield return new WaitForSeconds(0.5f);
        }
        TroopMovement newTroop = Instantiate(troop.troop, spawnpoint.position, spawnpoint.rotation);
        newTroop.AddedUnit(army);
        newTroop.MoveToPosition(exitPoint.position);
        isBuilding = false;
        troopInProgress = null;
        if (queue.Count > 0)
        {
            StartCoroutine(BuildingTroop(queue[0]));
            queue.RemoveAt(0);
        }
        else
        {
            for (int i = 0; i < buildingSmokes.Length; i++)
            {
                buildingSmokes[i].Stop();
            }
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