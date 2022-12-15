using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperProjectile : TankShell
{
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<UnitBase>())
        {
            if (collision.collider.GetComponent<UnitBase>().army != army)
            {
                if (collision.collider.GetComponent<UnitBase>().type == UnitType.Troop)
                    collision.collider.GetComponent<UnitBase>().OnTakeDamage(damage * 4);
                else
                    collision.collider.GetComponent<UnitBase>().OnTakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
