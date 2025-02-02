using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public TMP_Text dialogueText;           // สำหรับคำใบ้ธรรมดา

    public bool hasMagnifierMessage;        // ตรวจสอบว่ามีคำใบ้พิเศษหรือไม่
    public string dialogueTextSpecialMessage; // ข้อความของคำใบ้พิเศษ
    public string dialogueTextMessage;      // ข้อความของคำใบ้ธรรมดา

    public void ShowDialogue()
    {
        // เช็คว่าอยู่ในโหมด Magnifier และมีข้อความพิเศษ
        if (ToolManager.Instance.CurrentMode == "Magnifier" && hasMagnifierMessage)
        {
            // ถ้ามีคำใบ้พิเศษ ให้แสดงข้อความพิเศษ
            if (dialogueText != null)
            {
                dialogueText.text = dialogueTextSpecialMessage;
            }

            // ให้ข้อความพิเศษขึ้นใน dialogueText เท่านั้น
            dialogueText.text = dialogueTextSpecialMessage;
        }
        else
        {
            // แสดงข้อความธรรมดา
            if (dialogueText != null)
            {
                dialogueText.text = dialogueTextMessage;
            }
        }

        // แสดงหน้าต่างไดอะล็อค
        DialogueManager.Instance.ShowDialogue(dialogueText.text, gameObject);
    }
}
