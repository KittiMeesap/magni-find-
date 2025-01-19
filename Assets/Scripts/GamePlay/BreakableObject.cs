using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public enum ActionType { Expand, Shrink } // การกระทำที่ต้องการ
    public ActionType requiredAction; // เงื่อนไข: ขยายหรือย่อ
    public float maxScale = 3f; // ขนาดสูงสุด
    public float minScale = 0.5f; // ขนาดต่ำสุด
    public GameObject hiddenItem; // ไอเท็มที่ซ่อนอยู่ในวัตถุ

    private bool isActionComplete = false; // ตรวจสอบว่าเงื่อนไขสำเร็จแล้วหรือไม่

    public void ModifyScale(float scaleChange)
    {
        // ตรวจสอบว่ามีวัตถุถูกเลือกอยู่หรือไม่
        if (ToolManager.Instance.GetSelectedObject() != gameObject)
        {
            Debug.LogWarning($"{gameObject.name} is not the currently selected object!");
            return; // หยุดทำงานหากวัตถุนี้ไม่ได้ถูกเลือก
        }

        if (isActionComplete) return; // หากเงื่อนไขสำเร็จแล้ว หยุดการทำงาน

        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

        // จำกัดขนาดไม่ให้เกินขอบเขต
        if (newScale.x > maxScale) newScale = Vector3.one * maxScale;
        if (newScale.x < minScale) newScale = Vector3.one * minScale;

        transform.localScale = newScale;

        // กรณีที่ต้องการให้ "ขยาย"
        if (requiredAction == ActionType.Expand)
        {
            if (scaleChange > 0 && newScale.x >= maxScale) // ขยายจนถึง maxScale
            {
                RevealItem();
            }
            else if (scaleChange < 0 && newScale.x <= minScale) // ย่อจนถึง minScale
            {
                HandleWrongAction();
            }
        }
        // กรณีที่ต้องการให้ "ย่อ"
        else if (requiredAction == ActionType.Shrink)
        {
            if (scaleChange < 0 && newScale.x <= minScale) // ย่อจนถึง minScale
            {
                RevealItem();
            }
            else if (scaleChange > 0 && newScale.x >= maxScale) // ขยายจนถึง maxScale
            {
                HandleWrongAction();
            }
        }
    }

    private void RevealItem()
    {
        isActionComplete = true;
        Debug.Log($"Correct action! {gameObject.name} revealed an item.");
        if (hiddenItem != null)
        {
            hiddenItem.SetActive(true); // แสดงไอเท็ม
        }
        Destroy(gameObject); // ทำลายวัตถุ
    }

    private void HandleWrongAction()
    {
        isActionComplete = true;
        Debug.Log($"Wrong action! {gameObject.name} destroyed and item lost.");
        if (hiddenItem != null)
        {
            Destroy(hiddenItem); // ทำลายไอเท็ม
        }
        Destroy(gameObject); // ทำลายวัตถุ
    }
}
