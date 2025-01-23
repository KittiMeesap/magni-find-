using UnityEngine;

public class MagnifyingGlassController : MonoBehaviour
{
    public float zoomAmount = 0.1f;

    void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier")
        {
            GameObject targetObject = ToolManager.Instance.GetSelectedObject();

            if (targetObject != null)
            {
                DialogueSystem dialogueSystem = targetObject.GetComponent<DialogueSystem>();

                if (Input.GetMouseButtonDown(0))
                {
                    if (dialogueSystem != null && dialogueSystem.hasMagnifierMessage)
                    {
                        dialogueSystem.ShowDialogue(); 
                    }
                    else
                    {
                        ModifyObjectScale(targetObject, zoomAmount);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (dialogueSystem != null && dialogueSystem.hasMagnifierMessage)
                    {
                        dialogueSystem.ShowDialogue();
                    }
                    else
                    {
                        ModifyObjectScale(targetObject, -zoomAmount);
                    }
                }
            }
            else
            {
                Debug.LogWarning("No object selected! Use Hand mode to select an object first.");
            }
        }
    }

    void ModifyObjectScale(GameObject target, float scaleChange)
    {
        BreakableObject breakable = target.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            breakable.ModifyScale(scaleChange);
        }
        else
        {
            Debug.LogWarning("Selected object is not breakable.");
        }
    }
}
