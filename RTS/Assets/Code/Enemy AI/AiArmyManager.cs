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
    public List<Factory> hFactories = new List<Factory>();

    [Header("Army production")]
    public ArmySquads squads;
    public int squadCap;
    public Squad squadToBuild;
    public bool isBuilding;

    private void Start()
    {
        ecoRating = Mathf.Clamp(ecoRating + Random.Range(-1f, 1f), 1, 10);
        agroRating = Mathf.Clamp(agroRating + Random.Range(-1f, 1f), 1, 10);
        costMod = Mathf.Clamp(costMod + Random.Range(-0.5f, 0.5f), 0.5f, 5);
        for (int i = 0; i < supplySets.Count; i++)
        {
            supplySets[i].optimalEco += Mathf.RoundToInt((16 + 2 * (Vector3.Distance(supplySets[i].depot.transform.position, supplySets[i].yard.transform.position) / 15f)) / 7.5f);
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
        for (int i = squadCap; i < squads.squads.Length; i++)
        {
            yield return new WaitForSeconds((11f - agroRating) * 90f);
            ecoRating = Mathf.Clamp(ecoRating + Random.Range(0.1f, 0.75f), 1, 10);
            squadCap++;
            isBuilding = false;
        }
        Debug.Log("Max difficulty");
    }

    void SetNewSquad()
    {
        int i = Random.Range(0, squadCap);
        squadToBuild = new Squad(squads.squads[i]);
        isBuilding = true;
    }

    private void FixedUpdate()
    {
        CheckCurrentEco();
        if (CurrentEcoScore() < ecoRating / 10)
        {
            if (HQBuilding.GetSupplies(hq.troops[0].cost, army))
                hq.BuildNewTroop(0);
        }
        else if (!isBuilding)
        {
            SetNewSquad();
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
}

[System.Serializable]
public class SupplySet
{
    public SupplyYard yard;
    public SupplyDepot depot;
    public int optimalEco;
}