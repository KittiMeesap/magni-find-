using UnityEngine;
using System.Collections;

public class InteractableApple : MonoBehaviour
{
    private bool isDragging = false;
    private bool isPlacedInTray = false;
    private Vector3 initialPosition;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;
    [SerializeField] private float scaleStep = 0.1f;
    [SerializeField] private Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 maxScale = new Vector3(2f, 2f, 2f);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }
        else if (ToolManager.Instance.CurrentMode == "Magnifier")
        {
            HandleScaling();
        }
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && BirdMinigame.Instance.CanPickUpApple && !isPlacedInTray)
        {
            isDragging = true;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (BirdMinigame.Instance.CanPickUpApple)
        {
            BirdMinigame.Instance.PlaceAppleInTray();
            isPlacedInTray = true;
            gameObject.SetActive(false); // ✅ ปิดแอปเปิ้ลหลังจากวางสำเร็จ
        }
    }

    private void HandleScaling()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ModifyScale(scaleStep);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ModifyScale(-scaleStep);
        }
    }

    private void ModifyScale(float scaleChange)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;
        newScale.x = Mathf.Clamp(newScale.x, minScale.x, maxScale.x);
        newScale.y = Mathf.Clamp(newScale.y, minScale.y, maxScale.y);
        newScale.z = Mathf.Clamp(newScale.z, minScale.z, maxScale.z);
        transform.localScale = newScale;

        BirdMinigame.Instance.CheckAppleSize();
    }

    private void OnMouseOver()
    {
        if (spriteRenderer != null && highlightedSprite != null)
        {
            spriteRenderer.sprite = highlightedSprite;
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
