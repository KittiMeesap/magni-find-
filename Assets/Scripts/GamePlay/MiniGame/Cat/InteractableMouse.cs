using UnityEngine;
using System.Collections;

public class InteractableMouse : MonoBehaviour
{
    private bool isScaling = false;
    private bool isMouseOver = false; // ✅ เช็คว่าตอนนี้เมาส์อยู่บนตัวหนูไหม
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite defaultSprite;       // ✅ Sprite ปกติ
    [SerializeField] private Sprite highlightedSprite;   // ✅ Sprite เมื่อเอาเมาส์ไปวาง

    [SerializeField] private float scaleDuration = 0.3f; // ✅ เวลาที่ใช้ในการเปลี่ยนขนาด
    private Coroutine scaleCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
    }

    private void Update()
    {
        if (CatMinigame.Instance.IsMouseRunning) return; // ✅ ถ้าหนูกำลังวิ่ง ห้ามย่อขยาย
        if (!isMouseOver) return; // ✅ ต้องเอาเมาส์ไปวางที่ตัวหนูเท่านั้นถึงจะย่อขยายได้

        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isScaling)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Upsize);
                StartSmoothScale(0.1f);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Smallsize);
                StartSmoothScale(-0.1f);
            }
        }
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;
        if (CatMinigame.Instance.IsMouseRunning) return; // ✅ ถ้าหนูกำลังวิ่ง ห้ามทำอะไร
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.enabled) return;

        isMouseOver = true; // ✅ เมาส์อยู่บนตัวหนูแล้ว

        if (MinigameManager.Instance.IsPlayingMinigame && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite;
            }
        }
        else
        {
            if (spriteRenderer != null && defaultSprite != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }

    private void OnMouseExit()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.enabled) return;

        isMouseOver = false; // ✅ เมาส์ออกจากตัวหนูแล้ว หยุดย่อขยาย

        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    private void StartSmoothScale(float scaleStep)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(SmoothScale(scaleStep));
    }

    private IEnumerator SmoothScale(float scaleStep)
    {
        isScaling = true;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale + Vector3.one * scaleStep;

        targetScale.x = Mathf.Clamp(targetScale.x, 0.5f, 1f);
        targetScale.y = Mathf.Clamp(targetScale.y, 0.5f, 1f);
        targetScale.z = Mathf.Clamp(targetScale.z, 0.5f, 1f);

        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        isScaling = false;

        CatMinigame.Instance.CheckMouseSize();
    }
}
