using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Formations", menuName = "ScriptableObjects/Formations")]
public class Formations : ScriptableObject
{
    public Formation[] formations;

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
        for (int i = 0; i < troops.Length; i++)
        {
            troops[i].MoveToPosition(Targetpoint + new Vector3(formations[set].positions[i].x * dir.x, 0, formations[set].positions[i].z * dir.z));
        }
    }
}

[System.Serializable]
public class Formation
{
    public Vector3[] positions;
}