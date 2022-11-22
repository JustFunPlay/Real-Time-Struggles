using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTroopManager : MonoBehaviour
{
    public static PlayerTroopManager instance;
    public List<UnitBase> playerUnits = new List<UnitBase>();
    private void Start()
    {
        instance = this;
    }
}
