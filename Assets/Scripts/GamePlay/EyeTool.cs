using UnityEngine;

public class EyeTool : MonoBehaviour
{
    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Eye")
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                InteractWithObject();
            }
        }
    }

    private void InteractWithObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            DialogueSystem dialogueSystem = hit.collider.GetComponent<DialogueSystem>();
            if (dialogueSystem != null)
            {
                dialogueSystem.ShowDialogue();
            }
        }
    }
}
