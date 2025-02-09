using UnityEngine;

public class PowerButton : MonoBehaviour
{
    private bool isOn = false;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private VinylDisc vinylDisc;
    [SerializeField] private Sprite defaultSprite;       // ✅ Sprite ปกติ
    [SerializeField] private Sprite highlightedSprite;   // ✅ Sprite เมื่อเอาเมาส์ไปวาง

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;

        if (ToolManager.Instance.CurrentMode != "Hand") return; // ✅ ต้องอยู่ในโหมด Hand เท่านั้น
        if (!TurntableMinigame.Instance.CanPressPower) return; // ✅ ถ้าไม่มีแผ่นเสียงห้ามเปิด

        isOn = !isOn;
        spriteRenderer.sprite = isOn ? onSprite : offSprite;

        if (isOn)
        {
            TurntableMinigame.Instance.TurnOn();
        }
        else
        {
            TurntableMinigame.Instance.TurnOff();
        }
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;
        if (vinylDisc.transform.position == vinylDisc.TrayPosition.position)
        {
            if (MinigameManager.Instance.IsPlayingMinigame && ToolManager.Instance.CurrentMode == "Hand")
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
        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}
