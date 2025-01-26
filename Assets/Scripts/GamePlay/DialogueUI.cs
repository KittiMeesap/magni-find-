using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    private GameObject currentItemObject; // GameObject �Ѩ�غѹ����ʴ������

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

        // �Ѵ��á���ʴ�/��͹�ͧ�����
        if (currentItemObject != null)
        {
            currentItemObject.SetActive(false); // ��͹�������͹˹��
        }

        if (itemObject != null)
        {
            itemObject.SetActive(true); // �ʴ����������
            currentItemObject = itemObject; // ������������ѧ�ʴ�
        }
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);

        // ��͹������Ѩ�غѹ
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
