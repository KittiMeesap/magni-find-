using UnityEngine;

public class InteractableClockNumber : MonoBehaviour
{
    public GameObject targetPosition; // ตำแหน่งเป้าหมาย
    public Vector3 correctScale; // ขนาดที่ถูกต้อง
    public float scaleTolerance = 0.05f; // เพิ่มความยืดหยุ่นเล็กน้อยสำหรับการตรวจสอบขนาด
    public float snapDistance = 0.5f; // ระยะ Snap

    public Material defaultMaterial; // วัสดุปกติ
    public Material draggingMaterial; // วัสดุระหว่างการลาก
    public Material magnifierMaterial; // วัสดุเมื่อย่อขยาย

    public bool isSnapped = false;
    private bool isDragging = false;
    private static InteractableClockNumber selectedNumber; // ตัวเลขที่กำลังถูกเลือก
    private SpriteRenderer spriteRenderer; // Sprite Renderer ของ Object

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name} is missing a SpriteRenderer!");
        }
    }

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
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && !isSnapped)
        {
            isDragging = true;
            selectedNumber = this;

            if (spriteRenderer != null && draggingMaterial != null)
            {
                spriteRenderer.material = draggingMaterial;
            }
        }
        else if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped)
        {
            selectedNumber = this;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (spriteRenderer != null && defaultMaterial != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

    private void OnMouseOver()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ModifyScale(0.1f); // ขยาย
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ModifyScale(-0.1f); // ย่อ
            }
        }
    }

    private void OnMouseExit()
    {
        if (selectedNumber == this)
        {
            selectedNumber = null;
        }

        if (spriteRenderer != null && defaultMaterial != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

    private void DragObject()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    private void ModifyScale(float scaleStep)
    {
        float stepSize = 0.1f;

        float newX = Mathf.Round((transform.localScale.x + scaleStep) / stepSize) * stepSize;
        float newY = Mathf.Round((transform.localScale.y + scaleStep) / stepSize) * stepSize;
        float newZ = Mathf.Round((transform.localScale.z + scaleStep) / stepSize) * stepSize;

        newX = Mathf.Clamp(newX, 0.5f, 0.9f);
        newY = Mathf.Clamp(newY, 0.5f, 0.9f);
        newZ = Mathf.Clamp(newZ, 0.5f, 0.9f);

        transform.localScale = new Vector3(newX, newY, newZ);

        Debug.Log($"Modified Scale: {transform.localScale}");
    }

    private bool IsCloseToTarget()
    {
        float positionDistance = Vector3.Distance(transform.position, targetPosition.transform.position);
        return positionDistance <= snapDistance;
    }

    private bool IsScaleCorrect()
    {
        return Mathf.Approximately(transform.localScale.x, correctScale.x) &&
               Mathf.Approximately(transform.localScale.y, correctScale.y) &&
               Mathf.Approximately(transform.localScale.z, correctScale.z);
    }

    private void SnapToTarget()
    {
        if (!IsCloseToTarget() || !IsScaleCorrect())
        {
            Debug.LogWarning($"Cannot snap {gameObject.name} - Position or Scale incorrect.");
            return;
        }

        transform.position = targetPosition.transform.position;
        isSnapped = true;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder -= 1;
        }

        Debug.Log($"{gameObject.name} snapped to target successfully!");
        ClockMinigame.Instance.CompletePart();
    }
}
