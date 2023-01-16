using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RepairBay : Building
{
    [Header("Troop repair")]
    public float troopFindRadius;
    public float repairDuration;
    public float repairLockout;
    public int repairAmount;
    public int repairCost;

    [Header("Navigation")]
    public Transform entranceLocation;
    public Transform repairPoint;
    public Transform exitLocation;
    public Transform entranceGate;
    public Transform exitGate;

    [Header("Queue")]
    public float queueDistance;
    List<TroopMovement> troopsInQueue = new List<TroopMovement>();

    TroopMovement troopInAction;

    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        StartCoroutine(CheckForBrokenTroop());
    }
    IEnumerator CheckForBrokenTroop()
    {
        while (!troopInAction)
        {
            TroopMovement troop = FindTroopToRepair();
            troopInAction = troop;
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(RepairTroop());
    }

    IEnumerator RepairTroop()
    {
        troopInAction.canBeSeclected = false;
        troopInAction.OnDeselected();
        troopInAction.inBuilding = true;
        troopInAction.inQueue = false;
        troopsInQueue.Remove(troopInAction);
        troopInAction.MoveToPosition(repairPoint.position);
        for (int i = 0; i < 10; i++)
        {
            entranceGate.transform.Rotate(0, 0, 9);
            if (i == 3)
                entranceGate.GetComponent<NavMeshObstacle>().enabled = false;
            yield return new WaitForSeconds(0.075f);
        }
        while (Vector3.Distance(troopInAction.transform.position, repairPoint.position) > 5f)
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

        while (troopInAction.currentHP < troopInAction.maxHP && HQBuilding.GetSupplies(repairCost, army))
        {
            troopInAction.currentHP = Mathf.Clamp(troopInAction.currentHP + repairAmount, 0, troopInAction.maxHP);
            HQBuilding.ChangeSupplies(-repairCost, army);
            yield return new WaitForSeconds(repairDuration);
        }

        troopInAction.MoveToPosition(exitLocation.position);
        for (int i = 0; i < 10; i++)
        {
            exitGate.transform.Rotate(0, 0, 9);
            if (i == 3)
                exitGate.GetComponent<NavMeshObstacle>().enabled = false;
            yield return new WaitForSeconds(0.075f);
        }
        while (Vector3.Distance(troopInAction.transform.position, exitLocation.position) > 5f)
        {
            troopInAction.MoveToPosition(exitLocation.position);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 10; i++)
        {
            exitGate.transform.Rotate(0, 0, -9);
            if (i == 6)
                exitGate.GetComponent<NavMeshObstacle>().enabled = true;
            yield return new WaitForSeconds(0.075f);
        }
        if (troopInAction.TryGetComponent(out SupplyTruck truck))
            truck.CheckToAutomate();
        troopInAction.inBuilding = false;
        troopInAction.canBeSeclected = true;
        troopInAction = null;
        yield return new WaitForSeconds(repairLockout);
        StartCoroutine(CheckForBrokenTroop());
    }

    private void FixedUpdate()
    {
        for (int i = troopsInQueue.Count - 1; i >= 0; i--)
        {
            if (troopsInQueue[i])
                troopsInQueue[i].MoveToPosition(entranceLocation.position + (i * queueDistance * (troopsInQueue[i].transform.position - entranceLocation.position).normalized));
            else
                troopsInQueue.RemoveAt(i);
        }

        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army == army && unit.GetComponent<TroopMovement>())
            {
                TroopMovement troop = unit.GetComponent<TroopMovement>();

                if (Vector3.Distance(troop.transform.position, entranceLocation.position) < queueDistance * (troopsInQueue.Count + 1) && !troop.inBuilding && !troop.inQueue && troop.currentHP < troop.maxHP)
                {
                    troopsInQueue.Add(troop);
                    troop.inQueue = true;
                }
            }
        }
    }
    TroopMovement FindTroopToRepair()
    {
        TroopMovement troopTorepair = null;
        if (troopsInQueue.Count > 0)
            troopTorepair = troopsInQueue[0];
        return troopTorepair;
    }
}
