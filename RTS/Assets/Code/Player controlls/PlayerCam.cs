using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
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
    Camera cam;
    Vector3 camDir;
    float currentCamDistance;
    float zoomValue;
    float rotateValue;

    bool holdLeftClick;
    bool holdRightClick;
    Vector2 mouseValue;
    float holdDuration;

    [Header("Player")]
    public Army playerArmy;
    public List<UnitBase> selectedUnits = new List<UnitBase>();
    public List<GroupedUnits> groups;
    public LayerMask selectionLayer;
    public LayerMask groundLayer;

    // selection stuff
    Vector3 selectStartPos;
    bool addSelect;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        currentCamDistance = Vector3.Distance(transform.position, cam.transform.position);
        camDir = (cam.transform.position - transform.position).normalized;
        for (int i = 0; i < 10; i++)
        {
            groups.Add(new GroupedUnits());
        }
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
        if (!holdRightClick && callbackContext.started)
        {
            holdLeftClick = true;
            holdDuration = 0;
            selectStartPos = Mouse.current.position.ReadValue();
        }
        else if (holdLeftClick && callbackContext.canceled && holdDuration >= 0.2f)
        {
            holdLeftClick = false;

        }
        else if (holdLeftClick && callbackContext.canceled)
        {
            
            if (Physics.Raycast(clickRay, out hit, 100f, selectionLayer))
            {
                if (hit.collider.GetComponent<UnitBase>()?.army == playerArmy)
                {
                    if (!addSelect)
                    {
                        for (int i = selectedUnits.Count -1; i >= 0; i--)
                        {
                            selectedUnits[i].OnDeselected(this);
                        }
                    }
                    if (!selectedUnits.Contains(hit.collider.GetComponent<UnitBase>()))
                        hit.collider.GetComponent<UnitBase>().OnSelected(this);
                    else if (addSelect)
                        hit.collider.GetComponent<UnitBase>().OnDeselected(this);
                }
            }
            else if (selectedUnits.Count > 0 && Physics.Raycast(clickRay, out hit, 100f, groundLayer))
            {
                foreach (TroopMovement troop in selectedUnits)
                    troop.moveToPosition(hit.point);
            }
            holdLeftClick = false;
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
        else if (holdRightClick && callbackContext.canceled)
        {
            for (int i = selectedUnits.Count - 1; i >= 0; i--)
            {
                selectedUnits[i].OnDeselected(this);
            }
            holdRightClick = false;
        }
    }
    public void MoveMouse(InputAction.CallbackContext callbackContext)
    {
        if (holdRightClick)
            mouseValue += callbackContext.ReadValue<Vector2>();
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
                selectedUnits[i].OnDeselected(this);
            }
            foreach (TroopMovement troop in groups[group].groupedUnits)
            {
                troop.OnSelected(this);
            }
        }
    }

    private void Update()
    {
        if (!holdLeftClick)
            MoveCam();
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
            Vector2 resolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
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
        foreach (UnitBase unit in newUnits)
        {
            groupedUnits.Add(unit);
        }
    }
    public GroupedUnits()
    {
        groupedUnits = new List<UnitBase>();
    }

}