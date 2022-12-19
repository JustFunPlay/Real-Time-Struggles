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
    public UnitType type;
    public MeshRenderer[] renderers;
    public SkinnedMeshRenderer[] skinnedRenderers;
    public Vector3[] targetingPoints;
    /// <summary>
    /// corners, halfwaypoints and centre-of-mass for the purpose of targeting distance, [0] is allways centre-of-mass
    /// </summary>

    public virtual void AddedUnit(Army army_)
    {
        army = army_;
        for (int i = 0; i < renderers.Length; i++)
        {
            switch (army)
            {
                case Army.Blue:
                    renderers[i].material.SetColor("_Color", Color.blue);
                    break;
                case Army.Cyan:
                    renderers[i].material.SetColor("_Color", Color.cyan);
                    break;
                case Army.Green:
                    renderers[i].material.SetColor("_Color", Color.green);
                    break;
                case Army.Magenta:
                    renderers[i].material.SetColor("_Color", Color.magenta);
                    break;
                case Army.Red:
                    renderers[i].material.SetColor("_Color", Color.red);
                    break;
                case Army.Yellow:
                    renderers[i].material.SetColor("_Color", Color.yellow);
                    break;
                default:
                    Debug.LogError("you somohow managed to fail at assigning an army color, how did you even do that?");
                    break;
            }

        }
        for (int i = 0; i < skinnedRenderers.Length; i++)
        {
            switch (army)
            {
                case Army.Blue:
                    skinnedRenderers[i].material.SetColor("_Color", Color.blue);
                    break;
                case Army.Cyan:
                    skinnedRenderers[i].material.SetColor("_Color", Color.cyan);
                    break;
                case Army.Green:
                    skinnedRenderers[i].material.SetColor("_Color", Color.green);
                    break;
                case Army.Magenta:
                    skinnedRenderers[i].material.SetColor("_Color", Color.magenta);
                    break;
                case Army.Red:
                    skinnedRenderers[i].material.SetColor("_Color", Color.red);
                    break;
                case Army.Yellow:
                    skinnedRenderers[i].material.SetColor("_Color", Color.yellow);
                    break;
                default:
                    Debug.LogError("you somohow managed to fail at assigning an army color, how did you even do that?");
                    break;
            }
        }
        if (army == PlayerCam.playerArmy)
            PlayerTroopManager.instance.playerUnits.Add(this);
        currentHP = maxHP;
        PlayerTroopManager.instance.allUnits.Add(this);
    }

    public virtual void OnTakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            OnDeath();
    }
    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    public virtual void OnHeal(int healAmmount)
    {
        currentHP += healAmmount;
    }

    public void OnSelected()
    {
        if (!PlayerCam.instance.selectedUnits.Contains(this))
            PlayerCam.instance.selectedUnits.Add(this);
        isSelected = true;
        showSelected.material.color = Color.green;
    }
    public void OnDeselected()
    {
        PlayerCam.instance.selectedUnits.Remove(this);
        isSelected = false;
        showSelected.material.color = Color.white;
    }

    protected virtual void OnDestroy()
    {
        if (army == PlayerCam.playerArmy)
        {
            PlayerTroopManager.instance.playerUnits.Remove(this);
            OnDeselected();
        }
        PlayerTroopManager.instance.allUnits.Remove(this);
    }

    public Vector3 GetClosestTargetingPoint(Vector3 origin)
    {
        Vector3 closestPos = transform.position;
        float closestDst = Vector3.Distance(transform.position, origin);
        foreach (Vector3 pos in targetingPoints)
        {
            if (Vector3.Distance(transform.position + transform.TransformDirection(pos), origin) < closestDst)
            {
                closestPos = transform.position + transform.TransformDirection(pos);
                closestDst = Vector3.Distance(transform.position + transform.TransformDirection(pos), origin);
            }
        }

        return closestPos;
    }
}

[System.Serializable]
public enum UnitType
{
    HeadQuarters,
    ResourceTruck,
    ResourceDepot,
    LightTroop,
    HeavyTroop,
    DefenseBuilding,
    LightFactory,
    HeavyFactory,
    PowerPlant,
    ConstructionSite
}