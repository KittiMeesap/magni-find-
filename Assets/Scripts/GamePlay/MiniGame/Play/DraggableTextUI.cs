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
        canvas = GetComponentInParent<Canvas>(); // หา Canvas ที่เป็น Parent
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // บันทึกตำแหน่งเริ่มต้น
        originalPosition = rectTransform.anchoredPosition;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && canvas != null)
        {
            // คำนวณตำแหน่งใหม่โดยใช้ eventData และ ScaleFactor ของ Canvas
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        // ตรวจสอบว่าถูกวางในตำแหน่งที่ถูกต้องหรือไม่
        if (PlayMinigame.Instance != null)
        {
            PlayMinigame.Instance.CheckLetterPlacement(this);
        }
    }

}
