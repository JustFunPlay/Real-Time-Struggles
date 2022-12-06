using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    public Building building;
    public int investedResources;
    public float collectionRadius;

    public SupplyTruck truck;
    private void Start()
    {
        StartCoroutine(GetBuildingTruck());
    }
    IEnumerator GetBuildingTruck()
    {
        while (!truck)
        {
            foreach (UnitBase unit in PlayerTroopManager.instance.playerUnits)
            {
                if (unit.GetComponent<SupplyTruck>() && unit.GetComponent<SupplyTruck>().constructionSite == null && unit.GetComponent<SupplyTruck>().supplying == false)
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
            while (Vector3.Distance(transform.position, truck.transform.position) >= collectionRadius)
            {
                yield return new WaitForFixedUpdate();
            }
            truck.heldResources -= (building.buildCost / building.requiredTrips);
            investedResources += (building.buildCost / building.requiredTrips);
            truck.CheckSupplies();
            truck.CheckToAutomate();
            yield return new WaitForSeconds(1.5f);

        }
        truck.constructionSite = null;
        if (investedResources > building.buildCost)
        {
            truck.heldResources = (investedResources - building.buildCost);
            truck.CheckSupplies();
        }
        truck.CheckToAutomate();
        Invoke("FinishConstruction", 3f);
    }
    public void FinishConstruction()
    {
        Building newBuilding = Instantiate(building, transform.position, transform.rotation);
        newBuilding.AddedUnit(PlayerCam.playerArmy);
        Destroy(gameObject);
    }
}
