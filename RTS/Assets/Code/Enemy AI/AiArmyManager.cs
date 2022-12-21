using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiArmyManager : MonoBehaviour
{
    public Army army;

    [Header("Difficulty configuration")]
    [Tooltip("Defines how efficient the army's resource aquisition is"), Range(1, 10)] public float ecoRating;
    /// <summary>
    /// Defines how efficient the army's resource aquisition is
    /// </summary>
    [Tooltip("Defines how quickly the army is to attack another army's base"), Range(1, 10)] public float agroRating;
    /// <summary>
    /// Defines how quickly the army is to attack another army's base
    /// </summary>
    [Tooltip("Modifies the amount of resources neccesary for the army to build more troops"), Range(0.5f, 5)] public float costMod;
    /// <summary>
    /// Modifies the amount of resources neccesary for the army to build more troops
    /// </summary>

    [Header("Units")]
    public HQBuilding hq;
    public List<SupplySet> supplySets = new List<SupplySet>();
    public List<SupplyTruck> supplyTrucks = new List<SupplyTruck>();
    public List<Factory> lFactories = new List<Factory>();
    int l = 0;
    public List<Factory> hFactories = new List<Factory>();
    int h = 0;
    public List<ActiveSquad> activeSquads = new List<ActiveSquad>();

    [Header("Army production")]
    public ArmySquads squads;
    public int squadCap;
    public int minSquad;
    public Squad squadToBuild;
    public bool isBuilding;
    public Transform gatheringPoint;

    private void Start()
    {
        ecoRating = Mathf.Clamp(ecoRating + Random.Range(-1f, 1f), 1, 10);
        agroRating = Mathf.Clamp(agroRating + Random.Range(-1f, 1f), 1, 10);
        costMod = Mathf.Clamp(costMod + Random.Range(-0.5f, 0.5f), 0.5f, 5);
        for (int i = 0; i < supplySets.Count; i++)
        {
            supplySets[i].optimalEco += Mathf.CeilToInt(12/(60/(12 + 2 * (Vector3.Distance(supplySets[i].depot.transform.position, supplySets[i].yard.transform.position) / 16f))));
        }
        for (int i = 0; i < hq.troops.Length; i++)
        {
            hq.troops[i].cost = (int)(hq.troops[i].cost * costMod);
        }
        for (int f = 0; f < lFactories.Count; f++)
        {
            for (int t = 0; t < lFactories[f].troops.Length; t++)
            {
                lFactories[f].troops[t].cost = (int)(lFactories[f].troops[t].cost * costMod);
            }
        }
        for (int f = 0; f < hFactories.Count; f++)
        {
            for (int t = 0; t < hFactories[f].troops.Length; t++)
            {
                hFactories[f].troops[t].cost = (int)(hFactories[f].troops[t].cost * costMod);
            }
        }
        StartCoroutine(ProgressDifficulty());
    }

    IEnumerator ProgressDifficulty()
    {
        squadCap = 3;
        minSquad = 0;
        yield return new WaitForSeconds(120f);
        while (squadCap < squads.squads.Length)
        {
            yield return new WaitForSeconds((11f - agroRating) * 30f + 60f);
            ecoRating = Mathf.Clamp(ecoRating + Random.Range(0.1f, 0.75f), 1, 10);
            squadCap++;
        }
        while (minSquad < squads.squads.Length / 2)
        {
            yield return new WaitForSeconds((11f - agroRating) * 30f + 60f);
            minSquad++;
        }
        Debug.Log("Max difficulty");
    }

    void SetNewSquad()
    {
        isBuilding = true;
        int i = Random.Range(minSquad, squadCap);
        squadToBuild = new Squad(squads.squads[i]);
        StartCoroutine(BuildNewSquad());
    }

    IEnumerator BuildNewSquad()
    {
        activeSquads.Add(new ActiveSquad());
        while (squadToBuild.humvees + squadToBuild.apcs + squadToBuild.tanks + squadToBuild.howitzers + squadToBuild.snipers > 0)
        {
            for (int i = 0; i < squadToBuild.humvees; i++)
            {
                if (HQBuilding.GetSupplies(lFactories[0].troops[0].cost, army))
                {
                    if (l >= lFactories.Count)
                        l = 0;
                    lFactories[l].BuildNewTroop(0);
                    l++;
                    squadToBuild.humvees--;
                    i--;
                }
            }
            for (int i = 0; i < squadToBuild.apcs; i++)
            {
                if (HQBuilding.GetSupplies(lFactories[0].troops[1].cost, army))
                {
                    if (l >= lFactories.Count)
                        l = 0;
                    lFactories[l].BuildNewTroop(1);
                    l++;
                    squadToBuild.apcs--;
                    i--;
                }
            }
            for (int i = 0; i < squadToBuild.tanks; i++)
            {
                if (HQBuilding.GetSupplies(hFactories[0].troops[0].cost, army))
                {
                    if (h >= lFactories.Count)
                        h = 0;
                    hFactories[h].BuildNewTroop(0);
                    h++;
                    squadToBuild.tanks--;
                    i--;
                }
            }
            for (int i = 0; i < squadToBuild.howitzers; i++)
            {
                if (HQBuilding.GetSupplies(hFactories[0].troops[1].cost, army))
                {
                    if (h >= lFactories.Count)
                        h = 0;
                    hFactories[h].BuildNewTroop(1);
                    h++;
                    squadToBuild.howitzers--;
                    i--;
                }
            }
            for (int i = 0; i < squadToBuild.snipers; i++)
            {
                if (HQBuilding.GetSupplies(hFactories[0].troops[2].cost, army))
                {
                    if (h >= lFactories.Count)
                        h = 0;
                    hFactories[h].BuildNewTroop(2);
                    h++;
                    squadToBuild.snipers--;
                    i--;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        while (CheckIfBuilding())
        {
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(ManageSquad(activeSquads[activeSquads.Count-1]));
        yield return new WaitForSeconds(3f);
        isBuilding = false;
    }

    IEnumerator ManageSquad(ActiveSquad squad)
    {
        yield return new WaitForSeconds(15f);
        while (squad.squadMemebers.Count > 0)
        {
            squad = CheckSquadAliveness(squad);
            if (!SquadInCombat(squad.squadMemebers.ToArray()))
            {
                Vector3 squadOrigin = SquadOrigin(squad.squadMemebers.ToArray());
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
                UnitBase target = closestUnits[t];
                float tInCombat = 0;
                while (target && squad.squadMemebers.Count > 0 && tInCombat < 1.5f && Vector3.Distance(SquadOrigin(squad.squadMemebers.ToArray()), target.GetClosestTargetingPoint(SquadOrigin(squad.squadMemebers.ToArray()))) > SquadRange(squad.squadMemebers.ToArray()))
                {
                    squad = CheckSquadAliveness(squad);
                    Vector3 targetPoint = target.transform.position + ((SquadOrigin(squad.squadMemebers.ToArray()) - target.transform.position).normalized * (SquadRange(squad.squadMemebers.ToArray()) - 5f));
                    Formations.instance.SetFormation(squad.squadMemebers.ToArray(), targetPoint);
                    if (SquadInCombat(squad.squadMemebers.ToArray()))
                        tInCombat += 0.1f;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FixedUpdate()
    {
        CheckCurrentEco();
        CheckSquads();
        if (CurrentEcoScore() < ecoRating / 10f)
        {
            if (HQBuilding.GetSupplies(hq.troops[0].cost, army))
                hq.BuildNewTroop(0);
        }
        else if (!isBuilding)
        {
            isBuilding = true;
            SetNewSquad();
        }
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army == army && unit.GetComponent<TroopMovement>())
            {
                if (CheckIfAssigned(unit) == false)
                {
                    activeSquads[activeSquads.Count - 1].squadMemebers.Add(unit.GetComponent<TroopMovement>());
                    Formations.instance.SetFormation(activeSquads[activeSquads.Count - 1].squadMemebers.ToArray(), gatheringPoint.position);
                }
            }
        }
    }
    void CheckCurrentEco()
    {
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army == army)
            {
                if (unit.GetComponent<SupplyTruck>())
                {
                    if (!supplyTrucks.Contains(unit.GetComponent<SupplyTruck>()))
                    {
                        supplyTrucks.Add(unit.GetComponent<SupplyTruck>());
                        
                    }
                }
            }
        }
        for (int i = supplySets.Count - 1; i >= 0; i--)
        {
            if (supplySets[i].depot == null)
                supplySets.RemoveAt(i);
        }
        int t = 0;
        for (int set = 0; set < supplySets.Count; set++)
        {
            for (int i = 0; i < supplySets[set].optimalEco; i++)
            {
                if (t == supplyTrucks.Count)
                    break;
                if (supplyTrucks[t])
                {
                    supplyTrucks[t].assignedYard = supplySets[set].yard;
                    supplyTrucks[t].assignedDepot = supplySets[set].depot;
                    if (!supplyTrucks[t].supplying && !supplyTrucks[t].inQueue)
                        supplyTrucks[t].CheckToAutomate();
                    t++;
                }
                else
                    supplyTrucks.RemoveAt(t);

            }
        }
    }
    void CheckSquads()
    {
        for (int i = activeSquads.Count - 1; i >= 0; i--)
        {
            for (int u = activeSquads[i].squadMemebers.Count - 1; u >= 0; u--)
            {
                if (activeSquads[i].squadMemebers[u] == null)
                    activeSquads[i].squadMemebers.RemoveAt(u);
            }
            if (activeSquads[i].squadMemebers.Count == 0 && i != activeSquads.Count - 1)
                activeSquads.RemoveAt(i);
        }
    }
    ActiveSquad CheckSquadAliveness(ActiveSquad squad)
    {
        for (int i = 0; i < squad.squadMemebers.Count; i++)
        {
            if (squad.squadMemebers[i] == null)
            {
                squad.squadMemebers.RemoveAt(i);
                i--;
            }
        }
        return squad;
    }
    float CurrentEcoScore()
    {
        int eco = 0;
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army == army)
            {
                if (unit.GetComponent<SupplyTruck>())
                    eco++;
                else if (unit.GetComponent<HQBuilding>())
                {
                    if (unit.GetComponent<HQBuilding>().isBuilding)
                        eco++;
                    eco += unit.GetComponent<HQBuilding>().queue.Count;
                }
            }
        }
        int optimalEco = 0;
        for (int i = 0; i < supplySets.Count; i++)
        {
            optimalEco += supplySets[i].optimalEco;
        }
        float ecoScore = (float)eco / (float)optimalEco;
        Debug.Log(eco + ", " + optimalEco + ", " + ecoScore.ToString());
        return ecoScore;
    }
    bool CheckIfBuilding()
    {
        for (int i = 0; i < lFactories.Count; i++)
        {
            if (lFactories[i].isBuilding)
                return true;
        }
        for (int i = 0; i < hFactories.Count; i++)
        {
            if (hFactories[i].isBuilding)
                return true;
        }
        return false;
    }
    bool CheckIfAssigned(UnitBase unit)
    {
        TroopMovement troop;
        if (unit.GetComponent<SupplyTruck>())
            return true;
        else
        {
            troop = unit.GetComponent<TroopMovement>();
            for (int i = 0; i < activeSquads.Count; i++)
            {
                if (activeSquads[i].squadMemebers.Contains(troop))
                    return true;
            }
        }
        return false;
    }
    
    Vector3 SquadOrigin(TroopMovement[] troops)
    {
        Vector3 origin = new Vector3();
        for (int i = 0; i < troops.Length; i++)
        {
            origin += troops[i].transform.position;
        }
        origin /= troops.Length;
        return origin;
    }
    bool SquadInCombat(TroopMovement[] troops)
    {
        for (int i = 0; i < troops.Length; i++)
        {
            if (troops[i].GetComponent<Tank>() && troops[i].GetComponent<Tank>().target)
                return true;
            else if (troops[i].GetComponent<Howitzer>() && troops[i].GetComponent<Howitzer>().target)
                return true;
        }
        return false;
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
    float SquadRange(TroopMovement[] troops)
    {
        float avarageRange = 0;
        for (int i = 0; i < troops.Length; i++)
        {
            if (troops[i].GetComponent<Tank>())
                avarageRange += troops[i].GetComponent<Tank>().range;
            else if (troops[i].GetComponent<Howitzer>())
                avarageRange += troops[i].GetComponent<Howitzer>().range;
        }
        avarageRange /= troops.Length;
        return avarageRange;
    }
    int[] CalculateTargetWeight(Vector3 origin, UnitBase[] targets)
    {
        int[] weights = new int[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 30f)
                weights[i] = 50;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 60f)
                weights[i] = 20;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 90f)
                weights[i] = 10;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 120f)
                weights[i] = 5;
            else if (Vector3.Distance(origin, targets[i].GetClosestTargetingPoint(origin)) <= 150f)
                weights[i] = 2;
            else
                weights[i] = 1;
        }
        return weights;
    }
}

[System.Serializable]
public class SupplySet
{
    public SupplyYard yard;
    public SupplyDepot depot;
    public int optimalEco;
}

[System.Serializable]
public class ActiveSquad
{
    public List<TroopMovement> squadMemebers = new List<TroopMovement>();
}