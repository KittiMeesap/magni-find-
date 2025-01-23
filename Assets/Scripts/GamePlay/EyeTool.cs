using UnityEditor.VersionControl;
using UnityEngine;

public class EyeTool : MonoBehaviour
{
    public Texture2D eyeCursor;

    private void Awake()
    {
        SetEyeCursor();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InteractWithObject();
        }
    }

    private void SetEyeCursor()
    {
        Cursor.SetCursor(eyeCursor, Vector2.zero, CursorMode.Auto);
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
                if (ToolManager.Instance.CurrentMode == "Eye")
                {
                    dialogueSystem.ShowDialogue();
                }
                else
                {
                    Debug.LogWarning("Use the Eye tool to interact with this item.");
                }
            }
            else
            {
                Debug.LogWarning("No DialogueSystem attached to the object.");
            }
        }
    }
}
