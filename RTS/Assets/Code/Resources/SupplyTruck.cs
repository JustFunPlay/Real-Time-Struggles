using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyTruck : TroopMovement
{
    public SupplyYard assignedYard;
    public SupplyDepot assignedDepot;
    public int heldResources;
    public GameObject supplyVisualizer;
    public ConstructionSite constructionSite;

    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        PlayerTroopManager.instance.supplyTrucks.Add(this);
    }
    public void CheckToAutomate()
    {
        if (constructionSite && assignedDepot)
        {
            if (heldResources >= (constructionSite.building.buildCost / constructionSite.building.requiredTrips))
                MoveToPosition(constructionSite.transform.position);
            else if (!HQBuilding.GetSupplies(constructionSite.building.buildCost / constructionSite.building.requiredTrips, army) && assignedYard)
            {
                if (heldResources > 0)
                    MoveToPosition(assignedDepot.entranceLocation.position);
                else
                    MoveToPosition(assignedYard.entranceLocation.position);
            }
            else
                MoveToPosition(assignedDepot.entranceLocation.position);
            Invoke("OnDeselected", 0.1f);
        }
        else if (constructionSite && heldResources >= (constructionSite.building.buildCost / constructionSite.building.requiredTrips))
        {
            MoveToPosition(constructionSite.transform.position);
            Invoke("OnDeselected", 0.1f);
        }
        else if (assignedYard && assignedDepot)
        {
            if (heldResources > 0)
                MoveToPosition(assignedDepot.entranceLocation.position);
            else
                MoveToPosition(assignedYard.entranceLocation.position);
            Invoke("OnDeselected", 0.1f);
        }
        else
            OnSelected();
    }
    public void CheckSupplies()
    {
        if (heldResources > 0)
            supplyVisualizer.SetActive(true);
        else
            supplyVisualizer.SetActive(false);
    }

    protected override void OnDestroy()
    {
        PlayerTroopManager.instance.supplyTrucks.Remove(this);
        base.OnDestroy();
    }
}
