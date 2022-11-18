using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    public Army army;
    bool isSelected;
    public MeshRenderer showSelected;

    private void Start()
    {
        if (army == Army.Blue)
            GetComponent<MeshRenderer>().material.color = Color.blue;
        else if (army == Army.Cyan)
            GetComponent<MeshRenderer>().material.color = Color.cyan;
        else if (army == Army.Green)
            GetComponent<MeshRenderer>().material.color = Color.green;
        else if (army == Army.Magenta)
            GetComponent<MeshRenderer>().material.color = Color.magenta;
        else if (army == Army.Red)
            GetComponent<MeshRenderer>().material.color = Color.red;
        else if (army == Army.Yellow)
            GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    public void OnSelected(PlayerCam cam)
    {
        cam.selectedUnits.Add(this);
        isSelected = true;
        showSelected.material.color = Color.green;
    }
    public void OnDeselected(PlayerCam cam)
    {
        cam.selectedUnits.Remove(this);
        isSelected = false;
        showSelected.material.color = Color.white;
    }
}
