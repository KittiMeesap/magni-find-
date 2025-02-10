using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableObjectPlayMinigame : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject targetObject; // Target object to snap to
    public float snapDistance = 0.5f; // Distance threshold for snapping
    public Vector3 correctScale; // Correct scale of the object
    public float scaleTolerance = 0.1f; // Acceptable scale tolerance
    public float scaleStep = 0.1f; // Scale change per click
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // Minimum scale
    public Vector3 maxScale = new Vector3(3f, 3f, 3f); // Maximum scale
    public float snapYOffset = 0.0f; // Y offset for snap position

    private bool isDragging = false;
    private bool isSnapped = false; // ✅ ห้ามดึงออกหลัง snap
    private bool isSelected = false; // Flag to track if object is selected

    private RectTransform rectTransform;
    private Canvas canvas;

    private Vector2 originalPointerPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // Get RectTransform component
        canvas = GetComponentInParent<Canvas>(); // Get the Canvas that's the parent
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // ถ้าตัวอักษรถูก snap ไปแล้ว ห้ามลาก
        if (isSnapped) return;

        // Record the position where the pointer clicked
        originalPointerPosition = eventData.position;
        isDragging = true;

        // Check if the current mode is "Hand"
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            isSelected = true; // Mark as selected
            ToolManager.Instance.SetSelectedObject(gameObject); // Set selected object
            Debug.Log("Object clicked: " + gameObject.name); // Log when clicked
        }

        // If in "Magnifier" mode and the object is selected, handle zoom
        if (ToolManager.Instance.CurrentMode == "Magnifier" && isSelected)
        {
            HandleMagnify(); // Start zoom functionality
            isDragging = false; // Prevent dragging in magnifier mode
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Prevent dragging in Magnifier mode unless selected
        if (ToolManager.Instance.CurrentMode == "Magnifier" && isSelected)
        {
            HandleMagnify(); // Handle zoom in magnifier mode
            return; // Do nothing for dragging
        }

        if (isDragging)
        {
            // Update the position of the object as we drag the pointer
            Vector2 dragDelta = eventData.position - originalPointerPosition;

            // Update the position while dragging using event data and canvas scale factor
            rectTransform.anchoredPosition += dragDelta / canvas.scaleFactor;

            originalPointerPosition = eventData.position; // Update the position for next frame
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            isDragging = false; // Stop dragging
            Debug.Log("Object released: " + gameObject.name); // Log when released

            // Check letter placement when dragging ends
            if (PlayMinigame.Instance != null)
            {
                // ส่งตัวเองเป็น InteractableObjectPlayMinigame แทน
                PlayMinigame.Instance.CheckLetterPlacement(this);
            }
        }
    }


    private void ModifyScale(float scaleChange)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

        // Clamp scale within min and max
        newScale.x = Mathf.Clamp(newScale.x, minScale.x, maxScale.x);
        newScale.y = Mathf.Clamp(newScale.y, minScale.y, maxScale.y);
        newScale.z = Mathf.Clamp(newScale.z, minScale.z, maxScale.z);

        transform.localScale = newScale;

        Debug.Log($"Modified scale of {gameObject.name} to {newScale}");
    }

    // ✅ เมื่อ snap แล้วให้ห้ามดึงออก
    public void MarkAsSnapped()
    {
        isSnapped = true;
        Debug.Log($"{gameObject.name} is now snapped and locked.");
    }

    // Function to zoom the UI object
    private void HandleMagnify()
    {
        GameObject selectedObject = ToolManager.Instance.GetSelectedObject();
        if (selectedObject == gameObject && !isSnapped) // ตรวจสอบว่า snap แล้วหรือยัง
        {
            if (Input.GetMouseButtonDown(0)) // คลิกซ้ายเพื่อขยาย
            {
                ModifyScale(scaleStep);
            }
            else if (Input.GetMouseButtonDown(1)) // คลิกขวาเพื่อลดขนาด
            {
                ModifyScale(-scaleStep);
            }
        }
    }
}
