using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Howitzer : CombatTroop
{
    public ArtilleryShell shell;

    public float splashRadius;
    public float minimumRange;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!target)
        {
            aimAssist.LookAt(aimAssist.position + transform.forward, Vector3.up);
        }
        else if (Vector3.Distance(target.GetClosestTargetingPoint(transform.position), aimAssist.position) > range || Vector3.Distance(target.GetClosestTargetingPoint(transform.position), aimAssist.position) < minimumRange)
            target = null;
        else
        {
            aimAssist.LookAt(target.transform.position + target.transform.TransformDirection(target.targetingPoints[0]), Vector3.up);
            aimAssist.eulerAngles = new Vector3(-CalculateBarrelAngle() * Mathf.Rad2Deg, aimAssist.eulerAngles.y, aimAssist.eulerAngles.z);
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
        foreach (Building building in PlayerTroopManager.instance.buildings)
        {
            if (building.army != army && Vector3.Distance(turretRotate.position, building.GetClosestTargetingPoint(transform.position)) <= range && Vector3.Distance(building.GetClosestTargetingPoint(transform.position), aimAssist.position) > minimumRange && building.type == UnitType.DefenseBuilding && (!potentialTarget || Vector3.Distance(building.GetClosestTargetingPoint(transform.position), aimAssist.position) < Vector3.Distance(potentialTarget.GetClosestTargetingPoint(transform.position), aimAssist.position)))
                potentialTarget = building;
        }
        if (potentialTarget)
        {
            target = potentialTarget;
            return;
        }
        foreach (Building building in PlayerTroopManager.instance.buildings)
        {
            if (building.army != army && Vector3.Distance(turretRotate.position, building.GetClosestTargetingPoint(transform.position)) <= range && Vector3.Distance(building.GetClosestTargetingPoint(transform.position), aimAssist.position) > minimumRange && (!potentialTarget || Vector3.Distance(building.GetClosestTargetingPoint(transform.position), aimAssist.position) < Vector3.Distance(potentialTarget.GetClosestTargetingPoint(transform.position), aimAssist.position)))
                potentialTarget = building;
        }
        if (potentialTarget)
        {
            target = potentialTarget;
            return;
        }
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army != army && Vector3.Distance(turretRotate.position, unit.GetClosestTargetingPoint(transform.position)) <= range && Vector3.Distance(unit.GetClosestTargetingPoint(transform.position), aimAssist.position) > minimumRange && (!potentialTarget || Vector3.Distance(unit.GetClosestTargetingPoint(transform.position), aimAssist.position) < Vector3.Distance(potentialTarget.GetClosestTargetingPoint(transform.position), aimAssist.position)))
                potentialTarget = unit;
        }
        target = potentialTarget;
    }

    IEnumerator Fire()
    {
        canFire = false;
        muzzleFlash.Play();
        ArtilleryShell firedShell = Instantiate(shell, FirePoint.position, FirePoint.rotation);
        firedShell.Launch(damage, splashRadius, target.transform.position + target.transform.TransformDirection(target.targetingPoints[0]), army);
        yield return new WaitForSeconds(attackSpeed);
        canFire = true;
    }

    float CalculateBarrelAngle()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 dir = targetPos - barrelAngle.position;
        Vector3 groundDir = new Vector3(dir.x, 0, dir.z);
        targetPos = new Vector3(groundDir.magnitude, targetPos.y, 0);

        float height = targetPos.y + targetPos.magnitude / 2f;


        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float a = (-0.5f * g);
        float b = Mathf.Sqrt(2 * g * height);
        float c = -yt;

        float tPlus = QuadraticEquation(a, b, c, 1);
        float tMin = QuadraticEquation(a, b, c, -1);

        float time = tPlus > tMin ? tPlus : tMin;
        float angle = Mathf.Atan(b * time / xt);
        return angle;
    }
    float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }
}
