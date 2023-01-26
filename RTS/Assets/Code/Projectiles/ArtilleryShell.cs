using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArtilleryShell : MonoBehaviour
{
    Rigidbody rb;
    float velocity;
    float angle;
    Vector3 startPos;
    Army army;

    //public GameObject splashEffect;
    float radius;
    int damage;
    float airTime;

    public void Launch(int damage_, float radius_, Vector3 targetPos, Army army_)
    {
        damage = damage_;
        radius = radius_;
        army = army_;
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        Vector3 dir = targetPos - startPos;
        Vector3 groundDir = new Vector3(dir.x, 0, dir.z);
        targetPos = new Vector3(groundDir.magnitude, dir.y, 0);
        float InitialVelocity;
        float time;
        float angle;
        float height = targetPos.y + targetPos.magnitude / 2f;
        height = Mathf.Max(0.001f, height);
        CalculatePathWithHeight(targetPos, height, out InitialVelocity, out angle, out time);
        StartCoroutine(InAirMoving(groundDir.normalized, InitialVelocity, angle, time + 5));
    }

    float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }
    void CalculatePathWithHeight(Vector3 targetPos, float h, out float initialVelocity, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float a = (-0.5f * g);
        float b = Mathf.Sqrt(2 * g * h);
        float c = -yt;

        float tPlus = QuadraticEquation(a, b, c, 1);
        float tMin = QuadraticEquation(a, b, c, -1);

        time = tPlus > tMin ? tPlus : tMin;
        angle = Mathf.Atan(b * time / xt);
        initialVelocity = b / Mathf.Sin(angle);
    }

    IEnumerator InAirMoving(Vector3 dir, float initialVelocity, float angle, float time)
    {
        airTime = 0;
        while (airTime < time)
        {
            float x = initialVelocity * airTime * Mathf.Cos(angle);
            float y = initialVelocity * airTime * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(airTime, 2);
            rb.MovePosition(startPos + dir * x + Vector3.up * y);

            airTime += Time.fixedDeltaTime * 5;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (UnitBase unit in PlayerTroopManager.instance.allUnits)
        {
            if (unit.army != army && Vector3.Distance(transform.position, unit.GetClosestTargetingPoint(transform.position)) <= radius)
            {
                if (unit.GetComponent<Building>())
                    unit.OnTakeDamage(damage * 4);
                else
                    unit.OnTakeDamage(damage);
            }
        }
        airTime = 1000;
        ParticleManager.instance.ArtilleryHit(transform.position);
        gameObject.SetActive(false);
    }
}
