using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Missile : MonoBehaviour
{
    Army army;
    UnitBase target;
    Vector3 targetPos;
    int damage;
    float blastRadius;
    Rigidbody rb;
    public float flySpeed;

    public void Launch(Army army_, UnitBase target_, int damage_, float radius_)
    {
        army = army_;
        target = target_;
        damage = damage_;
        blastRadius = radius_;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Fly());
    }

    IEnumerator Fly()
    {
        while (Vector3.Distance(targetPos, transform.position) > 1f)
        {
            if (target)
                targetPos = target.transform.position;
            transform.LookAt(targetPos, Vector3.up);
            rb.MovePosition(transform.position + transform.forward * flySpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        Explode();
    }
    void Explode()
    {
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army != army && Vector3.Distance(transform.position, unit.transform.position) <= blastRadius)
                unit.OnTakeDamage(damage);
        }
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
