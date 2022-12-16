using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBuildingMenu : MonoBehaviour
{
    public HolographicBuilding holoBuilding;
    public void placeNewBuilding()
    {
        //if (HQBuilding.GetSupplies(holoBuilding.buildingToSpawn.building.buildCost, PlayerCam.playerArmy))
        {
            PlayerCam.instance.newBuilding = Instantiate(holoBuilding);
            PlayerCam.instance.inBuildMode = true;
        }
    }
}
