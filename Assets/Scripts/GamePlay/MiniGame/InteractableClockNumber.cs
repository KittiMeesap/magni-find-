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

    private bool isSnapped = false;
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
            selectedNumber = this; // ตั้งค่าตัวเลขที่ถูกเลือก

            // เปลี่ยนวัสดุเป็นวัสดุลาก
            if (spriteRenderer != null && draggingMaterial != null)
            {
                spriteRenderer.material = draggingMaterial;
            }
        }
        else if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped)
        {
            selectedNumber = this; // ตั้งค่าตัวเลขที่ถูกเลือกเมื่อกดในโหมด Magnifier
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // เปลี่ยนวัสดุกลับเป็นวัสดุปกติ
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

    private void HandleScaling()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // คลิกซ้าย (ขยาย)
            if (spriteRenderer != null && magnifierMaterial != null)
            {
                spriteRenderer.material = magnifierMaterial; // เปลี่ยนเป็นวัสดุสำหรับย่อขยาย
            }
            ModifyScale(0.1f);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // คลิกขวา (ย่อ)
            if (spriteRenderer != null && magnifierMaterial != null)
            {
                spriteRenderer.material = magnifierMaterial; // เปลี่ยนเป็นวัสดุสำหรับย่อขยาย
            }
            ModifyScale(-0.1f);
        }

        // เมื่อปล่อยปุ่มคลิก ให้เปลี่ยนวัสดุกลับเป็น Default
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            if (spriteRenderer != null && defaultMaterial != null)
            {
                spriteRenderer.material = defaultMaterial;
            }
        }
    }


    private void ModifyScale(float scaleStep)
    {
        // ขั้นตอนของ scale (กำหนด step size เป็น 0.1)
        float stepSize = 0.1f;

        // คำนวณค่าใหม่ในแต่ละแกนโดยการ snap ตาม step size
        float newX = Mathf.Round((transform.localScale.x + scaleStep) / stepSize) * stepSize;
        float newY = Mathf.Round((transform.localScale.y + scaleStep) / stepSize) * stepSize;
        float newZ = Mathf.Round((transform.localScale.z + scaleStep) / stepSize) * stepSize;

        // Clamp ค่า scale ให้อยู่ในช่วงที่กำหนด (0.5 - 0.9)
        newX = Mathf.Clamp(newX, 0.5f, 0.9f);
        newY = Mathf.Clamp(newY, 0.5f, 0.9f);
        newZ = Mathf.Clamp(newZ, 0.5f, 0.9f);

        // ตั้งค่า scale ใหม่
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

        // ปิดการทำงานของ Collider เมื่อ Snap สำเร็จ
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // ปรับ Order in Layer ลดลง 1
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder -= 1;
        }

        Debug.Log($"{gameObject.name} snapped to target successfully!");
        ClockMinigame.Instance.CompletePart();
    }
}
