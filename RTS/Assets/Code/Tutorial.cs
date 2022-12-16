using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int tutorialStage;

    private void FixedUpdate()
    {
        switch(tutorialStage)
        {
            case 0:
                if (PlayerTroopManager.instance.playerUnits.Count == 3)
                {
                    //
                }
                break;
            case 1:
                int t = 0;
                foreach (UnitBase unit in PlayerTroopManager.instance.playerUnits)
                {
                    if (unit.GetComponent<SupplyTruck>())
                        t++;
                }
                if (t >= 3)
                {

                }
                break;
            case 2:

                break;

        }
        
    }
}
