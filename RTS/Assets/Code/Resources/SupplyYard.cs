using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("Queue")]
    public float queueDistance;
    public List<SupplyTruck> trucksInQueue = new List<SupplyTruck>();

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
        truckInAction.canBeSeclected = false;
        truckInAction.OnDeselected();
        truckInAction.inBuilding = true;
        truckInAction.inQueue = false;
        trucksInQueue.Remove(truckInAction);
        truckInAction.MoveToPosition(transform.position);
        for (int i = 0; i < 10; i++)
        {
            entranceGate.transform.Rotate(0, 0, 9);
            if (i == 3)
                entranceGate.GetComponent<NavMeshObstacle>().enabled = false;
            yield return new WaitForSeconds(0.075f);
        }
        while (truckInAction && Vector3.Distance(truckInAction.transform.position, transform.position) > 5f)
        {
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
        truckInAction.heldResources = resourcesPerTrip;
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

    void FixedUpdate()
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
            if (unit.GetComponent<SupplyTruck>() && !unit.GetComponent<SupplyTruck>().inBuilding)
            {
                if (Vector3.Distance(unit.transform.position, entranceLocation.position) < queueDistance * (trucksInQueue.Count + 1) && !unit.GetComponent<SupplyTruck>().inQueue && unit.GetComponent<SupplyTruck>().heldResources == 0)
                {
                    trucksInQueue.Add(unit.GetComponent<SupplyTruck>());
                    unit.GetComponent<SupplyTruck>().inQueue = true;
                }
            }
        }
    }

    SupplyTruck FindTruckToLoad()
    {
        SupplyTruck truckToLoad = null;
        //foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        //{
        //    if (unit.GetComponent<SupplyTruck>() && Vector3.Distance(unit.transform.position, entranceLocation.position) <= truckFindRadius)
        //    {
        //        if (!truckToLoad || Vector3.Distance(unit.transform.position, entranceLocation.position) < Vector3.Distance(truckToLoad.transform.position, entranceLocation.position))
        //            truckToLoad = unit.GetComponent<SupplyTruck>();
        //    }
        //}
        if (trucksInQueue.Count > 0)
            truckToLoad = trucksInQueue[0];
        return truckToLoad;
    }


    void OnDestroy()
    {
        PlayerTroopManager.instance.supplyYards.Remove(this);
        foreach (SupplyTruck truck in trucksInQueue)
        {
            truck.inQueue = false;
        }
    }
}
