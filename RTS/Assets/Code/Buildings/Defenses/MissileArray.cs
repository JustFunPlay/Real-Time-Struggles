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

    private void FixedUpdate()
    {
        if (!target)
            FindTarget();
        else
        {
            aimAssist.LookAt(target.transform.position, Vector3.up);
            Vector3 newLookAt = Vector3.Lerp(horizontalTurn.forward, aimAssist.forward, turnSpeed * Time.fixedDeltaTime);
            horizontalTurn.LookAt(horizontalTurn.position + new Vector3(newLookAt.x, 0, newLookAt.z));
            //verticalTurn.rotation = new Quaternion(0, aimAssist.rotation.y, 0, 0);
            if (canFire && Vector3.Dot(horizontalTurn.forward, target.transform.position - horizontalTurn.position) > 0.9f)
                StartCoroutine(MissileBurst());
        }
    }

    void FindTarget()
    {
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army != army && Vector3.Distance(transform.position, unit.transform.position) <= range)
            {
                target = unit;
                break;
            }
        }
    }

    IEnumerator MissileBurst()
    {
        canFire = false;
        for (int i = 0; i < burstCount; i++)
        {
            LaunchMissile();
            yield return new WaitForSeconds(burstDelay);
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
