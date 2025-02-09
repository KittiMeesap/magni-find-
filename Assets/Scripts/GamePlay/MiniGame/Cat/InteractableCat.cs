using UnityEngine;
using System.Collections;
using System;

public class InteractableCat : MonoBehaviour
{
    [SerializeField] private Sprite fatCatSprite; // ✅ แมวอ้วน
    [SerializeField] private Sprite thinCatSprite; // ✅ แมวผอม
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite defaultSprite;       // ✅ Sprite ปกติ
    [SerializeField] private Sprite highlightedSprite;   // ✅ Sprite เมื่อเอาเมาส์ไปวาง

    private Collider2D col;
    private bool isClicked = false; // ✅ กันการเปลี่ยน Sprite ซ้ำ
    private bool isMouseOver = false; // ✅ เช็คว่าตอนนี้เมาส์อยู่บนแมวหรือไม่

    [SerializeField] private float fadeDuration = 0.5f; // ✅ ระยะเวลาในการเปลี่ยน sprite ให้เนียน
    [SerializeField] private float minAlpha = 0.5f; // ✅ ค่าโปร่งใสต่ำสุด (ทำให้แมวไม่จางจนหายไป)

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
    }

    private void Update()
    {
        if (isClicked || col == null || !col.enabled) return; // ✅ ถ้าเคยคลิกไปแล้ว หรือ Collider ปิด ให้ return
        if (!isMouseOver) return; // ✅ ต้องอยู่บนตัวแมวเท่านั้นถึงจะกดได้

        if (ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (Input.GetMouseButtonDown(0)) // ✅ คลิกซ้าย -> แมวอ้วน
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Upsize);
                Debug.Log("คลิกซ้าย -> เปลี่ยนเป็นแมวอ้วน");

                StartCoroutine(FadeChangeSprite(fatCatSprite, () =>
                {
                    StartCoroutine(CatMinigame.Instance.FadeInMouse());
                }));

                CatMinigame.Instance.SetCatState("fat");
                DisableColliderAfterClick();
            }
            else if (Input.GetMouseButtonDown(1)) // ✅ คลิกขวา -> แมวผอม
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Smallsize);
                Debug.Log("คลิกขวา -> เปลี่ยนเป็นแมวผอม");
                StartCoroutine(FadeChangeSprite(thinCatSprite, () =>
                {
                    StartCoroutine(CatMinigame.Instance.FadeInMouse());
                }));
                CatMinigame.Instance.SetCatState("thin");
                DisableColliderAfterClick();
            }
        }
    }

    private void DisableColliderAfterClick()
    {
        isClicked = true;
        if (col != null)
        {
            col.enabled = false; // ✅ ปิด Collider หลังจากคลิก
            Debug.Log("Collider ปิดแล้ว");
        }
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;
        if (col == null || !col.enabled) return;

        isMouseOver = true; // ✅ เมาส์อยู่บนตัวแมวแล้ว

        if (spriteRenderer != null)
        {
            Color spriteColor = spriteRenderer.color;
            if (Mathf.RoundToInt(spriteColor.a * 255) != 255) return; // ✅ ถ้า Alpha ไม่ใช่ 255 ให้ return
        }

        if (MinigameManager.Instance.IsPlayingMinigame && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite;
            }
            else if (spriteRenderer != null && defaultSprite != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }

    private void OnMouseExit()
    {

        if (col == null || !col.enabled) return;

        isMouseOver = false; // ✅ เมาส์ออกจากตัวแมวแล้ว หยุดให้กดเปลี่ยนได้

        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    private IEnumerator FadeChangeSprite(Sprite newSprite, Action onComplete)
    {
        
        if (spriteRenderer == null) yield break;
        DialogueUI.Instance.DialogueButton(false);

        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, minAlpha, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        spriteRenderer.sprite = newSprite; // ✅ เปลี่ยน Sprite ใหม่

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(minAlpha, 1f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        onComplete?.Invoke(); // ✅ เรียก Callback เมื่อทำงานเสร็จ
    }
}
