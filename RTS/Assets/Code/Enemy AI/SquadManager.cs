using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    List<TroopMovement> squad = new List<TroopMovement>();
    Army army;
    int ogSquadSize;
    AiArmyManager armyManager;
    float squadRange;
    float moveDelay;

    bool isMoving;

    bool gettingRepaired;
    RepairBay currentRepairBay;

    UnitBase moveTarget;

    bool inCombat;
    float timeInCombat;

    public void AssembleSquad(List<TroopMovement> newSquad, Army army_, AiArmyManager aiArmy)
    {
        squad = newSquad;
        army = army_;
        armyManager = aiArmy;
        ogSquadSize = squad.Count;
        CalculateSquadRange();
        moveDelay = 15f;
        StartCoroutine(ManageSquad());
    }

    IEnumerator ManageSquad()
    {
        while (squad.Count > 0)
        {
            CheckSquadAliveness();
            if (gettingRepaired && currentRepairBay)
            {
                List<TroopMovement> ready = new List<TroopMovement>();
                foreach (TroopMovement troop in squad)
                {
                    if (troop.currentHP == troop.maxHP && !ready.Contains(troop))
                        ready.Add(troop);
                    else if (troop.canBeSeclected && !troop.inQueue && !ready.Contains(troop))
                        troop.MoveToPosition(currentRepairBay.entranceLocation.position);
                }
                Formations.instance.SetFormation(ready.ToArray(), armyManager.altWaitArea.position);
                if (ready.Count == squad.Count)
                {
                    gettingRepaired = false;
                    moveDelay = 5f;
                }
            }
            else if (!isMoving && !inCombat)
            {
                if (moveDelay > 0)
                    moveDelay -= 0.5f;
                else
                    CalculateNewMoveTo();
            }
            else if (isMoving && moveTarget)
            {
                Vector3 targetPoint = moveTarget.transform.position + ((SquadOrigin() - moveTarget.transform.position).normalized * (squadRange - 5f));
                if (Vector3.Distance(targetPoint, SquadOrigin()) > 5f)
                    Formations.instance.SetFormation(squad.ToArray(), targetPoint);
                if (SquadInCombat() && !inCombat)
                    timeInCombat += 0.5f;
            }
            if (timeInCombat >= 1 && !inCombat)
            {
                inCombat = true;
                timeInCombat = 0;
            }
            if (inCombat)
            {
                moveTarget = InCombatTarget();
                if (moveTarget && Vector3.Distance(SquadOrigin(), moveTarget.transform.position) > squadRange * 2)
                {
                    moveTarget = null;
                }
                if (!moveTarget)
                {
                    inCombat = false;
                    int i = Random.Range(0, 3);
                    if (armyManager.repairBays.Count > 0 && i == 0)
                        FindRepairBay();
                    else
                        isMoving = false;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(gameObject);
    }

    void CheckSquadAliveness()
    {
        bool lostUnit = false;
        for (int i = 0; i < squad.Count; i++)
        {
            if (squad[i] == null)
            {
                squad.RemoveAt(i);
                i--;
                lostUnit = true;
                CalculateSquadRange();
            }
        }
        if (lostUnit == true && squad.Count * 2 <= ogSquadSize && armyManager.repairBays.Count > 0)
        {
            int retreat = Random.Range(squad.Count, ogSquadSize + 1);
            if (retreat * 2 <= ogSquadSize)
            {
                FindRepairBay();
            }

        }
    }

    void CalculateNewMoveTo()
    {
        Vector3 squadOrigin = SquadOrigin();
        UnitBase[] closestUnits = ClosestTargets(squadOrigin);
        int[] weights = CalculateTargetWeight(squadOrigin, closestUnits);
        int r = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            r += weights[i];
        }
        int w = Random.Range(0, r);
        int t = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if (w < weights[i])
            {
                t = i;
                break;
            }
            else
                w -= weights[i];
        }
        moveTarget = closestUnits[t];
        moveDelay = Random.Range(1f, 6f);
        timeInCombat = 0;
        isMoving = true;
        inCombat = false;
    }

    void FindRepairBay()
    {
        RepairBay repairBay = null;
        foreach (RepairBay bay in armyManager.repairBays)
        {
            if (!repairBay || Vector3.Distance(SquadOrigin(), bay.transform.position) < Vector3.Distance(SquadOrigin(), repairBay.transform.position))
            {
                repairBay = bay;
            }
        }
        currentRepairBay = repairBay;
        gettingRepaired = true;
        isMoving = false;
        inCombat = false;
        timeInCombat = 0;
        foreach (TroopMovement unit in squad)
        {
            unit.MoveToPosition(repairBay.entranceLocation.position);
        }
    }

    Vector3 SquadOrigin()
    {
        Vector3 origin = new Vector3();
        for (int i = 0; i < squad.Count; i++)
        {
            origin += squad[i].transform.position;
        }
        origin /= squad.Count;
        return origin;
    }
    bool SquadInCombat()
    {
        for (int i = 0; i < squad.Count; i++)
        {
            if (squad[i].GetComponent<Tank>() && squad[i].GetComponent<Tank>().target)
                return true;
            else if (squad[i].GetComponent<Howitzer>() && squad[i].GetComponent<Howitzer>().target)
                return true;
            else if (squad[i].GetComponent<Humvee>() && squad[i].GetComponent<Humvee>().target)
                return true;
        }
        return false;
    }
    UnitBase InCombatTarget()
    {
        UnitBase target = null;
        foreach (TroopMovement troop in squad)
        {
            if (troop.GetComponent<Tank>() && troop.GetComponent<Tank>().target && (!target || Vector3.Distance(SquadOrigin(), troop.GetComponent<Tank>().target.transform.position) > Vector3.Distance(SquadOrigin(), target.transform.position)))
                target = troop.GetComponent<Tank>().target;
            else if (troop.GetComponent<Howitzer>() && troop.GetComponent<Howitzer>().target && (!target || Vector3.Distance(SquadOrigin(), troop.GetComponent<Howitzer>().target.transform.position) > Vector3.Distance(SquadOrigin(), target.transform.position)))
                target = troop.GetComponent<Howitzer>().target;
            else if (troop.GetComponent<Humvee>() && troop.GetComponent<Humvee>().target && (!target || Vector3.Distance(SquadOrigin(), troop.GetComponent<Humvee>().target.transform.position) > Vector3.Distance(SquadOrigin(), target.transform.position)))
                target = troop.GetComponent<Humvee>().target;
        }
        return target;
    }
    UnitBase[] ClosestTargets(Vector3 origin)
    {
        List<UnitBase> closestUnits = new List<UnitBase>();
        for (int i = 0; i < 5; i++)
        {
            UnitBase closestUnit = null;
            foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
            {
                if (unit.army != army && !closestUnits.Contains(unit) && (closestUnit == null || Vector3.Distance(origin, unit.GetClosestTargetingPoint(origin)) < Vector3.Distance(origin, closestUnit.GetClosestTargetingPoint(origin))))
                {
                    closestUnit = unit;
                }
            }
            if (closestUnit)
                closestUnits.Add(closestUnit);
        }
        return closestUnits.ToArray();
    }
    void CalculateSquadRange()
    {
        float avarageRange = 0;
        for (int i = 0; i < squad.Count; i++)
        {
            if (squad[i].GetComponent<Tank>())
                avarageRange += squad[i].GetComponent<Tank>().range;
            else if (squad[i].GetComponent<Howitzer>())
                avarageRange += squad[i].GetComponent<Howitzer>().range;
            else if (squad[i].GetComponent<Humvee>())
                avarageRange += squad[i].GetComponent<Humvee>().range;
        }
        avarageRange /= squad.Count;
        squadRange = avarageRange;
    }
    int[] CalculateTargetWeight(Vector3 origin, UnitBase[] targets)
    {
        int[] weights = new int[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 30f)
                weights[i] = 100;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 60f)
                weights[i] = 50;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 90f)
                weights[i] = 20;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 120f)
                weights[i] = 10;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 150f)
                weights[i] = 5;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 200f)
                weights[i] = 2;
            else
                weights[i] = 1;
        }
        return weights;
    }
}
