using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQMenu : MonoBehaviour
{
    HQBuilding hq;

    private void Start()
    {
        foreach (HQBuilding hq_ in PlayerTroopManager.instance.HQs)
        {
            if (hq_.army == PlayerCam.playerArmy)
            {
                hq = hq_;
                break;
            }
        }
    }

    public void BuildNewTruck()
    {
        if (hq.supplies >= hq.truckCost)
            hq.StartBuilding();
    }
    public void RemoveTruckFromQueue()
    {
        hq.RemoveBuilding();
    }
}
