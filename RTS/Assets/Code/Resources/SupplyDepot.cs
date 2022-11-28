using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Animator entranceGate;
    public Animator exitGate;

    SupplyTruck truckInAction;
    protected override void Start()
    {
        base.Start();
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
        entranceGate.SetTrigger("open");
        while (Vector3.Distance(truckInAction.transform.position, loadPoint.position) > 5f)
        {
            //truckInAction.MoveToPosition(loadPoint.position);

            yield return new WaitForSeconds(0.1f);
        }
        entranceGate.SetTrigger("close");

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

        truckInAction.MoveToPosition(exitLocation.position);
        exitGate.SetTrigger("open");
        while (Vector3.Distance(truckInAction.transform.position, exitLocation.position) > 5f)
        {
            truckInAction.MoveToPosition(exitLocation.position);
            yield return new WaitForSeconds(0.1f);
        }
        exitGate.SetTrigger("close");
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
