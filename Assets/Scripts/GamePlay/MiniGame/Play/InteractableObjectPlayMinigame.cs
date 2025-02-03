using TMPro;
using UnityEngine;

public class InteractableObjectPlayMinigame : MonoBehaviour
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
    private bool isSnapped = false; // Block dragging after snapping
    private bool isSelected = false; // Flag to track if object is selected

    [SerializeField] private Material defaultMaterial; // Default material
    [SerializeField] private Material highlightMaterial; // Highlight material during interaction

    private void Update()
    {
        // Handle dragging in Hand mode (only if object is not snapped)
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging && !isSnapped)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;

            // Check for snap conditions (correct scale and within snap distance)
            if (IsScaleCorrect() && IsWithinSnapDistance())
            {
                SnapToTarget(); // Snap to target if conditions met
            }
        }
    }

    private void OnMouseDown()
    {
        // Only allow selection in "Hand" mode and if not snapped
        if (!isSnapped && ToolManager.Instance.CurrentMode == "Hand")
        {
            isDragging = true; // Start dragging
            isSelected = true; // Mark as selected
            ToolManager.Instance.SetSelectedObject(gameObject); // Set selected object
            ChangeMaterial(highlightMaterial); // Highlight object during dragging
            Debug.Log("Object clicked: " + gameObject.name); // Log when clicked
        }
    }

    private void OnMouseUp()
    {
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            isDragging = false; // Stop dragging
            ChangeMaterial(defaultMaterial); // Revert material when dragging stops
            Debug.Log("Object released: " + gameObject.name); // Log when released
        }
    }

    private void ChangeMaterial(Material newMaterial)
    {
        // Update material of the object
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && newMaterial != null)
        {
            spriteRenderer.material = newMaterial;
        }
    }

    private bool IsScaleCorrect()
    {
        // Check if the object scale matches the correct scale within tolerance
        return Vector3.Distance(transform.localScale, correctScale) <= scaleTolerance;
    }

    private bool IsWithinSnapDistance()
    {
        // Check if the object is within snap distance of the target
        return targetObject != null && Vector3.Distance(transform.position, targetObject.transform.position) <= snapDistance;
    }

    private void SnapToTarget()
    {
        // Calculate snap position with Y offset
        Vector3 snapPosition = targetObject.transform.position;
        snapPosition.y += snapYOffset;

        transform.position = snapPosition;

        // Snap successful if scale is correct
        if (IsScaleCorrect())
        {
            Debug.Log($"Snapped to target {targetObject.name} successfully!");
            isSnapped = true; // Block further dragging after snapping
            PlayMinigame.Instance.CompletePart(); // Notify the game that this part is complete
        }
        else
        {
            // Optional: Change material or log feedback if scale is incorrect
            ChangeMaterial(defaultMaterial); // Reset material if scale is incorrect
            Debug.Log("Scale is incorrect, unable to snap.");
        }
    }
}
