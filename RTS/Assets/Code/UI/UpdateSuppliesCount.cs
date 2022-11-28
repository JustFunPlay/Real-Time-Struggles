using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSuppliesCount : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    HQBuilding hq;

    private void Start()
    {
        Invoke("FindHQ", 0.1f);
    }
    void FindHQ()
    {
        foreach (HQBuilding hq_ in PlayerTroopManager.instance.HQs)
        {
            if (hq_.army == PlayerCam.playerArmy)
            {
                hq = hq_;
                break;
            }
        }
        StartCoroutine(UpdateResources());
    }

    IEnumerator UpdateResources()
    {
        while (true)
        {
            text.text = "$" + hq.supplies.ToString();
            yield return new WaitForFixedUpdate();
        }
    }
}
