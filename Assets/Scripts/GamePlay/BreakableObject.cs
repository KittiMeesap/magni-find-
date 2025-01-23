using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public enum ActionType { Expand, Shrink } 
    public ActionType requiredAction;
    public float maxScale = 3f; 
    public float minScale = 0.5f; 
    public GameObject hiddenItem; 

    private bool isActionComplete = false;

    public void ModifyScale(float scaleChange)
    {
        if (ToolManager.Instance.GetSelectedObject() != gameObject)
        {
            Debug.LogWarning($"{gameObject.name} is not the currently selected object!");
            return; 
        }

        if (isActionComplete) return;

        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

        if (newScale.x > maxScale) newScale = Vector3.one * maxScale;
        if (newScale.x < minScale) newScale = Vector3.one * minScale;

        transform.localScale = newScale;

        if (requiredAction == ActionType.Expand)
        {
            if (scaleChange > 0 && newScale.x >= maxScale) 
            {
                RevealItem();
            }
            else if (scaleChange < 0 && newScale.x <= minScale) 
            {
                HandleWrongAction();
            }
        }
        else if (requiredAction == ActionType.Shrink)
        {
            if (scaleChange < 0 && newScale.x <= minScale) 
            {
                RevealItem();
            }
            else if (scaleChange > 0 && newScale.x >= maxScale) 
            {
                HandleWrongAction();
            }
        }
    }

    private void RevealItem()
    {
        isActionComplete = true;
        Debug.Log($"Correct action! {gameObject.name} revealed an item.");
        if (hiddenItem != null)
        {
            hiddenItem.SetActive(true);
        }
        Destroy(gameObject);
    }

    private void HandleWrongAction()
    {
        isActionComplete = true;
        Debug.Log($"Wrong action! {gameObject.name} destroyed and item lost.");
        if (hiddenItem != null)
        {
            Destroy(hiddenItem);
        }
        Destroy(gameObject); 
    }
}
