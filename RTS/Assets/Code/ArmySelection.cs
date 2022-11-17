using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmySelection : MonoBehaviour
{
    public Army army;
    private void Start()
    {
        UnitBase[] units = FindObjectsOfType<UnitBase>();
        foreach (UnitBase unit in units)
        {
            if (unit.army == Army.Blue)
                unit.GetComponent<MeshRenderer>().material.color = Color.blue;
            else if(unit.army == Army.Cyan)
                unit.GetComponent<MeshRenderer>().material.color = Color.cyan;
            else if (unit.army == Army.Green)
                unit.GetComponent<MeshRenderer>().material.color = Color.green;
            else if (unit.army == Army.Magenta)
                unit.GetComponent<MeshRenderer>().material.color = Color.magenta;
            else if (unit.army == Army.Red)
                unit.GetComponent<MeshRenderer>().material.color = Color.red;
            else if (unit.army == Army.Yellow)
                unit.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        
    }
}

[System.Serializable]
public enum Army
{
    Blue,
    Cyan,
    Green,
    Magenta,
    Red,
    Yellow
}