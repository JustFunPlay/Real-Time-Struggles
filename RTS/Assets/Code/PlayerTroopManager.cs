using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTroopManager : MonoBehaviour
{
    public static PlayerTroopManager instance;
    public List<UnitBase> playerUnits = new List<UnitBase>();
    public List<UnitBase> allUnits = new List<UnitBase>();
    public List<SupplyYard> supplyYards = new List<SupplyYard>();
    public List<HQBuilding> HQs = new List<HQBuilding>();
    public List<SupplyTruck> supplyTrucks = new List<SupplyTruck>();
    public List<TroopMovement> troops = new List<TroopMovement>();
    public List<Building> buildings = new List<Building>();

    void Awake()
    {
        instance = this;
    }
}
