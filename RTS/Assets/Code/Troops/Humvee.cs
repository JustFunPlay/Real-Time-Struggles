using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humvee : CombatTroop
{
    
    public LayerMask hitlayer;

    private void FixedUpdate()
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
            if (canFire && Vector3.Dot(turretRotate.forward, ((target.transform.position + target.transform.TransformDirection(target.targetingPoints[0])) - turretRotate.position).normalized) > 0.99f)
                StartCoroutine(Fire());
        }
        lookAt = Vector3.Lerp(lookAt, aimAssist.forward, turretRotSpeed * Time.fixedDeltaTime);
        turretRotate.LookAt(turretRotate.position + new Vector3(lookAt.x, 0, lookAt.z));
        barrelAngle.LookAt(barrelAngle.position + turretRotate.forward + new Vector3(0, lookAt.y, 0), Vector3.up);
    }
    protected override void FindTarget()
    {
        UnitBase potentialTarget = null;
        foreach (TroopMovement troop in PlayerTroopManager.instance.troops)
        {
            if (troop.army != army && Vector3.Distance(turretRotate.position, troop.GetClosestTargetingPoint(transform.position)) <= range && troop.type == UnitType.LightTroop && (!potentialTarget || Vector3.Distance(troop.GetClosestTargetingPoint(transform.position), aimAssist.position) < Vector3.Distance(potentialTarget.GetClosestTargetingPoint(transform.position), aimAssist.position)))
                potentialTarget = troop;
        }
        if (potentialTarget)
        {
            target = potentialTarget;
            return;
        }
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army != army && Vector3.Distance(turretRotate.position, unit.GetClosestTargetingPoint(transform.position)) <= range && (unit.type == UnitType.HeavyTroop || unit.type == UnitType.DefenseBuilding) && (!potentialTarget || Vector3.Distance(unit.GetClosestTargetingPoint(transform.position), aimAssist.position) < Vector3.Distance(potentialTarget.GetClosestTargetingPoint(transform.position), aimAssist.position)))
                potentialTarget = unit;
        }
        if (potentialTarget)
        {
            target = potentialTarget;
            return;
        }
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army != army && Vector3.Distance(turretRotate.position, unit.GetClosestTargetingPoint(transform.position)) <= range && (!potentialTarget || Vector3.Distance(unit.GetClosestTargetingPoint(transform.position), aimAssist.position) < Vector3.Distance(potentialTarget.GetClosestTargetingPoint(transform.position), aimAssist.position)))
                potentialTarget = unit;
        }
        target = potentialTarget;
    }

    IEnumerator Fire()
    {
        canFire = false;
        if (target.type == UnitType.LightTroop || target.type == UnitType.ResourceTruck)
            target.OnTakeDamage(damage * 2);
        else
            target.OnTakeDamage(damage);

        ParticleManager.instance.SetLine(FirePoint.position, FirePoint.position + FirePoint.forward * Vector3.Distance(FirePoint.position, target.transform.position));

        yield return new WaitForSeconds(attackSpeed);
        canFire = true;
    }
}
