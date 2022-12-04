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
        truckInAction.MoveToPosition(loadPoint.position);
        for (int i = 0; i < 10; i++)
        {
            entranceGate.transform.Rotate(0, 0, 9);
            if (i == 3)
                entranceGate.GetComponent<NavMeshObstacle>().enabled = false;
            yield return new WaitForSeconds(0.075f);
        }
        while (Vector3.Distance(truckInAction.transform.position, loadPoint.position) > 5f)
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

        yield return new WaitForSeconds(collectionDuration);
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == army)
            {
                hq.supplies += truckInAction.heldResources;
                break;
            }
        }
        truckInAction.heldResources = 0;
        truckInAction.CheckSupplies();

        truckInAction.MoveToPosition(exitLocation.position);
        for (int i = 0; i < 10; i++)
        {
            exitGate.transform.Rotate(0, 0, 9);
            if (i == 3)
                exitGate.GetComponent<NavMeshObstacle>().enabled = false;
            yield return new WaitForSeconds(0.075f);
        }
        while (Vector3.Distance(truckInAction.transform.position, exitLocation.position) > 5f)
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
        truckInAction.CheckToAutomate();
        truckInAction = null;
        yield return new WaitForSeconds(collectionLockout);
        StartCoroutine(CheckForTruck());
    }

    SupplyTruck FindTruckToLoad()
    {
        SupplyTruck truckToLoad = null;
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.GetComponent<SupplyTruck>() && Vector3.Distance(unit.transform.position, entranceLocation.position) <= truckFindRadius)
            {
                if ((!truckToLoad || Vector3.Distance(unit.transform.position, entranceLocation.position) < Vector3.Distance(truckToLoad.transform.position, entranceLocation.position)) && unit.GetComponent<SupplyTruck>().heldResources > 0)
                    truckToLoad = unit.GetComponent<SupplyTruck>();
            }
        }
        return truckToLoad;
    }
}
