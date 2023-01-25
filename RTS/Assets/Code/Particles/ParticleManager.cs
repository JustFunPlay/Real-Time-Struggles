using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    [Header("Light attacks")]
    public GameObject exampleBullet;
    List<GameObject> bullets = new List<GameObject>();
    public ParticleSystem exampleHit;
    List<ParticleSystem> hits = new List<ParticleSystem>();
    int currentBulletIndex;

    [Header("Heavy attacks")]
    public TankShell exampleShell;
    List<TankShell> shells = new List<TankShell>();
    public SniperProjectile exampleVariantShell;
    List<SniperProjectile> variantShells = new List<SniperProjectile>();
    public ParticleSystem exampleHeavyHit;
    List<ParticleSystem> heavyHits = new List<ParticleSystem>();
    int currentHeavyIndex;
    int currentHeavyHitIndex;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 50; i++)
        {
            bullets.Add(Instantiate(exampleBullet, transform.position, Quaternion.identity, transform));
            bullets[i].SetActive(false);
            hits.Add(Instantiate(exampleHit, transform.position, Quaternion.identity, transform));
            shells.Add(Instantiate(exampleShell, transform.position, Quaternion.identity, transform));
            shells[i].gameObject.SetActive(false);
            variantShells.Add(Instantiate(exampleVariantShell, transform.position, Quaternion.identity, transform));
            variantShells[i].gameObject.SetActive(false);
            heavyHits.Add(Instantiate(exampleHeavyHit, transform.position, Quaternion.identity, transform));
        }
    }

    public void FireBullet(Vector3 startpos, Vector3 endpos)
    {
        if (currentBulletIndex >= bullets.Count)
            currentBulletIndex = 0;
        bullets[currentBulletIndex].SetActive(true);
        bullets[currentBulletIndex].transform.position = startpos;
        bullets[currentBulletIndex].transform.LookAt(endpos, Vector3.up);
        StartCoroutine(ProjectBullet(currentBulletIndex, startpos, endpos));
        currentBulletIndex++;

    }
    IEnumerator ProjectBullet(int index, Vector3 startpoint, Vector3 endpoint)
    {
        //Vector3 dir = (endpoint - startpoint).normalized;
        float dst = Vector3.Distance(startpoint, endpoint);
        for (int i = 0; i < 10; i++)
        {
            bullets[index].transform.Translate(Vector3.forward * (dst / 10));
            yield return new WaitForSeconds(0.01f);
        }
        bullets[index].SetActive(false);
        hits[index].transform.position = endpoint;
        hits[index].transform.LookAt(startpoint, Vector3.up);
        hits[index].Play();
    }

    public void HeavyBullet(Transform origin, bool isVariant)
    {
        if (currentHeavyIndex >= shells.Count)
        {

        }
    }
}
