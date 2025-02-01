using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public int itemIndex;
    private ProgressManager progressManager;

    public void Initialize(ProgressManager manager)
    {
        progressManager = manager;
    }

    void OnMouseDown()
    {
        MinigameManager.Instance.OnRewardCollected();

        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            if (progressManager != null)
            {
                progressManager.MarkAsFound(itemIndex);
                Debug.Log($"Item {name} collected!");
                Destroy(gameObject);
            }
        }
    }
}
