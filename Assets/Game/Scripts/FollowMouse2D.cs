using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class FollowMouse2D : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Canvas canvas; // Assign your Canvas in the Inspector
    public RectTransform rectTransform;
    public Vector2 pointerOffset, gamepadOffset;

    // Reference to your Input Actions asset
    public InputActionAsset uiInputActions;
    private InputAction pointerPositionAction;

    ManagerInputCalls inputCall => ManagerInputCalls.Instance;

    void Awake()
    {
        //rectTransform = GetComponent<RectTransform>();
        // Find the PointerPosition action from your Input Actions asset
        pointerPositionAction = uiInputActions.FindAction("UI/PointerPosition");
    }

    void OnEnable()
    {
        // Enable the action when the script is enabled
        if (pointerPositionAction != null) pointerPositionAction.Enable();
    }
    void OnDisable()
    {
        // Disable the action when the script is disabled
        if (pointerPositionAction != null)
            pointerPositionAction.Disable();
    }

   /* public void OnBeginDrag(PointerEventData eventData)
    {
        // Calculate the offset between the pointer and the UI element's pivot
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
    }*/

    /*public void OnDrag(PointerEventData eventData)
    {
        return;
        // Get the current pointer position from the new Input System
        Vector2 currentPointerPosition = pointerPositionAction.ReadValue<Vector2>();

        // Convert screen position to local position within the Canvas
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent.GetComponent<RectTransform>(), currentPointerPosition, eventData.pressEventCamera, out Vector2 localPointerPosition))
        {
            // Set the new position of the UI element, accounting for the offset
            rectTransform.localPosition = localPointerPosition - pointerOffset;
        }
    }*/

    /*public void OnEndDrag(PointerEventData eventData)
    {
        // Optional: Add logic for when dragging stops (e.g., snapping to a grid)
    }*/

    /*public void OnPointerClick(PointerEventData eventData)
    {
        _isHold = !_isHold;
        _eventData = eventData;
    }
    bool _isHold;
    PointerEventData _eventData;*/
    private void Update()
    {
        //if (!_isHold || _eventData == null) return;
        if (inputCall._input.currentControlScheme == "Gamepad") OverSelect(); 
        else Drag();
    }
    public void OverSelect()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(ManagerGame.Instance._goSelector.transform.position);
        //SPACE CAMERA
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPoint,
            canvas.worldCamera, // Or the camera used by your Screen Space - Camera canvas
            out Vector2 localPoint // The resulting local position within the Canvas
        );

        // Now you can set the UI element's anchoredPosition
        rectTransform.anchoredPosition = localPoint + gamepadOffset;
        if (!ManagerGame.Instance._goSelector.activeInHierarchy)
        {
            gameObject.SetActive(false);
            rectTransform.anchoredPosition = Vector2.up * 1000f;
        }
    }
    public void Drag()
    {
        if (ManagerGame.Instance._eventData == null) return;
        Vector2 currentPointerPosition = pointerPositionAction.ReadValue<Vector2>();
        //OVERLAY
        /*
        // Convert screen position to local position within the Canvas
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent.GetComponent<RectTransform>(),
            currentPointerPosition, ManagerGame.Instance._eventData.pressEventCamera,
            out Vector2 localPointerPosition))
        {
            // Set the new position of the UI element, accounting for the offset
            if (localPointerPosition != Vector2.zero)
                rectTransform.localPosition = localPointerPosition - pointerOffset;
        }*/

        //SPACE CAMERA
        // Convert mouse position from screen space to local space on the canvas
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            currentPointerPosition,
            canvas.worldCamera, // Use the camera assigned to the Canvas
            out localPoint
        );

        // Set the anchored position of the UI Image
        rectTransform.anchoredPosition = localPoint - pointerOffset;
    }
}
