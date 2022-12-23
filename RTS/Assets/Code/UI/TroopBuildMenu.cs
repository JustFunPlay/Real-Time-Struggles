using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopBuildMenu : MonoBehaviour
{
    public bool requiresTechCenter;
    public Button button;
    private void FixedUpdate()
    {
        if (requiresTechCenter)
        {
            bool hasTech = false;
            foreach (UnitBase unit in PlayerTroopManager.instance.playerUnits)
            {
                if (unit.type == UnitType.TechCenter)
                    hasTech = true;
            }
            if (hasTech)
                button.interactable = true;
            else
                button.interactable = false;
        }
    }

    public void BuildNewTroop(int index)
    {
        PlayerCam.instance.selectedUnits[0].GetComponent<Factory>()?.BuildNewTroop(index);
    }
}
