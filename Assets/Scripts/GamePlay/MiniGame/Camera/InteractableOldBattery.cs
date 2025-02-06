using UnityEngine;

public class InteractableOldBattery : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 initialPosition;
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
    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float newY = Mathf.Min(mousePosition.y, initialPosition.y); // ให้ลากลงได้อย่างเดียว
            transform.position = new Vector3(initialPosition.x, newY, transform.position.z);
        }
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            isDragging = true;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (Vector3.Distance(transform.position, initialPosition) >= 0.5f)
        {
            CameraMinigame.Instance.RemoveOldBattery();
        }

        // ✅ เมื่อปล่อยเมาส์ให้เปลี่ยนกลับเป็น defaultSprite ทันที
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    private void OnMouseOver()
    {
        // ✅ เพิ่มเงื่อนไข: ถ้าไม่ได้ลากอยู่ ค่อยเปลี่ยนเป็น highlightedSprite
        if (!isDragging && MinigameManager.Instance.IsPlayingMinigame && (ToolManager.Instance.CurrentMode == "Hand") && CameraMinigame.Instance.OldBatteryRemoved == false) 
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite;
            }
        }
    }

    private void OnMouseExit()
    {
        // ✅ ถ้าไม่ได้ลากอยู่ ให้เปลี่ยนกลับเป็น defaultSprite
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }


}
