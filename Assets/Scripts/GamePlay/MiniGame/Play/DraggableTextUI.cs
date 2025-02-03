using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableTextUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;
    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); // Get the Canvas that's the parent
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Record the initial position
        originalPosition = rectTransform.anchoredPosition;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && canvas != null)
        {
            // Update the position while dragging using event data and canvas scale factor
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        // Check letter placement when dragging ends
        if (PlayMinigame.Instance != null)
        {
            PlayMinigame.Instance.CheckLetterPlacement(this);
        }
        else
        {
            // Reset position if not placed correctly (optional)
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
