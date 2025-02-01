using UnityEngine;

public class InteractableObjectVaseMinigame : MonoBehaviour
{
    public GameObject targetObject; // วัตถุเป้าหมาย (ที่ต้องประกอบเข้าด้วยกัน)
    public float snapDistance = 0.5f; // ระยะที่ดึงวัตถุเข้าไป
    public Vector3 correctScale; // ขนาดที่ถูกต้อง
    public float scaleTolerance = 0.1f; // ความคลาดเคลื่อนของขนาดที่ยอมรับได้
    public float scaleStep = 0.1f; // ค่าการเพิ่ม/ลดของการขยาย
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // ขนาดต่ำสุด
    public Vector3 maxScale = new Vector3(3f, 3f, 3f); // ขนาดสูงสุด
    public float snapYOffset = 0.0f; // Offset แกน Y สำหรับตำแหน่ง Snap

    private bool isDragging = false;
    private bool isSnapped = false; // เพิ่มตัวแปรสถานะเพื่อบล็อกการ drag เมื่อ snap แล้ว

    [SerializeField] private Material defaultMaterial; // วัสดุเริ่มต้น
    [SerializeField] private Material highlightMaterial; // วัสดุเมื่อ Drag หรือ Resize


    void Update()
    {
        // Handle dragging in Hand mode
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging && !isSnapped)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;

            // ตรวจสอบเงื่อนไขก่อน Snap
            if (targetObject != null && IsScaleCorrect() && IsWithinSnapDistance())
            {
                SnapToTarget(); // Snap เข้าหาเป้าหมาย
            }
        }

        // Handle resizing in Magnify mode
        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped)
        {
            HandleMagnify();
        }
    }

    private void OnMouseDown()
    {
        if (!isSnapped) // ถ้า snap แล้วจะไม่สามารถ drag ได้
        {
            if (ToolManager.Instance.CurrentMode == "Hand")
            {
                // Start dragging only if clicked on this object
                isDragging = true;

                // เปลี่ยนวัสดุเป็น highlightMaterial เมื่อ Drag เริ่มต้น
                ChangeMaterial(highlightMaterial);
            }
            else if (ToolManager.Instance.CurrentMode == "Magnifier")
            {
                // Set this object as the target for magnifying
                ToolManager.Instance.SetSelectedObject(gameObject);

                // เปลี่ยนวัสดุเป็น highlightMaterial เมื่อเลือก Resize
                ChangeMaterial(highlightMaterial);
            }
        }
    }


    private void OnMouseUp()
    {
        isDragging = false;

        // เปลี่ยนวัสดุกลับเป็น defaultMaterial เมื่อปล่อยวัตถุ
        ChangeMaterial(defaultMaterial);
    }


    private void HandleMagnify()
    {
        GameObject selectedObject = ToolManager.Instance.GetSelectedObject();
        if (selectedObject == gameObject && !isSnapped) // ตรวจสอบว่า snap แล้วหรือยัง
        {
            if (Input.GetMouseButtonDown(0)) // คลิกซ้ายเพื่อขยาย
            {
                ModifyScale(scaleStep);
            }
            else if (Input.GetMouseButtonDown(1)) // คลิกขวาเพื่อลดขนาด
            {
                ModifyScale(-scaleStep);
            }
        }
    }

    private void ModifyScale(float scaleChange)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

        // Clamp scale within min and max
        newScale.x = Mathf.Clamp(newScale.x, minScale.x, maxScale.x);
        newScale.y = Mathf.Clamp(newScale.y, minScale.y, maxScale.y);
        newScale.z = Mathf.Clamp(newScale.z, minScale.z, maxScale.z);

        transform.localScale = newScale;

        // เปลี่ยนวัสดุเป็น highlightMaterial ระหว่างการ Resize
        ChangeMaterial(highlightMaterial);

        Debug.Log($"Modified scale of {gameObject.name} to {newScale}");
    }

    private void ChangeMaterial(Material newMaterial)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && newMaterial != null)
        {
            spriteRenderer.material = newMaterial;
        }
    }



    private bool IsScaleCorrect()
    {
        // ตรวจสอบว่า Scale ของวัตถุเท่ากับ Scale ที่กำหนดไว้ทุกแกน
        return transform.localScale == correctScale;
    }

    private bool IsWithinSnapDistance()
    {
        // ตรวจสอบว่าระยะห่างระหว่างวัตถุและเป้าหมาย <= snapDistance
        return Vector3.Distance(transform.position, targetObject.transform.position) <= snapDistance;
    }

    private void SnapToTarget()
    {
        // คำนวณตำแหน่ง Snap ด้วย Offset แกน Y
        Vector3 snapPosition = targetObject.transform.position;
        snapPosition.y += snapYOffset;

        // Snap ไปยังตำแหน่งเป้าหมาย
        transform.position = snapPosition;

        // ตรวจสอบว่ามีการประกอบและขนาดถูกต้องหรือไม่
        if (IsScaleCorrect())
        {
            Debug.Log($"Snapped to target {targetObject.name} successfully!");
            isSnapped = true; // ตั้งค่า isSnapped เป็น true เพื่อบล็อกการ drag
            VaseMinigame.Instance.CompletePart(); // แจ้งว่าชิ้นนี้สำเร็จแล้ว
        }
    }

}
