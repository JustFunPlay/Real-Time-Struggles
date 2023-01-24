using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileArray : Building
{
    public float range;
    public int burstCount;
    public float burstDelay;
    public float timeBetweenBursts;

    public int damage;
    public float splashRadius;
    public Missile missile;
    public Transform launchPoint;
    UnitBase target;
    public float turnSpeed;
    bool canFire = true;
    public Transform aimAssist;
    public Transform horizontalTurn;
    public Transform verticalTurn;

    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        InvokeRepeating("FindTarget", Random.Range(0f, 1f), 1);
    }
    private void FixedUpdate()
    {
        if (HQBuilding.HasPower(0, army))
        {
            if (!target)
            {
                aimAssist.LookAt(aimAssist.position + transform.forward, Vector3.up);
            }
            else if (Vector3.Distance(target.GetClosestTargetingPoint(transform.position), aimAssist.position) > range)
                target = null;
            else
            {
                aimAssist.LookAt(target.transform.position + target.transform.TransformDirection(target.targetingPoints[0]), Vector3.up);
                if (canFire && Vector3.Dot(horizontalTurn.forward, ((target.transform.position + target.transform.TransformDirection(target.targetingPoints[0])) - horizontalTurn.position).normalized) > 0.95f)
                    StartCoroutine(MissileBurst());
            }
            Vector3 newLookAt = Vector3.Lerp(horizontalTurn.forward, aimAssist.forward, turnSpeed * Time.fixedDeltaTime);
            horizontalTurn.LookAt(horizontalTurn.position + new Vector3(newLookAt.x, 0, newLookAt.z));
            verticalTurn.LookAt(verticalTurn.position + horizontalTurn.forward + new Vector3(0, newLookAt.y, 0), Vector3.up);
        }
    }

    void FindTarget()
    {
        foreach (TroopMovement troop in PlayerTroopManager.instance.troops)
        {
            if (troop.army != army && Vector3.Distance(aimAssist.position, troop.GetClosestTargetingPoint(transform.position)) <= range)
            {
                target = troop;
                break;
            }
        }
    }

    IEnumerator MissileBurst()
    {
        canFire = false;
        for (int i = 0; i < burstCount; i++)
        {
            if (target)
            {
                LaunchMissile();
                yield return new WaitForSeconds(burstDelay);
            }
        }
        yield return new WaitForSeconds(timeBetweenBursts);
        canFire = true;
    }
    void LaunchMissile()
    {
        Missile missileToLaunch = Instantiate(missile, launchPoint.position, launchPoint.rotation);
        missileToLaunch.Launch(army, target, damage, splashRadius);
    }
}
