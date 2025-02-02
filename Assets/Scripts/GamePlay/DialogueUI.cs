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

    // ??????????? Image ???????????????
    public Image itemImage; // Image UI ????????????????

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

    public void ShowDialogue(string message, GameObject itemObject = null)
    {
        dialogueText.text = message;
        dialoguePanel.SetActive(true);

        // ???????????????????????????
        if (currentItemObject != null)
        {
            currentItemObject.SetActive(false);
        }

        // ?????????? itemObject ???????????
        if (itemObject != null)
        {
            itemObject.SetActive(true); // ???????????????? itemObject

            currentItemObject = itemObject;

            // ?????????? itemObject ?? SpriteRenderer ???? Image ???????
            SpriteRenderer spriteRenderer = itemObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // ???????????? Image UI ??????????????????
                itemImage.sprite = spriteRenderer.sprite;
                itemImage.gameObject.SetActive(true); // ????????????
            }
            else
            {
                // ???????? SpriteRenderer ??????? Image
                Image image = itemObject.GetComponent<Image>();
                if (image != null)
                {
                    itemImage.sprite = image.sprite;
                    itemImage.gameObject.SetActive(true);
                }
                else
                {
                    itemImage.gameObject.SetActive(false); // ???????????? SpriteRenderer ???? Image
                }
            }
        }
        else
        {
            itemImage.gameObject.SetActive(false); // ??????????????? itemObject
        }
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        Camera.main.GetComponent<CameraController>().ExitMinigame();
        if (currentItemObject != null)
        {
            currentItemObject.SetActive(false);
            currentItemObject = null;
        }

        itemImage.gameObject.SetActive(false);
    }
}
