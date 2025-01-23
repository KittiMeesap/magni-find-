using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

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

    public void ShowDialogue(string message)
    {
        dialogueText.text = message; 
        dialoguePanel.SetActive(true); 
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false); 
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
