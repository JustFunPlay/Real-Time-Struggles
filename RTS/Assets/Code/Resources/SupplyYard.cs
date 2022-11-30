using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshObstacle))]
public class SupplyYard : MonoBehaviour
{
    [Header("Resource aquisition")]
    public int resourcesPerTrip;
    public float collectionDuration;
    public float collectionLockout;
    public float truckFindRadius;

    [Header("Navigation")]
    public Transform entranceLocation;
    public Transform exitLocation;
    public GameObject entranceGate;
    public GameObject exitGate;

    SupplyTruck truckInAction;


    void Start()
    {
        PlayerTroopManager.instance.supplyYards.Add(this);
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
        truckInAction.MoveToPosition(transform.position);
        for (int i = 0; i < 10; i++)
        {
            entranceGate.transform.Translate(1, 0, 0);
            yield return new WaitForSeconds(0.075f);
        }
        while (Vector3.Distance(truckInAction.transform.position, transform.position) > 5f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 10; i++)
        {
            entranceGate.transform.Translate(-1, 0, 0);
            yield return new WaitForSeconds(0.075f);
        }


        yield return new WaitForSeconds(collectionDuration);
        truckInAction.heldResources = resourcesPerTrip;
        truckInAction.CheckSupplies();

        truckInAction.MoveToPosition(exitLocation.position);
        for (int i = 0; i < 10; i++)
        {
            exitGate.transform.Translate(1, 0, 0);
            yield return new WaitForSeconds(0.075f);
        }
        while (Vector3.Distance(truckInAction.transform.position, exitLocation.position) > 5f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 10; i++)
        {
            exitGate.transform.Translate(-1, 0, 0);
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
                if (!truckToLoad || Vector3.Distance(unit.transform.position, entranceLocation.position) < Vector3.Distance(truckToLoad.transform.position, entranceLocation.position))
                    truckToLoad = unit.GetComponent<SupplyTruck>();
            }
        }
        return truckToLoad;
    }


    void OnDestroy()
    {
        PlayerTroopManager.instance.supplyYards.Remove(this);
    }
}
