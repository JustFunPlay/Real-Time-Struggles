using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTroop : TroopMovement
{
    public Transform turretRotate;
    public Transform barrelAngle;
    public Transform firePoint;
    public Transform aimAssist;

    public int damage;
    public float range;
    public float attackSpeed;
    public float turretRotSpeed;

    protected Vector3 lookAt;
    public UnitBase target;
    protected bool canFire = true;

    public ParticleSystem muzzleFlash;

    public AudioSource shootSound;

    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        canFire = true;
        InvokeRepeating("FindTarget", Random.Range(0f, 1f), 1);
    }

    protected virtual void FindTarget()
    {

    }
}
