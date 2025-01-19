using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public int itemIndex; // ¥—™π’¢Õß‰Õ‡∑Á¡„π Quest UI
    private ProgressManager progressManager;

    public void Initialize(ProgressManager manager)
    {
        progressManager = manager;
    }

    void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            if (progressManager != null)
            {
                progressManager.MarkAsFound(itemIndex);
                Debug.Log($"Item {name} collected!");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("ProgressManager not assigned!");
            }
        }
        else
        {
            Debug.LogWarning("You must be in Hand mode to collect items!");
        }
    }
}
