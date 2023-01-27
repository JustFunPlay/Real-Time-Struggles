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

    [Header("Artillery")]
    public ArtilleryShell exampleArtillery;
    List<ArtilleryShell> artilleryShells = new List<ArtilleryShell>();
    public ParticleSystem exampleArtilleryHit;
    List<ParticleSystem> artilleryHits = new List<ParticleSystem>();
    int currentArtilleryIndex;
    int currentArtilleryHitIndex;

    [Header("Missile")]
    public Missile exampleMissile;
    List<Missile> missiles = new List<Missile>();
    int currentMissileIndex;
    public ParticleSystem exampleMissileExplosion;
    List<ParticleSystem> missileExplosions = new List<ParticleSystem>();
    int currentMissileExplosionIndex;

    [Header("Troop destruction")]
    public ParticleSystem exampleTroopExplosion;
    List<ParticleSystem> troopExplosions = new List<ParticleSystem>();
    int currentTroopExplosion;

    [Header("Building Destruction")]
    public ParticleSystem exampleBuildingExplosion;
    List<ParticleSystem> buildingExplosions = new List<ParticleSystem>();
    int currentBuildingExplosion;

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
            artilleryShells.Add(Instantiate(exampleArtillery, transform.position, Quaternion.identity, transform));
            artilleryShells[i].gameObject.SetActive(false);
            artilleryHits.Add(Instantiate(exampleArtilleryHit, transform.position, Quaternion.identity, transform));
            missiles.Add(Instantiate(exampleMissile, transform.position, Quaternion.identity, transform));
            missiles[i].gameObject.SetActive(false);
            missileExplosions.Add(Instantiate(exampleMissileExplosion, transform.position, Quaternion.identity, transform));
            troopExplosions.Add(Instantiate(exampleTroopExplosion, transform.position, Quaternion.identity, transform));
            buildingExplosions.Add(Instantiate(exampleBuildingExplosion, transform.position, Quaternion.identity, transform));
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
        hits[index].transform.LookAt(endpoint + (endpoint - startpoint), Vector3.up);
        hits[index].Play();
        hits[index].GetComponent<RandomSfx>().PlaySFX();
    }

    public void HeavyBullet(Transform origin, bool isVariant, out TankShell shell)
    {
        if (currentHeavyIndex >= shells.Count)
            currentHeavyIndex = 0;
        if (isVariant)
        {
            variantShells[currentHeavyIndex].gameObject.SetActive(true);
            variantShells[currentHeavyIndex].transform.position = origin.position;
            variantShells[currentHeavyIndex].transform.rotation = origin.rotation;
            shell = variantShells[currentBulletIndex];
        }
        else
        {
            shells[currentHeavyIndex].gameObject.SetActive(true);
            shells[currentHeavyIndex].transform.position = origin.position;
            shells[currentHeavyIndex].transform.rotation = origin.rotation;
            shell = shells[currentBulletIndex];
        }
        currentHeavyIndex++;
    }
    public void HeavyHit(Transform origin)
    {
        if (currentHeavyHitIndex >= heavyHits.Count)
            currentHeavyHitIndex = 0;
        heavyHits[currentHeavyHitIndex].transform.position = origin.position;
        heavyHits[currentHeavyHitIndex].transform.rotation = origin.rotation;
        heavyHits[currentHeavyHitIndex].Play();
        heavyHits[currentHeavyHitIndex].GetComponent<AudioSource>().pitch = Random.Range(0.5f, 3f);
        heavyHits[currentHeavyHitIndex].GetComponent<AudioSource>().Play();
        currentHeavyHitIndex++;
    }

    public void ArtilleryShell(Transform origin, out ArtilleryShell shell)
    {
        if (currentArtilleryIndex >= artilleryShells.Count)
            currentArtilleryIndex = 0;
        artilleryShells[currentArtilleryIndex].gameObject.SetActive(true);
        artilleryShells[currentArtilleryIndex].transform.position = origin.position;
        artilleryShells[currentArtilleryIndex].transform.rotation = origin.rotation;
        shell = artilleryShells[currentArtilleryIndex];
        currentArtilleryIndex++;
    }
    public void ArtilleryHit(Vector3 position)
    {
        if (currentArtilleryHitIndex >= artilleryHits.Count)
            currentArtilleryHitIndex = 0;
        artilleryHits[currentArtilleryHitIndex].transform.position = position;
        artilleryHits[currentArtilleryHitIndex].Play();
        artilleryHits[currentArtilleryHitIndex].GetComponent<RandomSfx>().PlaySFX();
        currentArtilleryHitIndex++;
    }

    public void Missile(Transform origin, out Missile missile)
    {
        if (currentMissileIndex >= missiles.Count)
            currentMissileIndex = 0;
        missiles[currentMissileIndex].gameObject.SetActive(true);
        missiles[currentMissileIndex].transform.position = origin.position;
        missiles[currentMissileIndex].transform.rotation = origin.rotation;
        missile = missiles[currentMissileIndex];
        currentMissileIndex++;
    }
    public void MissileExplosion(Transform origin)
    {
        if (currentMissileExplosionIndex >= missileExplosions.Count)
            currentMissileExplosionIndex = 0;
        missileExplosions[currentMissileExplosionIndex].transform.position = origin.position;
        missileExplosions[currentMissileExplosionIndex].transform.rotation = origin.rotation;
        missileExplosions[currentMissileExplosionIndex].Play();
        missileExplosions[currentMissileExplosionIndex].GetComponent<RandomSfx>().PlaySFX();
        currentMissileExplosionIndex++;
    }

    public void ExplodingTroop(Transform origin)
    {
        if (currentTroopExplosion >= troopExplosions.Count)
            currentTroopExplosion = 0;
        troopExplosions[currentTroopExplosion].transform.position = origin.position;
        troopExplosions[currentTroopExplosion].transform.rotation = origin.rotation;
        troopExplosions[currentTroopExplosion].Play();
        troopExplosions[currentTroopExplosion].GetComponent<RandomSfx>().PlaySFX();
        currentTroopExplosion++;
    }
    public void ExplodingBuilding(Transform origin)
    {
        if (currentBuildingExplosion >= buildingExplosions.Count)
            currentBuildingExplosion = 0;
        buildingExplosions[currentBuildingExplosion].transform.position = origin.position;
        buildingExplosions[currentBuildingExplosion].transform.rotation = origin.rotation;
        buildingExplosions[currentBuildingExplosion].Play();
        buildingExplosions[currentBuildingExplosion].GetComponent<RandomSfx>().PlaySFX();
        currentBuildingExplosion++;
    }
}
