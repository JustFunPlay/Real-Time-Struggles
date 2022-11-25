using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankShell : MonoBehaviour
{
    Army army;
    int damage;
    Rigidbody rb;
    public float flySpeed;

    public void Launch(Army army_, int damage_)
    {
        army = army_;
        damage = damage_;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Fly());
    }
    IEnumerator Fly()
    {
        float t = 10;
        while (t >= 0)
        {
            rb.MovePosition(transform.position + transform.forward * flySpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
            t -= Time.fixedDeltaTime;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<UnitBase>())
        {
            if (collision.collider.GetComponent<UnitBase>().army != army)
                collision.collider.GetComponent<UnitBase>().OnTakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
