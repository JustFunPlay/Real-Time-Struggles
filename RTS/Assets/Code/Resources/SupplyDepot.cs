using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SupplyDepot : Building
{
    [Header("Resource Aquisition")]
    public float truckFindRadius;
    public float collectionDuration;
    public float collectionLockout;

    [Header("Navigation")]
    public Transform entranceLocation;
    public Transform loadPoint;
    public Transform exitLocation;
    public Transform entranceGate;
    public Transform exitGate;

    [Header("Queue")]
    public float queueDistance;
    List<SupplyTruck> trucksInQueue = new List<SupplyTruck>();

    SupplyTruck truckInAction;
    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        StartCoroutine(CheckForTruck());
    }
    IEnumerator CheckForTruck()
    {
        while (!truckInAction)
        {
            SupplyTruck truck = FindTruckToLoad();
            truckInAction = truck;
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(LoadUpTruck());
    }

    IEnumerator LoadUpTruck()
    {
        truckInAction.canBeSeclected = false;
        truckInAction.OnDeselected();
        truckInAction.inBuilding = true;
        truckInAction.inQueue = false;
        trucksInQueue.Remove(truckInAction);
        truckInAction.MoveToPosition(loadPoint.position);
        for (int i = 0; i < 10; i++)
        {
            entranceGate.transform.Rotate(0, 0, 9);
            if (i == 3)
                entranceGate.GetComponent<NavMeshObstacle>().enabled = false;
            yield return new WaitForSeconds(0.075f);
        }
        while (truckInAction && Vector3.Distance(truckInAction.transform.position, loadPoint.position) > 5f)
        {
            //truckInAction.MoveToPosition(loadPoint.position);

            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 10; i++)
        {
            entranceGate.transform.Rotate(0, 0, -9);
            if (i == 6)
                entranceGate.GetComponent<NavMeshObstacle>().enabled = true;
            yield return new WaitForSeconds(0.075f);
        }
        if (!truckInAction)
        {
            StartCoroutine(CheckForTruck());
            yield break;
        }
        yield return new WaitForSeconds(collectionDuration);
        if (!truckInAction)
        {
            StartCoroutine(CheckForTruck());
            yield break;
        }
        if (truckInAction.constructionSite && HQBuilding.GetSupplies((truckInAction.constructionSite.building.buildCost / truckInAction.constructionSite.building.requiredTrips) - truckInAction.heldResources, army))
        {
            HQBuilding.ChangeSupplies(-((truckInAction.constructionSite.building.buildCost / truckInAction.constructionSite.building.requiredTrips) - truckInAction.heldResources), army);
            truckInAction.heldResources = (truckInAction.constructionSite.building.buildCost / truckInAction.constructionSite.building.requiredTrips);
        }
        else
        {
            HQBuilding.ChangeSupplies(truckInAction.heldResources, army);
            truckInAction.heldResources = 0;
        }
        truckInAction.CheckSupplies();

        truckInAction.MoveToPosition(exitLocation.position);
        for (int i = 0; i < 10; i++)
        {
            exitGate.transform.Rotate(0, 0, 9);
            if (i == 3)
                exitGate.GetComponent<NavMeshObstacle>().enabled = false;
            yield return new WaitForSeconds(0.075f);
        }
        while (truckInAction && Vector3.Distance(truckInAction.transform.position, exitLocation.position) > 5f)
        {
            truckInAction.MoveToPosition(exitLocation.position);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 10; i++)
        {
            exitGate.transform.Rotate(0, 0, -9);
            if (i == 6)
                exitGate.GetComponent<NavMeshObstacle>().enabled = true;
            yield return new WaitForSeconds(0.075f);
        }
        if (truckInAction)
        {
            truckInAction.CheckToAutomate();
            truckInAction.inBuilding = false;
            truckInAction.canBeSeclected = true;
            truckInAction = null;
        }
        yield return new WaitForSeconds(collectionLockout);
        StartCoroutine(CheckForTruck());
    }

    private void FixedUpdate()
    {
        for (int i = trucksInQueue.Count - 1; i >= 0; i--)
        {
            if (trucksInQueue[i])
                trucksInQueue[i].MoveToPosition(entranceLocation.position + (i * queueDistance * (trucksInQueue[i].transform.position - entranceLocation.position).normalized));
            else
                trucksInQueue.RemoveAt(i);
        }

        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.GetComponent<SupplyTruck>())
            {
                SupplyTruck truck = unit.GetComponent<SupplyTruck>();

                if (Vector3.Distance(truck.transform.position, entranceLocation.position) < queueDistance * (trucksInQueue.Count + 1) && !truck.inBuilding && !truck.inQueue && ((truck.heldResources > 0 && !truck.constructionSite)|| (truck.constructionSite && truck.heldResources < (truck.constructionSite.building.buildCost / truck.constructionSite.building.requiredTrips))))
                {
                    trucksInQueue.Add(truck);
                    truck.inQueue = true;
                }
            }
        }
    }
    SupplyTruck FindTruckToLoad()
    {
        SupplyTruck truckToLoad = null;
        if (trucksInQueue.Count > 0)
            truckToLoad = trucksInQueue[0];
        return truckToLoad;
    }
}
