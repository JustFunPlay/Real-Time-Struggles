using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int tutorialStage;
    public TutorialStage[] tutorialStages;
    public TroopMovement[] tutorialEnemies;
    public UnitBase[] outpostUnits;
    public AiArmyManager aiSetup;

    private void FixedUpdate()
    {
        switch(tutorialStage)
        {
            case 0:
                if (PlayerTroopManager.instance.playerUnits.Count == 3)
                {
                    ProgressTutorial();
                }
                break;
            case 1:
                int trucks = 0;
                foreach (UnitBase unit in PlayerTroopManager.instance.playerUnits)
                {
                    if (unit.GetComponent<SupplyTruck>())
                        trucks++;
                }
                if (trucks >= 3)
                {
                    ProgressTutorial();
                }
                break;
            case 2:
                if (HQBuilding.HasPower(1, PlayerCam.playerArmy))
                {
                    ProgressTutorial();
                }
                break;
            case 3:
                foreach (UnitBase unit in PlayerTroopManager.instance.playerUnits)
                {
                    if (unit.type == UnitType.ConstructionSite && unit.GetComponent<Building>().powerCost > 0)
                        tutorialStages[tutorialStage].objects[0].SetActive(false);
                    else if (unit.type == UnitType.DefenseBuilding)
                    {
                        ProgressTutorial();
                        Vector3 targetPoint = new Vector3();
                        Vector3 dir = new Vector3();
                        for (int i = 0; i < tutorialEnemies.Length; i++)
                        {
                            dir += tutorialEnemies[i].transform.position;
                        }
                        dir /= tutorialEnemies.Length;
                        dir = (dir - unit.transform.position).normalized;
                        targetPoint = unit.transform.position + dir * 25;
                        Formations.instance.SetFormation(tutorialEnemies, targetPoint);
                    }
                }
                break;
            case 4:
                int enemies = 0;
                for (int i = 0; i < tutorialEnemies.Length; i++)
                {
                    if (tutorialEnemies[i])
                        enemies++;
                }
                if (enemies == 0)
                {
                    ProgressTutorial();
                }
                break;
            case 5:
                foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
                {
                    if (hq.army == PlayerCam.playerArmy && hq.canHeal == false)
                    {
                        ProgressTutorial();
                    }
                }
                break;
            case 6:
                foreach (UnitBase unit in PlayerTroopManager.instance.playerUnits)
                {
                    if (unit.type == UnitType.LightFactory)
                    {
                        ProgressTutorial();
                    }
                }
                break;
            case 7:
                int tanks = 0;
                foreach (UnitBase unit in PlayerTroopManager.instance.playerUnits)
                {
                    if (unit.type == UnitType.LightTroop)
                        tanks++;
                }
                if (tanks >= 3)
                {
                    ProgressTutorial();
                }
                break;
            case 8:
                int units = 0;
                for (int i = 0; i < outpostUnits.Length; i++)
                {
                    if (outpostUnits[i])
                        units++;
                }
                if (units == 0)
                {
                    ProgressTutorial();
                }
                break;
            case 9:
                break;
        }
        
    }

    void ProgressTutorial()
    {
        for (int i = 0; i < tutorialStages[tutorialStage].objects.Length; i++)
        {
            if (tutorialStages[tutorialStage].objects[i])
                tutorialStages[tutorialStage].objects[i].SetActive(false);
        }
        tutorialStage++;
        for (int i = 0; i < tutorialStages[tutorialStage].objects.Length; i++)
        {
            tutorialStages[tutorialStage].objects[i].SetActive(true);
        }
        if (tutorialStage == 9)
            aiSetup.StartAI(1, 1, 5);
    }
}

[System.Serializable]
public class TutorialStage
{
    public GameObject[] objects;
}