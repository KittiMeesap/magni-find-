using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public ToolManager toolManager; // ตัวจัดการเครื่องมือ
    public GameObject gameTitle; // ชื่อเกมที่จะแสดงในเมนูหลัก
    public float targetYPosition = 850f; // ตำแหน่ง Y ที่ชื่อเกมจะขยับไป
    public float speed = 100f; // ความเร็วในการขยับชื่อเกม

    private bool isToolChanged = false;

    private RectTransform gameTitleRectTransform;

    void Start()
    {
        // ตั้งค่าให้เริ่มต้นในโหมด "Eye" และให้ชื่อเกมเริ่มจากตำแหน่งเริ่มต้น
        toolManager.SetToolMode("Eye");

        // ดึง RectTransform ของ gameTitle เพื่อใช้ในการขยับ
        gameTitleRectTransform = gameTitle.GetComponent<RectTransform>();

        // เริ่มที่ตำแหน่ง Y = 0
        gameTitleRectTransform.position = new Vector3(gameTitleRectTransform.position.x, 0f, gameTitleRectTransform.position.z);
    }

    void Update()
    {
        // เช็คว่าเมาส์เลื่อนขึ้นหรือลงเพื่อสลับเครื่องมือ
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            ToggleToolMode();
        }

        // การขยับชื่อเกม
        if (!isToolChanged)
        {
            MoveTitle();
        }
    }

    private void MoveTitle()
    {
        // ตรวจสอบว่าชื่อเกมยังไม่ถึงตำแหน่งที่กำหนด
        if (gameTitleRectTransform.position.y < targetYPosition)
        {
            // ขยับชื่อเกมไปยังตำแหน่งที่กำหนด
            float newY = Mathf.MoveTowards(gameTitleRectTransform.position.y, targetYPosition, speed * Time.deltaTime);
            gameTitleRectTransform.position = new Vector3(gameTitleRectTransform.position.x, newY, gameTitleRectTransform.position.z);
        }
        else
        {
            // เมื่อชื่อเกมถึงตำแหน่งที่กำหนดแล้วให้เปลี่ยนเครื่องมือเป็น "Hand"
            if (!isToolChanged)
            {
                isToolChanged = true;
                toolManager.SetToolMode("Hand");
            }
        }
    }

    private void ToggleToolMode()
    {
        // ถ้าชื่อเกมขยับเสร็จแล้ว ให้สามารถใช้ลูกกลิ้งเมาส์เพื่อสลับระหว่าง "Magnifier" และ "Hand"
        if (isToolChanged)
        {
            if (toolManager.CurrentMode == "Hand")
            {
                toolManager.SetToolMode("Magnifier");
            }
            else if (toolManager.CurrentMode == "Magnifier")
            {
                toolManager.SetToolMode("Hand");
            }
        }
    }
}
