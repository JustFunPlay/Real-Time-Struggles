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
            foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
            {
                if (unit.army == army && unit.GetComponent<SupplyTruck>() && unit.GetComponent<SupplyTruck>().constructionSite == null && unit.GetComponent<SupplyTruck>().supplying == false)
                {
                    if (!truck || Vector3.Distance(transform.position, unit.transform.position) < Vector3.Distance(transform.position, truck.transform.position))
                        truck = unit.GetComponent<SupplyTruck>();
                }
            }
            yield return new WaitForFixedUpdate();
        }
        truck.constructionSite = this;
        truck.CheckToAutomate();
        StartCoroutine(ConstructionInProgress());
    }
    IEnumerator ConstructionInProgress()
    {
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
}
