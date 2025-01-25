using UnityEngine;
using UnityEngine.UI;

public class ToolbarManager : MonoBehaviour
{
    public RectTransform toolbar; // RectTransform ของ Toolbar
    public float visibleXPosition = 0f; // ตำแหน่ง X เมื่อ Toolbar แสดง
    public float hiddenXPosition = -300f; // ตำแหน่ง X เมื่อ Toolbar ซ่อน
    public float moveSpeed = 5f; // ความเร็วในการเลื่อน
    public Button toggleButton; // ปุ่มสำหรับเปิด/ปิด Toolbar
    public Image arrowImage; // Image ของลูกศร
    public Sprite arrowOpenSprite; // Sprite สำหรับแสดง Toolbar
    public Sprite arrowCloseSprite; // Sprite สำหรับซ่อน Toolbar

    private bool isVisible = true; // เริ่มต้นเป็น "แสดง"

    void Start()
    {
        // ผูกฟังก์ชัน ToggleToolbar กับปุ่ม
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleToolbar);
        }
    }

    void ToggleToolbar()
    {
        isVisible = !isVisible; // เปลี่ยนสถานะ
        float targetX = isVisible ? visibleXPosition : hiddenXPosition;

        // เปลี่ยน Sprite ลูกศร
        UpdateArrowSprite();

        // เลื่อน Toolbar ไปยังตำแหน่งเป้าหมาย
        StopAllCoroutines();
        StartCoroutine(MoveToolbar(targetX));
    }

    void UpdateArrowSprite()
    {
        if (arrowImage != null)
        {
            arrowImage.sprite = isVisible ? arrowCloseSprite : arrowOpenSprite;
        }
    }

    void AutoHideToolbar()
    {
        // เริ่มต้นซ่อน Toolbar หลังจากแสดงตอนเริ่มเกม
        isVisible = false;
        float targetX = hiddenXPosition;

        // เปลี่ยน Sprite ลูกศร
        UpdateArrowSprite();

        // เลื่อน Toolbar ไปยังตำแหน่งซ่อน
        StopAllCoroutines();
        StartCoroutine(MoveToolbar(targetX));
    }

    System.Collections.IEnumerator MoveToolbar(float targetX)
    {
        while (Mathf.Abs(toolbar.anchoredPosition.x - targetX) > 0.01f)
        {
            float newX = Mathf.Lerp(toolbar.anchoredPosition.x, targetX, Time.deltaTime * moveSpeed);
            toolbar.anchoredPosition = new Vector2(newX, toolbar.anchoredPosition.y);
            yield return null;
        }

        toolbar.anchoredPosition = new Vector2(targetX, toolbar.anchoredPosition.y);
    }
}
