using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmySelection : MonoBehaviour
{
    public Army army;
    private void Start()
    {

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