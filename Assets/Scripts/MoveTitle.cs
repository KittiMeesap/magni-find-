using UnityEngine;
using UnityEngine.UI;

public class MoveText : MonoBehaviour
{
    public RectTransform textRectTransform;  // ใช้ RectTransform สำหรับ UI
    public float targetYPosition = 200f;  // เป้าหมายที่ตำแหน่ง Y ที่จะขยับไป
    public float speed = 5f;  // ความเร็วในการเคลื่อนที่
    private bool isMoving = false;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(StartMoving);
            Debug.Log("Button Listener added");
        }
        else
        {
            Debug.LogError("Button component not found!");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveUp();
        }
    }

    public void StartMoving()
    {
        if (!isMoving)
        {
            Debug.Log("Start Moving");
            isMoving = true;
        }
    }

    void MoveUp()
    {
        // ตรวจสอบตำแหน่ง Y และขยับให้ถึงตำแหน่ง targetYPosition
        if (textRectTransform.position.y < targetYPosition)
        {
            Debug.Log("Moving up");
            // ขยับตำแหน่ง Y ของ Text UI
            float newY = Mathf.MoveTowards(textRectTransform.position.y, targetYPosition, speed * Time.deltaTime);
            textRectTransform.position = new Vector3(textRectTransform.position.x, newY, textRectTransform.position.z);
        }
        else
        {
            // หยุดเมื่อถึงตำแหน่งที่กำหนด
            Debug.Log("Movement completed");
            isMoving = false;
        }
    }
}
