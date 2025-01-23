using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [TextArea]
    public string dialogueText;

    public void ShowDialogue()
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
