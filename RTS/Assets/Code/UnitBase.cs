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

    protected virtual void Start()
    {
        if (GetComponentInChildren<SkinnedMeshRenderer>())
        {
            if (army == Army.Blue)
                GetComponentInChildren<SkinnedMeshRenderer>().materials[colorMatIndex].color = Color.blue;
            else if (army == Army.Cyan)
                GetComponentInChildren<SkinnedMeshRenderer>().materials[colorMatIndex].color = Color.cyan;
            else if (army == Army.Green)
                GetComponentInChildren<SkinnedMeshRenderer>().materials[colorMatIndex].color = Color.green;
            else if (army == Army.Magenta)
                GetComponentInChildren<SkinnedMeshRenderer>().materials[colorMatIndex].color = Color.magenta;
            else if (army == Army.Red)
                GetComponentInChildren<SkinnedMeshRenderer>().materials[colorMatIndex].color = Color.red;
            else if (army == Army.Yellow)
                GetComponentInChildren<SkinnedMeshRenderer>().materials[colorMatIndex].color = Color.yellow;
        }
        currentHP = maxHP;

        if (army == PlayerCam.playerArmy)
            PlayerTroopManager.instance.playerUnits.Add(this);
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
}

[System.Serializable]
public enum UnitType
{
    HeadQuaters,
    ResourceTruck,
    ResourceDepot,
    Troop
}