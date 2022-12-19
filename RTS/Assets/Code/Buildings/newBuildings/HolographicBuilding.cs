using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class HolographicBuilding : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public LayerMask groundLayer;
    public Vector3 halfExtends;
    public Vector3 extendsOffset;
    public MeshRenderer[] meshes;
    public SkinnedMeshRenderer[] skinnedMeshes;
    public ConstructionSite buildingToSpawn;
    bool canPlace;
    
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Physics.Raycast(PlayerCam.instance.cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 500f, groundLayer);
            if (hit.collider)
            {
                transform.position = hit.point;
            }
        }
        int buildings = 0;
        Collider[] colliders = Physics.OverlapBox(transform.position + transform.TransformDirection(extendsOffset), halfExtends, transform.rotation, obstacleLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<Building>() || collider.GetComponent<ConstructionSite>() || collider.GetComponent<SupplyYard>())
                buildings++;
        }
        if (buildings > 0)
        {
            canPlace = false;
            foreach (MeshRenderer renderer in meshes)
            {
                renderer.material.color = new Color(1, 0, 0, 0.4f);
            }
            foreach (SkinnedMeshRenderer skinnedRenderer in skinnedMeshes)
            {
                skinnedRenderer.material.color = new Color(1, 0, 0, 0.4f);
            }
        }
        else
        {
            canPlace = true;
            foreach (MeshRenderer renderer in meshes)
            {
                renderer.material.color = new Color(0, 1, 0, 0.4f);
            }
            foreach (SkinnedMeshRenderer skinnedRenderer in skinnedMeshes)
            {
                skinnedRenderer.material.color = new Color(0, 1, 0, 0.4f);
            }
        }
    }
    public void RotateBuilding()
    {
        transform.Rotate(0, 90, 0);
    }

    public void SpawnBuilding(out bool isPlaced)
    {
        if (canPlace)
        {
            ConstructionSite newConstructionSite = Instantiate(buildingToSpawn, transform.position, transform.rotation);
            newConstructionSite.AddedUnit(PlayerCam.playerArmy);
            isPlaced = true;
            DestroyThis();
        }
        else
            isPlaced = false;
    }
    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
