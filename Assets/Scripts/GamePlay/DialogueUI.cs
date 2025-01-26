using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    private GameObject currentItemObject; // GameObject ปัจจุบันที่แสดงไอเท็ม

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string message, GameObject itemObject = null)
    {
        dialogueText.text = message;
        dialoguePanel.SetActive(true);

        // จัดการการแสดง/ซ่อนของไอเท็ม
        if (currentItemObject != null)
        {
            currentItemObject.SetActive(false); // ซ่อนไอเท็มก่อนหน้า
        }

        if (itemObject != null)
        {
            itemObject.SetActive(true); // แสดงไอเท็มใหม่
            currentItemObject = itemObject; // เก็บไอเท็มที่กำลังแสดง
        }
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);

        // ซ่อนไอเท็มปัจจุบัน
        if (currentItemObject != null)
        {
            currentItemObject.SetActive(false);
            currentItemObject = null;
        }
    }

    public void ToggleDialoguePanel()
    {
        if (dialoguePanel.activeSelf)
        {
            HideDialogue();
        }
        else
        {
            ShowDialogue(dialogueText.text);
        }
    }
}
