using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TroopMovement : UnitBase
{
    NavMeshAgent agent;
    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        agent = GetComponent<NavMeshAgent>();
        
    }

    //void Update()
    //{
        
    //}
    public void MoveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }
    public Vector3 ToViewportSpace(Camera camera)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);
        return viewportPoint;
    }
}
