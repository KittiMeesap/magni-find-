using UnityEngine;

public class ClockMinigame : MonoBehaviour
{
    [Header("Clock Components")]
    public Transform hourHand; // เข็มชั่วโมง
    public Transform minuteHand; // เข็มนาที
    public Vector3 correctHourHandRotation; // มุมที่ถูกต้องของเข็มชั่วโมง
    public Vector3 correctMinuteHandRotation; // มุมที่ถูกต้องของเข็มนาที
    public float rotationStep = 10f; // ระดับการหมุนในแต่ละครั้ง
    public float rotationTolerance = 5f; // ความคลาดเคลื่อนของมุมที่ยอมรับได้

    [Header("Clock Numbers")]
    public Vector3 correctScale; // ขนาดที่ถูกต้องของตัวเลข
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // ขนาดต่ำสุด
    public Vector3 maxScale = new Vector3(3f, 3f, 3f); // ขนาดสูงสุด
    public float scaleStep = 0.1f; // ขั้นตอนการปรับขนาด
    public float scaleTolerance = 0.1f; // ความคลาดเคลื่อนของขนาดที่ยอมรับได้

    private bool isDraggingHourHand = false;
    private bool isDraggingMinuteHand = false;

    void Update()
    {
        // Handle Hand Mode
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            if (isDraggingHourHand)
            {
                RotateHand(hourHand);
            }
            else if (isDraggingMinuteHand)
            {
                RotateHand(minuteHand);
            }
        }

        // Handle Magnifier Mode
        if (ToolManager.Instance.CurrentMode == "Magnifier" && ToolManager.Instance.GetSelectedObject() == gameObject)
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

        // Check Completion
        if (IsClockCorrect() && IsScaleCorrect())
        {
            Debug.Log("Clock Minigame Completed!");
            //MinigameManager.Instance.CompleteMinigame(); // แจ้งว่ามินิเกมสำเร็จแล้ว
        }
    }

    private void RotateHand(Transform hand)
    {
        Vector3 rotation = hand.eulerAngles;
        rotation.z += Input.GetAxis("Mouse X") * rotationStep * Time.deltaTime;
        hand.eulerAngles = rotation;
    }

    private void ModifyScale(float scaleChange)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

        // Clamp scale within min and max
        newScale.x = Mathf.Clamp(newScale.x, minScale.x, maxScale.x);
        newScale.y = Mathf.Clamp(newScale.y, minScale.y, maxScale.y);
        newScale.z = Mathf.Clamp(newScale.z, minScale.z, maxScale.z);

        transform.localScale = newScale;

        Debug.Log($"Modified scale of {gameObject.name} to {newScale}");
    }

    private bool IsClockCorrect()
    {
        // ตรวจสอบว่าเข็มนาฬิกาอยู่ในมุมที่ถูกต้อง
        return Mathf.Abs(Vector3.Distance(hourHand.eulerAngles, correctHourHandRotation)) <= rotationTolerance &&
               Mathf.Abs(Vector3.Distance(minuteHand.eulerAngles, correctMinuteHandRotation)) <= rotationTolerance;
    }

    private bool IsScaleCorrect()
    {
        // ตรวจสอบว่าขนาดตัวเลขอยู่ในช่วงที่กำหนด
        return Mathf.Abs(transform.localScale.x - correctScale.x) <= scaleTolerance &&
               Mathf.Abs(transform.localScale.y - correctScale.y) <= scaleTolerance &&
               Mathf.Abs(transform.localScale.z - correctScale.z) <= scaleTolerance;
    }

    private void OnMouseDown()
    {
        // เริ่มหมุนเข็มใน Hand Mode
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float hourDistance = Vector2.Distance(mousePos, hourHand.position);
            float minuteDistance = Vector2.Distance(mousePos, minuteHand.position);

            if (hourDistance < minuteDistance)
            {
                isDraggingHourHand = true;
            }
            else
            {
                isDraggingMinuteHand = true;
            }
        }
    }

    private void OnMouseUp()
    {
        // หยุดหมุนเข็ม
        isDraggingHourHand = false;
        isDraggingMinuteHand = false;
    }
}
