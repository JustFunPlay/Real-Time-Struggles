using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBase : MonoBehaviour
{
    public string unitName;
    public int maxHP;
    public int currentHP;
    public Army army;
    public bool canBeSeclected;
    public GameObject showSelected;
    public UnitType type;
    public MeshRenderer[] renderers;
    public SkinnedMeshRenderer[] skinnedRenderers;
    public Vector3[] targetingPoints;
    /// <summary>
    /// corners, halfwaypoints and centre-of-mass for the purpose of targeting distance, [0] is allways centre-of-mass
    /// </summary>
    public Sprite icon;
    bool isSelected;

    public GameObject hpbar;
    public Slider hpSlider;
    float showTime = 0;

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
        canBeSeclected = true;
        hpbar.SetActive(false);
        hpSlider.maxValue = maxHP;
    }

    public virtual void OnTakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            OnDeath();
        else if (showTime <= 0)
            StartCoroutine(ShowHpBar());
        showTime = 1.5f;
    }
    public void DelayedDamage(int damage, float delay)
    {
        StartCoroutine(DelayDamageTaken(damage, delay));
    }
    IEnumerator DelayDamageTaken(int damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        OnTakeDamage(damage);
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
        if (canBeSeclected)
        {
            if (!PlayerCam.instance.selectedUnits.Contains(this))
                PlayerCam.instance.selectedUnits.Add(this);
            showSelected.SetActive(true);
            isSelected = true;

            if (showTime <= 0)
                StartCoroutine(ShowHpBar());
            showTime = 0.5f;
        }
    }
    public void OnDeselected()
    {
        isSelected = false;
        PlayerCam.instance.selectedUnits.Remove(this);
        showSelected.SetActive(false);
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
    IEnumerator ShowHpBar()
    {
        hpbar.SetActive(true);
        hpSlider.value = currentHP;
        hpbar.transform.LookAt(PlayerCam.instance.cam.transform.position, Vector3.up);
        if (currentHP * 3 <= maxHP)
            hpSlider.fillRect.GetComponent<Image>().color = Color.red;
        else if (currentHP * 1.5 <= maxHP)
            hpSlider.fillRect.GetComponent<Image>().color = Color.yellow;
        else
            hpSlider.fillRect.GetComponent<Image>().color = Color.green;
        yield return new WaitForFixedUpdate();
        while (isSelected || showTime > 0)
        {
            if (currentHP * 3 <= maxHP)
                hpSlider.fillRect.GetComponent<Image>().color = Color.red;
            else if (currentHP * 1.5 <= maxHP)
                hpSlider.fillRect.GetComponent<Image>().color = Color.yellow;
            else
                hpSlider.fillRect.GetComponent<Image>().color = Color.green;
            hpSlider.value = currentHP;
            hpbar.transform.LookAt(PlayerCam.instance.cam.transform.position, Vector3.up);
            yield return new WaitForFixedUpdate();
            if (!isSelected)
                showTime -= Time.fixedDeltaTime;
        }
        hpbar.SetActive(false);
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
    ConstructionSite,
    TechCenter,
    RepairBay
}