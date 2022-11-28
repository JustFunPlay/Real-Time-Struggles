using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQBuilding : Building
{
    [Header("Resources")]
    public int supplies;

    [Header("Resource Truck Spawning")]
    public SupplyTruck truck;
    public int truckCost;
    public float truckBuildTime;
    public Transform buildPoint;
    public Transform exitPoint;

    public int buildQueue;
    bool isBuilding;

    protected override void Start()
    {
        base.Start();
        PlayerTroopManager.instance.HQs.Add(this);
    }

    public void StartBuilding()
    {
        supplies -= truckCost;
        if (!isBuilding)
            StartCoroutine(BuildingNewTruck());
        else
            buildQueue++;
    }
    public void RemoveBuilding()
    {
        if (buildQueue > 0)
            buildQueue--;
        else
            StopCoroutine(BuildingNewTruck());
        supplies += truckCost;
    }

    IEnumerator BuildingNewTruck()
    {
        isBuilding = true;
        yield return new WaitForSeconds(truckBuildTime);
        SupplyTruck newTruck = Instantiate(truck, buildPoint.position, buildPoint.rotation);
        newTruck.AddedUnit(army);
        newTruck.MoveToPosition(exitPoint.position);
        isBuilding = false;
        if (buildQueue > 0)
        {
            buildQueue--;
            StartCoroutine(BuildingNewTruck());
        }

    }
}
