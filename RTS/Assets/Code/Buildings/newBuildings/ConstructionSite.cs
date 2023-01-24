using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : Building
{
    public Building building;
    public int investedResources;
    public float collectionRadius;

    public SupplyTruck truck;
    public override void AddedUnit(Army army_)
    {
        maxHP = building.maxHP / 2;
        powerCost = building.powerCost > 0 ? building.powerCost : 0;
        base.AddedUnit(army_);
        StartCoroutine(GetBuildingTruck());
    }
    IEnumerator GetBuildingTruck()
    {
        while (!truck)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (SupplyTruck truck_ in PlayerTroopManager.instance.allUnits)
            {
                if (truck_.army == army && truck_.constructionSite == null && truck_.inBuilding == false)
                {
                    if (!truck || Vector3.Distance(transform.position, truck_.transform.position) < Vector3.Distance(transform.position, truck.transform.position))
                    {
                        if (truck)
                            truck.constructionSite = null;
                        truck = truck_;
                        truck.constructionSite = this;
                    }
                }
            }
        }
        truck.CheckToAutomate();
        StartCoroutine(ConstructionInProgress());
    }
    IEnumerator ConstructionInProgress()
    {
        float progress = ((investedResources / (building.buildCost * 100f)) * 10000f);
        unitName = $"{building.unitName} ({(int)progress}%)";
        while (investedResources < building.buildCost)
        {
            yield return new WaitForFixedUpdate();
            while (truck && truck.heldResources < building.buildCost / building.requiredTrips)
                yield return new WaitForFixedUpdate();
            while (truck && Vector3.Distance(truck.transform.position, GetClosestTargetingPoint(truck.transform.position)) >= collectionRadius)
            {
                yield return new WaitForFixedUpdate();
            }
            if (!truck)
            {
                StartCoroutine(GetBuildingTruck());
                StopCoroutine(ConstructionInProgress());
            }
            truck.heldResources -= (building.buildCost / building.requiredTrips);
            investedResources += (building.buildCost / building.requiredTrips);
            progress = ((investedResources / (building.buildCost * 100f)) * 10000f);
            unitName = $"{building.unitName} ({(int)progress}%)";
            truck.CheckSupplies();
            truck.CheckToAutomate();

        }
        truck.constructionSite = null;
        if (investedResources > building.buildCost)
        {
            truck.heldResources = (investedResources - building.buildCost);
            truck.CheckSupplies();
        }
        truck.CheckToAutomate();
        truck = null;
        Invoke("FinishConstruction", 1f);
    }
    public void FinishConstruction()
    {
        Building newBuilding = Instantiate(building, transform.position, transform.rotation);
        newBuilding.AddedUnit(army);
        Destroy(gameObject);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (truck)
        {
            truck.constructionSite = null;
            truck.CheckToAutomate();
        }
    }
    public override void Sell()
    {
        HQBuilding.ChangeSupplies((int)(investedResources * 0.75f), army);
        base.Sell();
    }
}
