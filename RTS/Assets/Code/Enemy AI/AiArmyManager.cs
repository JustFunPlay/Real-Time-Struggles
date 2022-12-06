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

    public HQBuilding hq;
    public List<SupplySet> supplySets = new List<SupplySet>();
    public List<SupplyTruck> supplyTrucks = new List<SupplyTruck>();


    private void Start()
    {
        //foreach (HQBuilding hq_ in PlayerTroopManager.instance.HQs)
        //{
        //    if (hq_.army == army)
        //        hq = hq_;
        //}
        for (int i = 0; i < supplySets.Count; i++)
        {
            supplySets[i].optimalEco += Mathf.RoundToInt((16 + 2 * (Vector3.Distance(supplySets[i].depot.transform.position, supplySets[i].yard.transform.position) / 15f)) / 7.5f);
        }
        for (int i = 0; i < hq.troops.Length; i++)
        {
            hq.troops[i].cost = (int)(hq.troops[i].cost * costMod);
        }
    }

    private void FixedUpdate()
    {
        CheckCurrentEco();
        if (CurrentEcoScore() < ecoRating / 10)
        {
            if (HQBuilding.GetSupplies((int)(hq.troops[0].cost * costMod), army))
                hq.BuildNewTroop(0);
        }
        else
        {

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
                supplyTrucks[t].assignedYard = supplySets[set].yard;
                supplyTrucks[t].assignedDepot = supplySets[set].depot;
                if (!supplyTrucks[t].supplying && !supplyTrucks[t].inQueue)
                    supplyTrucks[t].CheckToAutomate();
                t++;

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
        //Debug.Log(eco + ", " + optimalEco + ", " + ecoScore.ToString());
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