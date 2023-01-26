using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankShell : MonoBehaviour
{
    protected Army army;
    protected int damage;
    protected Rigidbody rb;
    public float flySpeed;
    protected float airTime;

    public void Launch(Army army_, int damage_)
    {
        army = army_;
        damage = damage_;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Fly());
    }
    IEnumerator Fly()
    {
        airTime= 10;
        while (airTime >= 0)
        {
            rb.MovePosition(transform.position + transform.forward * flySpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
            airTime -= Time.fixedDeltaTime;
        }
        gameObject.SetActive(false);
        
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<UnitBase>())
        {
            if (collision.collider.GetComponent<UnitBase>().army != army)
                collision.collider.GetComponent<UnitBase>().OnTakeDamage(damage);
        }
        airTime = 0;
        ParticleManager.instance.HeavyHit(transform);
        gameObject.SetActive(false);
    }
}
