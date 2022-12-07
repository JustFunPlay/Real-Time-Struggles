using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : TroopMovement
{
    public Transform turretRotate;
    public Transform barrelAngle;
    public Transform FirePoint;
    public Transform aimAssist;
    public TankShell shell;

    public int damage;
    public float range;
    public float attackSpeed;
    public float turretRotSpeed;

    UnitBase target;
    bool canFire = true;

    private void FixedUpdate()
    {
        if (!target)
        {
            FindTarget();
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
        Vector3 newLookAt = Vector3.Lerp(turretRotate.forward, aimAssist.forward, turretRotSpeed * Time.fixedDeltaTime);
        turretRotate.LookAt(turretRotate.position + new Vector3(newLookAt.x, 0, newLookAt.z));
        barrelAngle.LookAt(barrelAngle.position + turretRotate.forward + new Vector3(0, newLookAt.y, 0), Vector3.up);
    }
    void FindTarget()
    {
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army != army && Vector3.Distance(turretRotate.position, unit.GetClosestTargetingPoint(transform.position)) <= range)
            {
                target = unit;
                break;
            }
        }
    }

    IEnumerator Fire()
    {
        canFire = false;
        TankShell firedShell = Instantiate(shell, FirePoint.position, FirePoint.rotation);
        firedShell.Launch(army, damage);
        yield return new WaitForSeconds(attackSpeed);
        canFire = true;
    }
}
