using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQMenu : MonoBehaviour
{
    //HQBuilding hq;

    //private void Start()
    //{
    //    foreach (HQBuilding hq_ in PlayerTroopManager.instance.HQs)
    //    {
    //        if (hq_.army == PlayerCam.playerArmy)
    //        {
    //            hq = hq_;
    //            break;
    //        }
    //    }
    //}

    public void BuildNewTruck(int index)
    {
        PlayerCam.instance.selectedUnits[0].GetComponent<Factory>().BuildNewTroop(index);
    }
}
