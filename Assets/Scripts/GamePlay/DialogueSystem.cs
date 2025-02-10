using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [TextArea]
    public string dialogueText;
    public bool hasMagnifierMessage;
    public string dialogueTextSpecial;
    public GameObject itemObject;

    [Header("SFX")]
    [SerializeField] private AudioClip sfx_OpenDialogue;



    


    public void ShowDialogue()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier" && hasMagnifierMessage)
        {
            if (DialogueUI.Instance != null)
            {
                SoundManager.Instance.PlaySFX(sfx_OpenDialogue);
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
                SoundManager.Instance.PlaySFX(sfx_OpenDialogue);
                DialogueUI.Instance.ShowDialogue(dialogueText, itemObject);
            }
            else
            {
                Debug.LogError("DialogueUI instance not found in the scene.");
            }
        }

        Camera.main.GetComponent<CameraController>().EnterMinigame();
    }
}
