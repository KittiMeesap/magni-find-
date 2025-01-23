using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [TextArea]
    public string dialogueText;
    public bool hasMagnifierMessage; 
    public string dialogueTextSpecial; 

    public void ShowDialogue()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier" && hasMagnifierMessage)
        {
            if (DialogueUI.Instance != null)
            {
                DialogueUI.Instance.ShowDialogue(dialogueTextSpecial);
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
                DialogueUI.Instance.ShowDialogue(dialogueText);
            }
            else
            {
                Debug.LogError("DialogueUI instance not found in the scene.");
            }
        }
    }
}
