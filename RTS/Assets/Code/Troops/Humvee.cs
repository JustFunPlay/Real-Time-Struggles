using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humvee : TroopMovement
{
    public Transform turretRotate;
    public Transform barrelAngle;
    public Transform FirePoint;
    public Transform aimAssist;
    public LayerMask hitlayer;

    public int damage;
    public float range;
    public float attackSpeed;
    public float turretRotSpeed;

    public LineRenderer line;
    Vector3 lookAt;
    public UnitBase target;
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
        lookAt = Vector3.Lerp(lookAt, aimAssist.forward, turretRotSpeed * Time.fixedDeltaTime);
        turretRotate.LookAt(turretRotate.position + new Vector3(lookAt.x, 0, lookAt.z));
        barrelAngle.LookAt(barrelAngle.position + turretRotate.forward + new Vector3(0, lookAt.y, 0), Vector3.up);
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
        if (Physics.Raycast(FirePoint.position, FirePoint.forward, out RaycastHit hit, range + 10, hitlayer))
        {
            UnitBase victim = hit.collider.GetComponent<UnitBase>();
            if (victim.army != army)
            {
                if (victim.type == UnitType.LightTroop || victim.type == UnitType.ResourceTruck)
                    victim.OnTakeDamage(damage * 2);
                else
                    victim.OnTakeDamage(damage);
            }
            LineRenderer newLine = Instantiate(line);
            newLine.SetPosition(0, FirePoint.position);
            newLine.SetPosition(1, hit.point);
        }
        else
        {
            LineRenderer newLine = Instantiate(line);
            newLine.SetPosition(0, FirePoint.position);
            newLine.SetPosition(1, FirePoint.position + FirePoint.forward * (range + 10));
        }
        yield return new WaitForSeconds(attackSpeed);
        canFire = true;
    }
}
