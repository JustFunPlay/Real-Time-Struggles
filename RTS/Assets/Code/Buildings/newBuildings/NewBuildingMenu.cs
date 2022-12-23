using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBuildingMenu : MonoBehaviour
{
    public HolographicBuilding holoBuilding;
    public Button button;
    public UnitType requiredBuildingType;

    private void FixedUpdate()
    {
        bool hasBuilding = false;
        foreach (UnitBase unit in PlayerTroopManager.instance.playerUnits)
        {
            if (unit.type == requiredBuildingType)
                hasBuilding = true;
        }
        if (HQBuilding.HasPower(holoBuilding.buildingToSpawn.building.powerCost, PlayerCam.playerArmy) && hasBuilding)
            button.interactable = true;
        else
            button.interactable = false;
    }

    public void placeNewBuilding()
    {
            PlayerCam.instance.newBuilding = Instantiate(holoBuilding);
            PlayerCam.instance.inBuildMode = true;
    }
}
