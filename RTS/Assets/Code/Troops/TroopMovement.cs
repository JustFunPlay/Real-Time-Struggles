using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopMovement : UnitBase
{
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Update()
    {
        
    }
    public void moveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }
}
