using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public int itemIndex;
    private ProgressManager progressManager;

    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
    }

    public void Initialize(ProgressManager manager)
    {
        progressManager = manager;
    }

    void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;

        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            // ✅ **เช็คว่า sprite มี alpha เป็น 255 (1f) หรือไม่**
            if (spriteRenderer != null && spriteRenderer.color.a < 1f)
            {
                Debug.Log("Item is not fully visible yet!");
                return; // ❌ **ออกจากฟังก์ชัน ไม่ให้กดเก็บไอเท็ม**
            }

            if (progressManager != null)
            {
                progressManager.MarkAsFound(itemIndex);
                Debug.Log($"Item {name} collected!");
                Destroy(gameObject);
                MinigameManager.Instance.OnRewardCollected();
            }
        }
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;

        if (MinigameManager.Instance.IsPlayingMinigame && (ToolManager.Instance.CurrentMode == "Hand"))
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite;
            }
        }
        else
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }

    private void OnMouseExit()
    {

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }    
    }
}
