using UnityEngine;
using UnityEngine.UI;

public class MoveTitle : MonoBehaviour
{
    public ToolManager toolManager; // ตัวจัดการเครื่องมือ
    public GameObject gameTitle; // ชื่อเกมที่จะแสดงในเมนูหลัก
    public float targetYPosition = 250f; // ตำแหน่ง Y ที่ชื่อเกมจะขยับไป
    public float speed = 50f; // ความเร็วในการขยับชื่อเกม
    public GameObject playMinigameObject; // ปุ่มเล่นมินิเกม

    private bool isToolChanged = false;
    private bool isTitleMoving = false; // ใช้เพื่อบอกว่าชื่อเกมเริ่มขยับหรือยัง

    private RectTransform gameTitleRectTransform;

    void Start()
    {
        // ตั้งค่าให้เริ่มต้นในโหมด "Eye"
        toolManager.SetToolMode("Eye");

        // ดึง RectTransform ของ gameTitle เพื่อใช้ในการขยับ
        gameTitleRectTransform = gameTitle.GetComponent<RectTransform>();

        // ตั้งชื่อเกมให้อยู่ที่ตำแหน่ง Y = 0 โดยใช้ anchoredPosition
        gameTitleRectTransform.anchoredPosition = new Vector2(gameTitleRectTransform.anchoredPosition.x, 0f);
    }

    void Update()
    {
        // เช็คว่าเมาส์เลื่อนขึ้นหรือลงเพื่อสลับเครื่องมือ
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            ToggleToolMode();
        }

        // เริ่มขยับชื่อเกมเมื่อคลิกที่ชื่อเกม
        if (Input.GetMouseButtonDown(0) && !isTitleMoving)
        {
            isTitleMoving = true;
        }

        // ถ้าชื่อเกมเริ่มขยับให้ทำการขยับ
        if (isTitleMoving && !isToolChanged)
        {
            Move();
        }
    }

    public void Move()
    {
        // ตรวจสอบว่าชื่อเกมยังไม่ถึงตำแหน่งที่กำหนด
        if (gameTitleRectTransform.anchoredPosition.y < targetYPosition)
        {
            // ขยับชื่อเกมไปยังตำแหน่งที่กำหนด
            float newY = Mathf.MoveTowards(gameTitleRectTransform.anchoredPosition.y, targetYPosition, speed * Time.deltaTime);
            gameTitleRectTransform.anchoredPosition = new Vector2(gameTitleRectTransform.anchoredPosition.x, newY);
        }
        else
        {
            // เมื่อชื่อเกมถึงตำแหน่งที่กำหนดแล้วให้เปลี่ยนเครื่องมือเป็น "Hand"
            if (!isToolChanged)
            {
                isToolChanged = true;
                toolManager.SetToolMode("Hand");

                // แสดงปุ่มเล่นมินิเกม
                playMinigameObject.SetActive(true);
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
