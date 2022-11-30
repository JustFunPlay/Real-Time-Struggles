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
    public int colorMatIndex;
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
            if (army == Army.Blue)
                renderers[i].materials[colorMatIndex].color = Color.blue;
            else if (army == Army.Cyan)
                renderers[i].materials[colorMatIndex].color = Color.cyan;
            else if (army == Army.Green)
                renderers[i].materials[colorMatIndex].color = Color.green;
            else if (army == Army.Magenta)
                renderers[i].materials[colorMatIndex].color = Color.magenta;
            else if (army == Army.Red)
                renderers[i].materials[colorMatIndex].color = Color.red;
            else if (army == Army.Yellow)
                renderers[i].materials[colorMatIndex].color = Color.yellow;
        }
        for (int i = 0; i < skinnedRenderers.Length; i++)
        {
            if (army == Army.Blue)
                skinnedRenderers[i].materials[colorMatIndex].color = Color.blue;
            else if (army == Army.Cyan)
                skinnedRenderers[i].materials[colorMatIndex].color = Color.cyan;
            else if (army == Army.Green)
                skinnedRenderers[i].materials[colorMatIndex].color = Color.green;
            else if (army == Army.Magenta)
                skinnedRenderers[i].materials[colorMatIndex].color = Color.magenta;
            else if (army == Army.Red)
                skinnedRenderers[i].materials[colorMatIndex].color = Color.red;
            else if (army == Army.Yellow)
                skinnedRenderers[i].materials[colorMatIndex].color = Color.yellow;
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

    private void OnDestroy()
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
    Troop,
    DefenseBuilding,
    Factory
}