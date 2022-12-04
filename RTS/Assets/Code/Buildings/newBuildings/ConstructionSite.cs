using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    public Building building;

    SupplyTruck truck;
    private void Start()
    {
        Invoke("FinishConstruction", 5f);
    }
    public void FinishConstruction()
    {
        Building newBuilding = Instantiate(building, transform.position, transform.rotation);
        newBuilding.AddedUnit(PlayerCam.playerArmy);
        Destroy(gameObject);
    }
}
