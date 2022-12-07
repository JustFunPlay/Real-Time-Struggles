using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Army Squads", menuName = "ScriptableObjects/NewArmySquads")]
public class ArmySquads : ScriptableObject
{
    public Squad[] squads;
}

[System.Serializable]
public class Squad
{
    public string name;
    public int humvees;
    public int apcs;
    public int tanks;
    public int howitzers;
    public int snipers;

    public Squad()
    {
        name = null;
        humvees = 0;
        apcs = 0;
        tanks = 0;
        howitzers = 0;
        snipers = 0;
    }
    public Squad(Squad newSquad)
    {
        name = newSquad.name;
        humvees = newSquad.humvees;
        apcs = newSquad.apcs;
        tanks = newSquad.tanks;
        howitzers = newSquad.howitzers;
        snipers = newSquad.snipers;
    }
    public Squad(string newName, int numberOfHumvees, int numberOfApcs, int numberOfTanks, int numberOfHowitzers, int numberOfSnipers)
    {
        name = newName;
        humvees = numberOfHumvees;
        apcs = numberOfApcs;
        tanks = numberOfTanks;
        howitzers = numberOfHowitzers;
        snipers = numberOfSnipers;
    }
}