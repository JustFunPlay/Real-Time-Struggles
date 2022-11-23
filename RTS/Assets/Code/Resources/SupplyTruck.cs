using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyTruck : TroopMovement
{
    public SupplyYard assignedYard;
    public SupplyDepot assignedDepot;
    public int heldResources;

    public void CheckToAutomate()
    {
        if (assignedYard && assignedDepot)
        {
            if (heldResources > 0)
            {
                MoveToPosition(assignedDepot.entranceLocation.position);
            }
            else
            {
                MoveToPosition(assignedYard.entranceLocation.position);
            }
            Invoke("OnDeselected", 0.1f);
        }
    }
}
