// InteractableClockNumber.cs
using UnityEngine;

public class InteractableClockNumber : MonoBehaviour
{
    public GameObject targetPosition; // ตำแหน่งเป้าหมาย
    public Vector3 correctScale; // ขนาดที่ถูกต้อง
    public float scaleTolerance = 0.1f; // ค่าคลาดเคลื่อนขนาดที่ยอมรับได้
    public float snapDistance = 0.5f; // ระยะ Snap

    private bool isSnapped = false;
    private bool isDragging = false;

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging && !isSnapped)
        {
            DragObject();

            if (IsCloseToTarget() && IsScaleCorrect())
            {
                SnapToTarget();
            }
        }

        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped)
        {
            HandleScaling();
        }
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && !isSnapped)
        {
            isDragging = true;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void DragObject()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    private void HandleScaling()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ModifyScale(0.1f);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ModifyScale(-0.1f);
        }
    }

    private void ModifyScale(float scaleChange)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;
        newScale.x = Mathf.Clamp(newScale.x, correctScale.x - scaleTolerance, correctScale.x + scaleTolerance);
        newScale.y = Mathf.Clamp(newScale.y, correctScale.y - scaleTolerance, correctScale.y + scaleTolerance);
        newScale.z = Mathf.Clamp(newScale.z, correctScale.z - scaleTolerance, correctScale.z + scaleTolerance);
        transform.localScale = newScale;
    }

    private bool IsCloseToTarget()
    {
        float positionDistance = Vector3.Distance(transform.position, targetPosition.transform.position);
        return positionDistance <= snapDistance;
    }

    private bool IsScaleCorrect()
    {
        return Mathf.Abs(transform.localScale.x - correctScale.x) <= scaleTolerance &&
               Mathf.Abs(transform.localScale.y - correctScale.y) <= scaleTolerance &&
               Mathf.Abs(transform.localScale.z - correctScale.z) <= scaleTolerance;
    }

    private void SnapToTarget()
    {
        transform.position = targetPosition.transform.position;
        isSnapped = true;
        Debug.Log($"{gameObject.name} snapped to target successfully!");
        ClockMinigame.Instance.CompletePart();
    }
}
