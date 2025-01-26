using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [TextArea]
    public string dialogueText; // ข้อความทั่วไป
    public bool hasMagnifierMessage; // ระบุว่ามีข้อความเฉพาะของ Magnifier หรือไม่
    public string dialogueTextSpecial; // ข้อความเฉพาะของ Magnifier

    public GameObject itemObject; // GameObject ที่จะใช้แสดงไอเท็ม


    public void ShowDialogue()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier" && hasMagnifierMessage)
        {
            if (DialogueUI.Instance != null)
            {
                DialogueUI.Instance.ShowDialogue(dialogueTextSpecial, itemObject);
            }
            else
            {
                Debug.LogError("DialogueUI instance not found in the scene.");
            }
        }
        else
        {
            if (DialogueUI.Instance != null)
            {
                DialogueUI.Instance.ShowDialogue(dialogueText, itemObject);
            }
            else
            {
                Debug.LogError("DialogueUI instance not found in the scene.");
            }
        }


    }
}
