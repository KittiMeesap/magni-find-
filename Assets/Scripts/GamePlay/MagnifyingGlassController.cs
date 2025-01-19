using UnityEngine;

public class MagnifyingGlassController : MonoBehaviour
{
    public float zoomAmount = 0.1f; // ปริมาณการย่อ/ขยายต่อคลิก

    void Update()
    {
        // ตรวจสอบโหมด Magnifier
        if (ToolManager.Instance.CurrentMode == "Magnifier")
        {
            GameObject targetObject = ToolManager.Instance.GetSelectedObject(); // ใช้วัตถุที่เลือกจาก ToolManager

            if (targetObject != null)
            {
                if (Input.GetMouseButtonDown(0)) // คลิกซ้าย -> ขยาย
                {
                    ModifyObjectScale(targetObject, zoomAmount);
                }
                else if (Input.GetMouseButtonDown(1)) // คลิกขวา -> ย่อ
                {
                    ModifyObjectScale(targetObject, -zoomAmount);
                }
            }
            else
            {
                Debug.LogWarning("No object selected! Use Hand mode to select an object first.");
            }
        }
    }

    void ModifyObjectScale(GameObject target, float scaleChange)
    {
        BreakableObject breakable = target.GetComponent<BreakableObject>();
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
