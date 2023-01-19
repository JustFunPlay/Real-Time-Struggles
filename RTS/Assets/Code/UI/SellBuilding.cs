using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellBuilding : MonoBehaviour
{
    public void Sell()
    {
        if (PlayerCam.instance.selectedUnits.Count == 1 && PlayerCam.instance.selectedUnits[0].GetComponent<Building>() && PlayerCam.instance.selectedUnits[0].type != UnitType.HeadQuarters)
        {
            PlayerCam.instance.selectedUnits[0].GetComponent<Building>().Sell();
        }
    }
}
