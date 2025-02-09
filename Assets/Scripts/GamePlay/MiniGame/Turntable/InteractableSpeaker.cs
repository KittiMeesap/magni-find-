using UnityEngine;

public class Speaker : MonoBehaviour
{
    public static Speaker Instance { get; private set; }

    [SerializeField] private Vector3 minScale = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private GameObject soundSymbol;

    private int currentLevel = 1;
    private int minLevel = 1;
    private int maxLevel = 5;
    private bool canScale = false;
    private bool isMouseOver = false; // ✅ ตรวจสอบว่าเมาส์อยู่บนลำโพงหรือไม่

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;       // ✅ Sprite ปกติ
    [SerializeField] private Sprite highlightedSprite;   // ✅ Sprite เมื่อเอาเมาส์ไปวาง

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
    }

    private void Update()
    {
        if (!canScale || !isMouseOver) return; // ✅ ต้องเปิดระบบ และเมาส์อยู่บน Object

        if (ToolManager.Instance.CurrentMode == "Magnifier") // ✅ ต้องเป็นโหมด Magnifier
        {
            if (Input.GetMouseButtonDown(0)) // ✅ คลิกซ้าย -> ขยาย
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Upsize);
                ChangeSpeakerSize(1);
            }
            else if (Input.GetMouseButtonDown(1)) // ✅ คลิกขวา -> ย่อ
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Smallsize);
                ChangeSpeakerSize(-1);
            }
        }
    }

    private void ChangeSpeakerSize(int change)
    {
        currentLevel = Mathf.Clamp(currentLevel + change, minLevel, maxLevel);
        UpdateSpeakerSize();
    }

    private void UpdateSpeakerSize()
    {
        float t = (float)(currentLevel - minLevel) / (maxLevel - minLevel);
        transform.localScale = Vector3.Lerp(minScale, maxScale, t);

        // ✅ ปรับ Alpha ของ Sound Symbol ตามระดับเสียง
        float alpha = Mathf.Lerp(0f, 1f, t);
        soundSymbol.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);

        SoundManager.Instance.SetMinigameVolume(alpha);
    }

    public void EnableScaling(bool enable)
    {
        canScale = enable;
    }

    public int CurrentLevel { get { return currentLevel; } } // ✅ เช็คระดับเสียงปัจจุบัน

    public void SetInitialVolume(int level)
    {
        currentLevel = Mathf.Clamp(level, minLevel, maxLevel);
        UpdateSpeakerSize();
    }

    // ✅ ฟังก์ชันช่วยคำนวณระดับเสียงตามระดับของลำโพง
    public float GetVolumeMultiplier()
    {
        return (float)(currentLevel - minLevel) / (maxLevel - minLevel);
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;
        isMouseOver = true;
        if (TurntableMinigame.Instance.IsOn)
        {
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
        if (Time.timeScale == 0f) return;

        isMouseOver = false;
        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

}
