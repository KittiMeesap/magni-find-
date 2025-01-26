// InteractableClockNumber.cs
using UnityEngine;

public class InteractableClockNumber : MonoBehaviour
{
    public GameObject targetPosition; // ���˹��������
    public Vector3 correctScale; // ��Ҵ���١��ͧ
    public float scaleTolerance = 0.1f; // ��Ҥ�Ҵ����͹��Ҵ�������Ѻ��
    public float snapDistance = 0.5f; // ���� Snap

    private bool isSnapped = false;
    private bool isDragging = false;
    private static InteractableClockNumber selectedNumber; // ����Ţ�����ѧ�١���͡

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

        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped && selectedNumber == this)
        {
            HandleScaling();
        }
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && !isSnapped)
        {
            isDragging = true;
            selectedNumber = this; // ��駤�ҵ���Ţ���١���͡
        }
        else if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped)
        {
            selectedNumber = this; // ��駤�ҵ���Ţ���١���͡����͡������ Magnifier
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

        // ��Ѻ Order in Layer Ŵŧ 1
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder -= 1;
        }

        Debug.Log($"{gameObject.name} snapped to target successfully!");
        ClockMinigame.Instance.CompletePart();
    }
}
