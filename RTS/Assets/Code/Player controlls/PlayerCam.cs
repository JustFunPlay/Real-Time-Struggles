using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerCam : MonoBehaviour
{
    public static PlayerCam instance;
    [Header("Camera Movement")]
    public float edgeMoveSpeed;
    /// <summary>
    /// The maximum speed at which the camera moves in units per second while the cursor is near the edge of the window
    /// </summary>
    public float mouseMoveSpeed;
    /// <summary>
    /// The avarage speed at which the camera moves in units per second while holding the right mouse button
    /// </summary>
    public float moveMargin;
    /// <summary>
    /// The area of the window in which the camera will move
    /// </summary>
    public float rotateSpeed;
    /// <summary>
    /// The speed at which the camera rotates in degrees per second
    /// </summary>
    public float zoomSpeed;
    /// <summary>
    /// The speed at which the camera zooms in or out
    /// </summary>
    public Maplimiter camBoundary;
    public Vector2 zoomBoundary;
    /// <summary>
    /// The minimum and maximum distance from origin;
    /// </summary>
    public Camera cam;
    Vector3 camDir;
    float currentCamDistance;
    float zoomValue;
    float rotateValue;

    bool holdLeftClick;
    bool holdRightClick;
    Vector2 mouseValue;
    float holdDuration;

    [Header("Player")]
    public static Army playerArmy;
    public List<UnitBase> selectedUnits = new List<UnitBase>();
    public List<GroupedUnits> groups;
    public LayerMask selectionLayer;
    public LayerMask groundLayer;

    [Header("Selection")]
    public RectTransform multiSelectTransform;
    Vector3 selectStartPos;
    bool addSelect;
    public bool inBuildMode;

    [Header("Menus")]
    public GameObject[] contextMenus;
    public GameObject baseMenu;
    public HolographicBuilding newBuilding;

    private void Start()
    {
        instance = this;
        cam = GetComponentInChildren<Camera>();
        currentCamDistance = Vector3.Distance(transform.position, cam.transform.position);
        camDir = (cam.transform.position - transform.position).normalized;
        for (int i = 0; i < 10; i++)
        {
            groups.Add(new GroupedUnits());
        }
        multiSelectTransform.gameObject.SetActive(false);
    }

    public void RotateCamInput(InputAction.CallbackContext callbackContext)
    {
        rotateValue = callbackContext.ReadValue<float>();
    }
    public void ZoomCamInput(InputAction.CallbackContext callbackContext)
    {
        zoomValue = callbackContext.ReadValue<float>();
    }
    public void LeftClick(InputAction.CallbackContext callbackContext)
    {
        Ray clickRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (inBuildMode && callbackContext.started)
        {
            newBuilding.SpawnBuilding(out bool isPlaced);
            if (isPlaced)
                inBuildMode = false;
        }
        else if (!holdRightClick && callbackContext.started)
        {
            holdLeftClick = true;
            holdDuration = 0;
            selectStartPos = Mouse.current.position.ReadValue();
            multiSelectTransform.anchoredPosition = new Vector3();
            multiSelectTransform.sizeDelta = new Vector2();
        }
        else if (holdLeftClick && callbackContext.canceled && holdDuration >= 0.2f)
        {
            multiSelectTransform.gameObject.SetActive(false);
            holdLeftClick = false;
            if (!addSelect)
            {
                for (int i = selectedUnits.Count - 1; i >= 0; i--)
                {
                    selectedUnits[i].OnDeselected();
                }
            }
            Bounds bounds = GetViewportBounds(selectStartPos, Mouse.current.position.ReadValue());
            foreach (UnitBase Unit in PlayerTroopManager.instance.playerUnits)
            {
                if (Unit.GetComponent<TroopMovement>() && bounds.Contains(Unit.GetComponent<TroopMovement>().ToViewportSpace(cam)))
                    Unit.OnSelected();
            }
            SelectCheck();
        }
        else if (holdLeftClick && callbackContext.canceled)
        {
            multiSelectTransform.gameObject.SetActive(false);
            if (IsHoveringOverUI())
            {

            }
            else if (Physics.Raycast(clickRay, out hit, 500f, selectionLayer))
            {
                if (hit.collider.GetComponent<SupplyYard>())
                {
                    foreach (UnitBase unit in selectedUnits)
                    {
                        if (unit.GetComponent<SupplyTruck>())
                        {
                            unit.GetComponent<SupplyTruck>().assignedYard = hit.collider.GetComponent<SupplyYard>();
                            unit.GetComponent<SupplyTruck>().MoveToPosition(hit.collider.GetComponent<SupplyYard>().entranceLocation.position);
                            unit.GetComponent<SupplyTruck>().CheckToAutomate();
                        }
                    }
                }
                else if (hit.collider.GetComponent<UnitBase>()?.army == playerArmy)
                {
                    if (hit.collider.GetComponent<SupplyDepot>() && selectedUnits.Count > 0)
                    {
                        foreach (UnitBase unit in selectedUnits)
                        {
                            if (unit.GetComponent<SupplyTruck>())
                            {
                                unit.GetComponent<SupplyTruck>().assignedDepot = hit.collider.GetComponent<SupplyDepot>();
                                unit.GetComponent<SupplyTruck>().CheckToAutomate();
                            }
                        }
                    }
                    else
                    {
                        if (!addSelect || hit.collider.GetComponent<Building>())
                        {
                            for (int i = selectedUnits.Count - 1; i >= 0; i--)
                            {
                                selectedUnits[i].OnDeselected();
                            }
                        }
                        if (!selectedUnits.Contains(hit.collider.GetComponent<UnitBase>()))
                            hit.collider.GetComponent<UnitBase>().OnSelected();
                        else if (addSelect)
                            hit.collider.GetComponent<UnitBase>().OnDeselected();
                    }
                }
            }
            else if (selectedUnits.Count > 0 && Physics.Raycast(clickRay, out hit, 500f, groundLayer))
            {
                for (int i = selectedUnits.Count - 1; i >= 0; i--)
                {
                    if (selectedUnits[i].GetComponent<TroopMovement>())
                        selectedUnits[i].GetComponent<TroopMovement>().MoveToPosition(hit.point);
                    else
                        selectedUnits[i].OnDeselected();
                }
            }
            holdLeftClick = false;
            SelectCheck();
        }
    }
    public void RightClick(InputAction.CallbackContext callbackContext)
    {
        if (!holdLeftClick && callbackContext.started)
        {
            holdRightClick = true;
            mouseValue = new Vector2();
            holdDuration = 0;
        }
        else if (holdRightClick && callbackContext.canceled && holdDuration >=0.2f)
        {
            holdRightClick = false;
        }
        else if (holdRightClick && inBuildMode && callbackContext.canceled)
        {
            Destroy(newBuilding.gameObject);
            inBuildMode = false;
            holdRightClick = false;
        }
        else if (holdRightClick && callbackContext.canceled)
        {
            for (int i = selectedUnits.Count - 1; i >= 0; i--)
            {
                selectedUnits[i].OnDeselected();
            }
            holdRightClick = false;
            SelectCheck();
        }
    }
    public void MoveMouse(InputAction.CallbackContext callbackContext)
    {
        if (holdRightClick)
            mouseValue += callbackContext.ReadValue<Vector2>();
    }
    public void RotateBuilding(InputAction.CallbackContext callbackContext)
    {
        if (inBuildMode && newBuilding && callbackContext.started)
            newBuilding.RotateBuilding();
    }
    public void AdditiveSelect(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            addSelect = true;
        else if (callbackContext.canceled)
            addSelect = false;
    }

    public void GroupSelect1(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(1);
    }
    public void GroupSelect2(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(2);
    }
    public void GroupSelect3(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(3);
    }
    public void GroupSelect4(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(4);
    }
    public void GroupSelect5(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(5);
    }
    public void GroupSelect6(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(6);
    }
    public void GroupSelect7(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(7);
    }
    public void GroupSelect8(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(8);
    }
    public void GroupSelect9(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(9);
    }
    public void GroupSelect0(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
            GroupSelection(0);
    }
    void GroupSelection(int group)
    {
        if (addSelect)
            groups[group] = new GroupedUnits(selectedUnits);
        else
        {
            for (int i = selectedUnits.Count - 1; i >= 0; i--)
            {
                selectedUnits[i].OnDeselected();
            }
            for (int t = groups[group].groupedUnits.Count - 1; t >= 0; t--)
            {
                if (groups[group].groupedUnits[t])
                    groups[group].groupedUnits[t].OnSelected();
                else
                    groups[group].groupedUnits.RemoveAt(t);
            }
            SelectCheck();
        }
    }

    void SelectCheck()
    {
        foreach (GameObject menu in contextMenus)
        {
            menu.SetActive(false);
        }
        if (selectedUnits.Count > 0)
        {
            baseMenu.SetActive(false);
            if (selectedUnits[0].type == UnitType.HeadQuarters)
                contextMenus[0].SetActive(true);
            else if (selectedUnits[0].type == UnitType.Factory)
                contextMenus[1].SetActive(true);
        }
        else
        {
            baseMenu.SetActive(true);
        }
    }


    private void Update()
    {
        if (!holdLeftClick)
            MoveCam();
        else if (holdDuration >= 0.2f)
        {
            multiSelectTransform.gameObject.SetActive(true);
            MultiSelect();
        }
        ZoomCam();
        transform.Rotate(0, -rotateValue * rotateSpeed * Time.deltaTime, 0);
        if (holdLeftClick || holdRightClick)
            holdDuration += Time.deltaTime;
    }

    void MoveCam()
    {
        Vector3 moveDir = new Vector3();
        if (holdRightClick)
        {
            if (holdDuration > 0.2f)
            {
                moveDir.x = mouseValue.x * mouseMoveSpeed * Time.deltaTime;
                moveDir.z = mouseValue.y * mouseMoveSpeed * Time.deltaTime;
            }
        }
        else
        {
            Vector2 resolution = new Vector2(Screen.width, Screen.height);
            Vector2 actualMargin = new Vector2(resolution.x * moveMargin, resolution.y * moveMargin);
            if (Application.isFocused)
            {
                if (Mouse.current.position.x.ReadValue() <= actualMargin.x && Mouse.current.position.x.ReadValue() >= 0)
                {
                    float perc = 1 - (Mouse.current.position.x.ReadValue() / actualMargin.x);
                    moveDir.x = -edgeMoveSpeed * Time.deltaTime * perc;
                }
                else if (Mouse.current.position.x.ReadValue() >= resolution.x - actualMargin.x && Mouse.current.position.x.ReadValue() <= resolution.x)
                {
                    float perc = 1 - ((resolution.x - Mouse.current.position.x.ReadValue()) / actualMargin.x);
                    moveDir.x = edgeMoveSpeed * Time.deltaTime * perc;
                }
                if (Mouse.current.position.y.ReadValue() <= actualMargin.y && Mouse.current.position.y.ReadValue() >= 0)
                {
                    float perc = 1 - (Mouse.current.position.y.ReadValue() / actualMargin.y);
                    moveDir.z = -edgeMoveSpeed * Time.deltaTime * perc;
                }
                else if (Mouse.current.position.y.ReadValue() >= resolution.y - actualMargin.y && Mouse.current.position.y.ReadValue() <= resolution.y)
                {
                    float perc = 1 - ((resolution.y - Mouse.current.position.y.ReadValue()) / actualMargin.y);
                    moveDir.z = edgeMoveSpeed * Time.deltaTime * perc;
                }
            }
        }
        transform.Translate(moveDir);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, camBoundary.xLimit.x, camBoundary.xLimit.y), transform.position.y, Mathf.Clamp(transform.position.z, camBoundary.zLimit.x, camBoundary.zLimit.y));
    }
    void ZoomCam()
    {
        currentCamDistance = Mathf.Clamp(currentCamDistance += zoomValue * zoomSpeed * Time.deltaTime, zoomBoundary.x, zoomBoundary.y);
        cam.transform.localPosition = currentCamDistance * camDir;
    }

    void MultiSelect()
    {
        Vector3[] newCanvasRect = RectSelect(selectStartPos, Mouse.current.position.ReadValue());
        multiSelectTransform.anchoredPosition = newCanvasRect[0];
        multiSelectTransform.sizeDelta = new Vector2(newCanvasRect[1].x, newCanvasRect[1].y);
    }

    Bounds GetViewportBounds(Vector3 anchor, Vector3 cursor)
    {
        Vector3 anchorViewport = cam.ScreenToViewportPoint(anchor);
        Vector3 cursorViewport = cam.ScreenToViewportPoint(cursor);
        Vector3 min = Vector3.Min(anchorViewport, cursorViewport);
        Vector3 max = Vector3.Max(anchorViewport, cursorViewport);

        min.z = cam.nearClipPlane;
        max.z = cam.farClipPlane;
        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    Vector3[] RectSelect(Vector3 anchor, Vector3 cursor)
    {
        Vector2 scaleMod = new Vector2(Screen.width, Screen.height);
        scaleMod.x /= 1920;
        scaleMod.y /= 1080;
        Vector3[] selectBounds = new Vector3[2];

        Vector3 min = Vector3.Min(anchor, cursor);
        Vector3 max = Vector3.Max(anchor, cursor);

        min.x /= scaleMod.x;
        min.y /= scaleMod.y;
        max.x /= scaleMod.x;
        max.y /= scaleMod.y;

        selectBounds[0] = (max - min) / 2 + min;
        selectBounds[1] = (max - min);

        return selectBounds;
    }

    bool IsHoveringOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}

[System.Serializable]
public class Maplimiter
{
    public Vector2 xLimit;
    public Vector2 zLimit;
}
[System.Serializable]
public class GroupedUnits
{
    public List<UnitBase> groupedUnits;
    public GroupedUnits(List<UnitBase> newUnits)
    {
        groupedUnits = new List<UnitBase>();
        for (int i = 0; i < newUnits.Count; i++)
        {
            groupedUnits.Add(newUnits[i]);
        }
    }
    public GroupedUnits()
    {
        groupedUnits = new List<UnitBase>();
    }

}