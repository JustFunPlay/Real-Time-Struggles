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
    public List<RepairBay> repairBays = new List<RepairBay>();

    [Header("Army production")]
    public ArmySquads squads;
    public int squadCap;
    public int minSquad;
    public Squad squadToBuild;
    public bool isBuilding;
    public Transform gatheringPoint;

    [Header("Squad management")]
    public SquadManager squadManager;
    public Transform altWaitArea;

    public void StartAI(float eco, float agro, float cost, int squad = 0)
    {
        army = hq.army;
        ecoRating = Mathf.Clamp(eco + Random.Range(-1f, 1f), 1, 10);
        agroRating = Mathf.Clamp(agro + Random.Range(-1f, 1f), 1, 10);
        costMod = Mathf.Clamp(cost + Random.Range(-0.5f, 0.5f), 0.5f, 5);
        squadCap = squad;
        for (int i = 0; i < supplySets.Count; i++)
        {
            supplySets[i].optimalEco += Mathf.CeilToInt(12f/(60f/(10f + 2f * (Vector3.Distance(supplySets[i].depot.transform.position, supplySets[i].yard.transform.position) / 15f))));
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
        StartCoroutine(ManageArmy());
    }

    IEnumerator ProgressDifficulty()
    {
        if (squadCap <= 0 || squadCap > squads.squads.Length)
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
        Debug.Log($"{army} is building a new {squadToBuild.name}");
        StartCoroutine(BuildNewSquad());
    }

    IEnumerator BuildNewSquad()
    {
        ActiveSquad emptySquad = new ActiveSquad();
        Debug.Log("a free squad slot is assigned by " + army);
        int totalSquadSize = squadToBuild.humvees + squadToBuild.apcs + squadToBuild.tanks + squadToBuild.howitzers + squadToBuild.snipers;
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
            foreach (TroopMovement troop in PlayerTroopManager.instance.troops)
            {
                if (troop.army == army && CheckIfAssigned(troop) == false && !emptySquad.squadMemebers.Contains(troop))
                {
                    emptySquad.squadMemebers.Add(troop);
                    Formations.instance.SetFormation(emptySquad.squadMemebers.ToArray(), gatheringPoint.position);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
        while (CheckIfBuilding())
        {
            foreach (TroopMovement troop in PlayerTroopManager.instance.troops)
            {
                if (troop.army == army && CheckIfAssigned(troop) == false && !emptySquad.squadMemebers.Contains(troop))
                {
                    emptySquad.squadMemebers.Add(troop);
                    Formations.instance.SetFormation(emptySquad.squadMemebers.ToArray(), gatheringPoint.position);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
        foreach (TroopMovement troop in PlayerTroopManager.instance.troops)
        {
            if (troop.army == army && CheckIfAssigned(troop) == false && !emptySquad.squadMemebers.Contains(troop))
            {
                emptySquad.squadMemebers.Add(troop);
                Formations.instance.SetFormation(emptySquad.squadMemebers.ToArray(), gatheringPoint.position);
            }
        }
        Debug.Log($"{army} has finished building a squad");
        SquadManager newSquad = Instantiate(squadManager);
        newSquad.AssembleSquad(emptySquad.squadMemebers, army, this);
        activeSquads.Add(emptySquad);
        yield return new WaitForSeconds(Random.Range(3f, 10f));
        isBuilding = false;
    }

    IEnumerator ManageArmy()
    {
        while (true)
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
            //foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
            //{
            //    if (unit.army == army && unit.GetComponent<TroopMovement>())
            //    {
            //        if (CheckIfAssigned(unit) == false)
            //        {
            //            activeSquads[activeSquads.Count - 1].squadMemebers.Add(unit.GetComponent<TroopMovement>());
            //            Formations.instance.SetFormation(activeSquads[activeSquads.Count - 1].squadMemebers.ToArray(), gatheringPoint.position);
            //        }
            //    }
            //}
            hq.HealAllBuildings();
            yield return new WaitForSeconds(0.5f);
        }
    }
    void CheckCurrentEco()
    {
        foreach (SupplyTruck truck in PlayerTroopManager.instance.supplyTrucks)
        {
            if (truck.army == army)
            {
                if (!supplyTrucks.Contains(truck))
                {
                    supplyTrucks.Add(truck);
                        
                }
            }
        }
        for (int i = supplySets.Count - 1; i >= 0; i--)
        {
            if (supplySets[i].depot == null)
            {
                for (int t = 0; t < supplySets[i].assignedTrucks.Count; t++)
                {
                    supplySets[i].assignedTrucks[t].assignedDepot = null;
                    supplySets[i].assignedTrucks[t].assignedYard = null;
                }
                supplySets.RemoveAt(i);
            }
        }
        for (int i = 0; i < supplyTrucks.Count; i++)
        {
            if (supplyTrucks[i] == null)
            {
                supplyTrucks.RemoveAt(i);
                i--;
            }
            else if (supplyTrucks[i].assignedDepot == null)
            {
                for (int s = 0; s < supplySets.Count; s++)
                {
                    if (supplySets[s].assignedTrucks.Count < supplySets[s].optimalEco || s == supplySets.Count - 1)
                    {
                        supplySets[s].assignedTrucks.Add(supplyTrucks[i]);
                        supplyTrucks[i].assignedYard = supplySets[s].yard;
                        supplyTrucks[i].assignedDepot = supplySets[s].depot;
                        supplyTrucks[i].CheckToAutomate();
                        break;
                    }
                }
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
            if (activeSquads[i].squadMemebers.Count == 0 && i != 0)
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
        //Debug.Log(eco + ", " + optimalEco + ", " + ecoScore.ToString());
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
}

[System.Serializable]
public class SupplySet
{
    public SupplyYard yard;
    public SupplyDepot depot;
    public int optimalEco;
    public List<SupplyTruck> assignedTrucks = new List<SupplyTruck>();
}

[System.Serializable]
public class ActiveSquad
{
    public List<TroopMovement> squadMemebers = new List<TroopMovement>();
}