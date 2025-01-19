using UnityEngine;

public class MagnifyingGlassController : MonoBehaviour
{
    public float zoomAmount = 0.1f; // ปริมาณการย่อ/ขยายต่อคลิก
    private GameObject targetObject; // วัตถุที่ถูกเลือก

    void Update()
    {
        // ตรวจจับการคลิกเลือกวัตถุ
        if (Input.GetMouseButtonDown(0) && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                targetObject = hit.collider.gameObject;
                Debug.Log($"Target selected: {targetObject.name}");
            }
        }

        // ย่อ/ขยายเฉพาะวัตถุที่เลือก
        if (targetObject != null && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (Input.GetMouseButtonDown(0)) // คลิกซ้าย -> ขยาย
            {
                ModifyObjectScale(zoomAmount);
            }
            else if (Input.GetMouseButtonDown(1)) // คลิกขวา -> ย่อ
            {
                ModifyObjectScale(-zoomAmount);
            }
        }
    }

    void ModifyObjectScale(float scaleChange)
    {
        BreakableObject breakable = targetObject.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            breakable.ModifyScale(scaleChange);
        }
        else
        {
            Debug.LogWarning("Selected object is not breakable.");
        }
    }

}
