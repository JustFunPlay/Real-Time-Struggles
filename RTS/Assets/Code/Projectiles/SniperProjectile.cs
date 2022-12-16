using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperProjectile : TankShell
{
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<UnitBase>())
        {
            UnitBase unit = collision.collider.GetComponent<UnitBase>();
            if (unit.army != army)
            {
                if (unit.type == UnitType.LightTroop || unit.type == UnitType.HeavyTroop)
                    unit.OnTakeDamage(damage * 4);
                else
                    unit.OnTakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
