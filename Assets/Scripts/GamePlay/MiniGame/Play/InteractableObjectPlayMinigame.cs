using UnityEngine;

public class InteractableObjectPlayMinigame : MonoBehaviour
{
    public GameObject targetObject; // วัตถุเป้าหมาย (ที่ต้องประกอบเข้าด้วยกัน)
    public float snapDistance = 0.5f; // ระยะที่ดึงวัตถุเข้าไป
    public Vector3 correctScale; // ขนาดที่ถูกต้อง
    public float snapYOffset = 0.0f; // Offset แกน Y สำหรับตำแหน่ง Snap

    private bool isDragging = false;  // ตัวแปรเช็คว่ากำลังลากอยู่หรือไม่
    private bool isSnapped = false;   // ตัวแปรเช็คว่าวัตถุถูก Snap แล้วหรือไม่

    [SerializeField] private Material defaultMaterial;  // วัสดุเริ่มต้น
    [SerializeField] private Material highlightMaterial; // วัสดุเมื่อ Drag หรือ Resize

    private void OnMouseDown()
    {
        // เมื่อผู้เล่นคลิกที่วัตถุ, ให้เริ่มลาก
        if (!isSnapped)
        {
            isDragging = true;
            ChangeMaterial(highlightMaterial); // เปลี่ยนวัสดุเมื่อเริ่มลาก
        }
    }

    private void OnMouseDrag()
    {
        // เมื่อเริ่มลากแล้ว, วัตถุจะตามมือลาก
        if (isDragging && !isSnapped)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos; // เคลื่อนที่ไปตามตำแหน่งของเมาส์
        }
    }

    private void OnMouseUp()
    {
        // เมื่อปล่อยเมาส์, ให้ตรวจสอบว่าต้องการ Snap หรือไม่
        isDragging = false;
        ChangeMaterial(defaultMaterial); // เปลี่ยนวัสดุกลับเป็น default

        if (IsWithinSnapDistance() && IsScaleCorrect())
        {
            SnapToTarget(); // Snap วัตถุไปยังตำแหน่งเป้าหมาย
        }
    }

    private void ChangeMaterial(Material newMaterial)
    {
        // เปลี่ยนวัสดุของวัตถุ
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && newMaterial != null)
        {
            spriteRenderer.material = newMaterial;
        }
    }

    private bool IsScaleCorrect()
    {
        // ตรวจสอบว่า Scale ของวัตถุตรงกับที่กำหนดหรือไม่
        return transform.localScale == correctScale;
    }

    private bool IsWithinSnapDistance()
    {
        // ตรวจสอบว่าระยะห่างระหว่างตำแหน่งวัตถุและเป้าหมาย <= snapDistance
        return Vector3.Distance(transform.position, targetObject.transform.position) <= snapDistance;
    }

    private void SnapToTarget()
    {
        // คำนวณตำแหน่ง Snap ด้วย Offset แกน Y
        Vector3 snapPosition = targetObject.transform.position;
        snapPosition.y += snapYOffset;

        // Snap ไปยังตำแหน่งที่กำหนด
        transform.position = snapPosition;

        // ถ้าวัตถุวางในตำแหน่งที่ถูกต้องแล้ว, จะตั้งค่า isSnapped เป็น true
        isSnapped = true;

        Debug.Log($"Snapped to target {targetObject.name} successfully!");
        PlayMinigame.Instance.CompletePart(); // แจ้งว่าเกมทำสำเร็จแล้ว
    }
}
