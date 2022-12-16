using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLifeTime : MonoBehaviour
{
    public float lifeTime;
    private void Start()
    {
        StartCoroutine(Yeet());
    }

    IEnumerator Yeet()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
