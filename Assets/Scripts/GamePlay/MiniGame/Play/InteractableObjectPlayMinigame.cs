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
    private bool isSnapped = false; // Block dragging after snapping
    private bool isSelected = false; // Flag to track if object is selected

    [SerializeField] private Material defaultMaterial; // Default material
    [SerializeField] private Material highlightMaterial; // Highlight material during interaction

    private TextMeshProUGUI textComponent; // Reference to TextMeshPro component
    private Vector2 originalPointerPosition;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>(); // Initialize TextMeshPro reference
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Record the position where the pointer clicked
        originalPointerPosition = eventData.position;
        isDragging = true;

        if (!isSnapped && ToolManager.Instance.CurrentMode == "Hand")
        {
            isSelected = true; // Mark as selected
            ToolManager.Instance.SetSelectedObject(gameObject); // Set selected object
            ChangeMaterial(highlightMaterial); // Highlight object during dragging
            Debug.Log("Object clicked: " + gameObject.name); // Log when clicked
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // Update the position of the object as we drag the pointer
            Vector2 dragDelta = eventData.position - originalPointerPosition;

            // Increase or decrease the font size based on drag movement
            if (dragDelta.y > 0)
                IncreaseFontSize(0.1f); // Increase font size
            else if (dragDelta.y < 0)
                DecreaseFontSize(0.1f); // Decrease font size

            originalPointerPosition = eventData.position; // Update the position for next frame
        }
    }

    public void OnPointerUp(PointerEventData eventData)
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

    public void IncreaseFontSize(float value)
    {
        if (textComponent != null)
        {
            // Increase font size in TextMeshPro
            textComponent.fontSize += value;
            textComponent.fontSize = Mathf.Clamp(textComponent.fontSize, 10f, 100f); // Clamp font size
        }
    }

    public void DecreaseFontSize(float value)
    {
        if (textComponent != null)
        {
            // Decrease font size in TextMeshPro
            textComponent.fontSize -= value;
            textComponent.fontSize = Mathf.Clamp(textComponent.fontSize, 10f, 100f); // Clamp font size
        }
    }
}
