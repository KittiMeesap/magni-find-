using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button closeButton;
    private GameObject currentItemObject; 

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

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideDialogue);
        }
        else
        {
            Debug.LogWarning("Close button not assigned in the inspector.");
        }
    }

    private void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            HideDialogue();
        }
    }

    public void ShowDialogue(string message, GameObject itemObject = null)
    {
        dialogueText.text = message;
        dialoguePanel.SetActive(true);

        if (currentItemObject != null)
        {
            currentItemObject.SetActive(false);
        }

        if (itemObject != null)
        {
            itemObject.SetActive(true);
            currentItemObject = itemObject;
        }
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);

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
