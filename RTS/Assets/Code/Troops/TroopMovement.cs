using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TroopMovement : UnitBase
{
    NavMeshAgent agent;
    public bool inQueue;
    public bool inBuilding;
    public ParticleSystem moveSmoke;
    public override void AddedUnit(Army army_)
    {
        base.AddedUnit(army_);
        agent = GetComponent<NavMeshAgent>();
        PlayerTroopManager.instance.troops.Add(this);
    }

    protected virtual void FixedUpdate()
    {
        if (agent.velocity.magnitude < 1)
            moveSmoke.Pause();
        else
            moveSmoke.Play();
    }
    public void MoveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }
    public Vector3 ToViewportSpace(Camera camera)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);
        return viewportPoint;
    }
    protected override void OnDestroy()
    {
        PlayerTroopManager.instance.troops.Remove(this);
        base.OnDestroy();
    }
}
