using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button closeButton;
    public Image itemImage;

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

            SpriteRenderer spriteRenderer = itemObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                itemImage.sprite = spriteRenderer.sprite;
                itemImage.gameObject.SetActive(true);
            }
            else
            {
                itemImage.gameObject.SetActive(false);
            }
        }
        else
        {
            itemImage.gameObject.SetActive(false);
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

        itemImage.gameObject.SetActive(false);
    }
}
