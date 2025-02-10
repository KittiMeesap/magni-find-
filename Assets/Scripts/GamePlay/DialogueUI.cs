using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button closeButton;
    public Button qButton;
    public Image CharImage;
    private GameObject currentItemObject;
    public bool IsOpenDialog = false;

    public Image itemImage;

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

        if (itemImage == null)
        {
            Debug.LogError("Item Image not assigned in the inspector.");
        }
    }

    private void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (!closeButton.gameObject.activeSelf) return;
            HideDialogue();
        }
    }

    public void ShowDialogue(string message, GameObject itemObject = null)
    {
        qButton.gameObject.SetActive(false);
        IsOpenDialog = true;
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
                Image image = itemObject.GetComponent<Image>();
                if (image != null)
                {
                    itemImage.sprite = image.sprite;
                    itemImage.gameObject.SetActive(true);
                }
                else
                {
                    itemImage.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            itemImage.gameObject.SetActive(false);
        }
    }

    public void HideDialogue()
    {
        qButton.gameObject.SetActive(true);
        IsOpenDialog = false;
        dialoguePanel.SetActive(false);

        // พักมินิเกมและออกจากมินิเกมเมื่อปิดไดอะล็อก
        if (MinigameManager.Instance != null && MinigameManager.Instance.IsPlayingMinigame)
        {
            MinigameManager.Instance.PauseMinigame();
        }

        if (currentItemObject != null)
        {
            currentItemObject.SetActive(false);
            currentItemObject = null;
        }

        itemImage.gameObject.SetActive(false);
        Camera.main.GetComponent<CameraController>().ExitMinigame();
    }

    public void DialogueButton(bool status)
    {
        closeButton.gameObject.SetActive(status);
        //closeButton.interactable = status;
    }

    public void DialogueUpdate(string message)
    {
        dialogueText.text = message;
    }
}

