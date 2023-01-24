using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurret : Building
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

    Vector3 lookAt;
    public UnitBase target;
    bool canFire = true;

    public ParticleSystem muzzleFlash;

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
                if (canFire && Vector3.Dot(turretRotate.forward, ((target.transform.position + target.transform.TransformDirection(target.targetingPoints[0])) - turretRotate.position).normalized) > 0.99f)
                    StartCoroutine(Fire());
            }
            lookAt = Vector3.Lerp(lookAt, aimAssist.forward, turretRotSpeed * Time.fixedDeltaTime);
            turretRotate.LookAt(turretRotate.position + new Vector3(lookAt.x, 0, lookAt.z));
            barrelAngle.LookAt(barrelAngle.position + turretRotate.forward + new Vector3(0, lookAt.y, 0), Vector3.up);
        }
    }
    void FindTarget()
    {
        foreach (TroopMovement troop in PlayerTroopManager.instance.troops)
        {
            if (troop.army != army && Vector3.Distance(turretRotate.position, troop.GetClosestTargetingPoint(transform.position)) <= range)
            {
                target = troop;
                break;
            }
        }
    }

    IEnumerator Fire()
    {
        canFire = false;
        muzzleFlash.Play();
        target.OnTakeDamage(damage);

        ParticleManager.instance.SetLine(FirePoint.position, FirePoint.position + FirePoint.forward * Vector3.Distance(FirePoint.position, target.transform.position));

        yield return new WaitForSeconds(attackSpeed);
        canFire = true;
    }
}
