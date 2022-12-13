using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formations : MonoBehaviour
{
    public static Formations instance;
    public Formation[] formations;

    private void Start()
    {
        instance = this;
    }
    public void SetFormation(TroopMovement[] troops, Vector3 Targetpoint)
    {
        int set = troops.Length - 1;
        if (troops.Length > formations.Length)
        {
            set = formations.Length - 1;
        }
        Vector3 midPoint = new Vector3();
        for (int i = 0; i < troops.Length; i++)
        {
            midPoint += troops[i].transform.position;
        }
        midPoint /= troops.Length;
        Vector3 dir = (Targetpoint - midPoint).normalized;
        transform.LookAt(transform.position + dir);
        for (int i = 0; i < troops.Length; i++)
        {
            troops[i].MoveToPosition(Targetpoint + transform.TransformDirection(formations[set].positions[i]));
        }
    }
}

[System.Serializable]
public class Formation
{
    public Vector3[] positions;
}