using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [TextArea]
    public string dialogueText;
    public bool hasMagnifierMessage;
    public string dialogueTextSpecial;

    public GameObject itemObject; // ?????? GameObject ????? SpriteRenderer ???? Image

    public void ShowDialogue()
    {
        // ????????????? Magnifier ?????????????????
        if (ToolManager.Instance.CurrentMode == "Magnifier" && hasMagnifierMessage)
        {
            if (DialogueUI.Instance != null)
            {
                // ??????????????????????????? itemObject
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
