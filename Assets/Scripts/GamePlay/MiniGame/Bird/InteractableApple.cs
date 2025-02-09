using UnityEngine;
using System.Collections;

public class InteractableApple : MonoBehaviour
{
    private bool isDragging = false;
    private bool isPlacedInTray = false;
    private bool isScaling = false; // ✅ ใช้ตรวจสอบว่าสามารถย่อขยายได้หรือไม่
    private bool hasFallen = false;
    private Vector3 initialPosition;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;
    [SerializeField] private float scaleStep = 0.1f;
    [SerializeField] private Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 maxScale = new Vector3(2f, 2f, 2f);
    private bool isShakingTray = false;
    private int originalSortingOrder;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    private void Update()
    {
        if (isScaling && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            // ✅ คลิกซ้าย -> ขยาย
            if (Input.GetMouseButtonDown(0))
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Upsize);
                ModifyScale(scaleStep);
            }
            // ✅ คลิกขวา -> ลดขนาด
            else if (Input.GetMouseButtonDown(1))
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Smallsize);
                ModifyScale(-scaleStep);
            }
        }

        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;

        // ✅ เริ่มการลาก ถ้าอยู่ในโหมด Hand และยังไม่ถูกวาง
        if (ToolManager.Instance.CurrentMode == "Hand" && BirdMinigame.Instance.CanPickUpApple && !isPlacedInTray)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Hand);
            isDragging = true;
            spriteRenderer.sortingOrder = originalSortingOrder + 3;
        }
    }

    private void OnMouseUp()
    {
        spriteRenderer.sortingOrder = originalSortingOrder;
        isDragging = false;
        isScaling = false;

        if (BirdMinigame.Instance.CanPickUpApple && hasFallen)
        {
            float distance = Vector3.Distance(transform.position, BirdMinigame.Instance.AppleTrayPosition.position);
            if (distance < 3.5) // ✅ ถ้าอยู่ในระยะ Tray ให้วาง
            {
                transform.position = BirdMinigame.Instance.AppleTrayPosition.position;
                isPlacedInTray = true;
                BirdMinigame.Instance.PlaceAppleInTray();
            }
            else
            {
                transform.position = BirdMinigame.Instance.AppleFallPosition.position; // ✅ ถ้าไม่อยู่ในระยะ ให้กลับตำแหน่งเดิม
            }
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
        if (Time.timeScale == 0f) return;
        if (spriteRenderer == null) return;
        

        // ✅ ถ้าแอปเปิ้ลถูกวางไปแล้ว ห้ามไฮไลต์
        if (isPlacedInTray)
        {
            spriteRenderer.sprite = defaultSprite;
            return;
        }

        if (!BirdMinigame.Instance.IsAppleCorrectSize && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            spriteRenderer.sprite = highlightedSprite; // ✅ ถ้ายังไม่ถูก และใช้ Magnifier
            isScaling = true; // ✅ อนุญาตให้ย่อขยายเมื่อเมาส์อยู่บนแอปเปิ้ล
        }
        else if (BirdMinigame.Instance.IsAppleCorrectSize && ToolManager.Instance.CurrentMode == "Hand")
        {
            if (!hasFallen) return;
            spriteRenderer.sprite = highlightedSprite; // ✅ ถ้าถูกแล้ว และใช้ Hand
            if (!isShakingTray) StartCoroutine(BirdMinigame.Instance.ShakeTray());
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
            isScaling = false; // ✅ ออกจากโหมดย่อขยายถ้าเมาส์ออก
        }
    }

    private void OnMouseExit()
    {

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
        isScaling = false; // ✅ หยุดย่อขยายถ้าเมาส์ออก
        BirdMinigame.Instance.ResetTray();
    }
    public void MarkAsFallen()
    {
        hasFallen = true; // ✅ ฟังก์ชันนี้เรียกจาก `BirdMinigame` เมื่อ Apple ตกถึงพื้น
    }
}
