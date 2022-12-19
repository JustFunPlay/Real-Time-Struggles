using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotateModifier;

    void FixedUpdate()
    {
        transform.Rotate(rotateModifier * Time.fixedDeltaTime);
    }
}
