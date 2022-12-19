using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealButton : MonoBehaviour
{
    public void HealingButton()
    {
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == PlayerCam.playerArmy)
                hq.HealAllBuildings();
        }
    }
}
