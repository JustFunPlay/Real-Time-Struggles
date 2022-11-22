using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopMovement : UnitBase
{
    NavMeshAgent agent;
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Update()
    {
        
    }
    public void moveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }
    public Vector3 ToViewportSpace(Camera camera)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);
        return viewportPoint;
    }
}
