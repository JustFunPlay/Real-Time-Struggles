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
    public LayerMask explosionLayer;
    bool inFlight;

    public void Launch(Army army_, UnitBase target_, int damage_, float radius_)
    {
        army = army_;
        target = target_;
        damage = damage_;
        blastRadius = radius_;
        rb = GetComponent<Rigidbody>();
        inFlight = true;
        StartCoroutine(Fly());
    }

    IEnumerator Fly()
    {
        while (inFlight && Vector3.Distance(targetPos, transform.position) > 1f)
        {
            if (target)
                targetPos = target.transform.position + target.transform.TransformDirection(target.targetingPoints[0]);
            transform.LookAt(targetPos, Vector3.up);
            rb.MovePosition(transform.position + transform.forward * flySpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        Explode();
    }
    void Explode()
    {
        //foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        //{
        //    if (unit.army != army && Vector3.Distance(transform.position, unit.GetClosestTargetingPoint(transform.position)) <= blastRadius)
        //        unit.OnTakeDamage(damage);
        //}

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, explosionLayer);
        List<UnitBase> units = new List<UnitBase>();
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<UnitBase>() && !units.Contains(collider.GetComponent<UnitBase>()) && collider.GetComponent<UnitBase>().army != army)
            {
                units.Add(collider.GetComponent<UnitBase>());
                collider.GetComponent<UnitBase>().OnTakeDamage(damage);
            }
        }
        inFlight = false;
        ParticleManager.instance.MissileExplosion(transform);
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
