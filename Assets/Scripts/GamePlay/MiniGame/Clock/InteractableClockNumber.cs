using SpriteGlow;
using UnityEngine;

public class InteractableClockNumber : MonoBehaviour
{
    public GameObject targetPosition; // ตำแหน่งเป้าหมาย
    public GameObject dragBoundsObject; // ✅ Object ที่ใช้เป็นขอบเขตของการลาก
    public Vector3 correctScale;
    public float scaleTolerance = 0.05f;
    public float snapDistance = 0.5f;

    public bool isSnapped = false;
    private bool isDragging = false;
    private static InteractableClockNumber selectedNumber;
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;       // ✅ Sprite ปกติ
    [SerializeField] private Sprite highlightedSprite;   // ✅ Sprite เมื่อเอาเมาส์ไปวาง

    private int originalSortingOrder; // ✅ เก็บค่า Sorting Order เดิม
    private Bounds dragBounds; // ✅ ขอบเขตของการลาก
    private Vector3 dragCenter; // ✅ ศูนย์กลางของขอบลาก
    private float dragRadius; // ✅ รัศมีของขอบลาก (ถ้าเป็นวงกลม)

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite; // ตั้งค่า Sprite เริ่มต้น
        }
        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name} is missing a SpriteRenderer!");
        }
        else
        {
            originalSortingOrder = spriteRenderer.sortingOrder; // ✅ เก็บค่า Sorting Order เดิม
        }

        if (dragBoundsObject != null)
        {
            GetObjectBounds(dragBoundsObject); // ✅ ดึงขอบเขตจาก Object ที่กำหนด
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

            // ✅ ทำให้ Object ที่ถูกเลือกไปอยู่หน้าสุด
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 999;
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

        // ✅ คืนค่า Sorting Order กลับเป็นปกติ
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }

    private void DragObject()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (dragBoundsObject != null)
        {
            if (dragRadius > 0)
            {
                // ✅ ถ้าเป็นวงกลม → จำกัดการลากในรัศมี
                Vector2 direction = (mousePosition - (Vector2)dragCenter).normalized;
                float distance = Vector2.Distance(mousePosition, dragCenter);
                float clampedDistance = Mathf.Min(distance, dragRadius);
                transform.position = dragCenter + (Vector3)(direction * clampedDistance);
            }
            else
            {
                // ✅ ถ้าเป็น BoxCollider → จำกัดการลากเป็นสี่เหลี่ยม
                float clampedX = Mathf.Clamp(mousePosition.x, dragBounds.min.x, dragBounds.max.x);
                float clampedY = Mathf.Clamp(mousePosition.y, dragBounds.min.y, dragBounds.max.y);
                transform.position = new Vector2(clampedX, clampedY);
            }
        }
        else
        {
            transform.position = mousePosition; // ถ้าไม่มีขอบเขต ให้ลากได้อิสระ
        }
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

    // ✅ **ฟังก์ชันดึงขอบเขตของ Object ที่ใช้เป็นขอบลาก**
    private void GetObjectBounds(GameObject obj)
    {
        if (obj == null) return;

        // ✅ ถ้ามี CircleCollider2D → กำหนดเป็นวงกลม
        CircleCollider2D circleCollider = obj.GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            dragCenter = obj.transform.position;
            dragRadius = circleCollider.radius * obj.transform.lossyScale.x;
            return;
        }

        // ✅ ถ้าเป็น BoxCollider2D → ใช้ขอบสี่เหลี่ยมปกติ
        BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            dragBounds = boxCollider.bounds;
            dragRadius = 0; // ตั้งค่าเป็น 0 เพราะใช้แบบสี่เหลี่ยม
            return;
        }

        // ✅ ถ้าไม่มี Collider → ใช้ SpriteRenderer
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            dragBounds = spriteRenderer.bounds;
            dragRadius = 0; // ใช้แบบสี่เหลี่ยม
            return;
        }

        Debug.LogWarning($"{obj.name} has no valid Collider or SpriteRenderer! Using default bounds.");
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

        if (MinigameManager.Instance.IsPlayingMinigame && ToolManager.Instance.CurrentMode == "Hand" || ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite; // ✅ เปลี่ยนเป็น Sprite ไฮไลท์
            }
        }
    }

    private void OnMouseExit()
    {
        if (selectedNumber == this)
        {
            selectedNumber = null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite; // ✅ กลับเป็น Sprite ปกติ
        }
    }
}
