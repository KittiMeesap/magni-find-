using UnityEngine;
using System.Collections;

public class Speaker : MonoBehaviour
{
    public static Speaker Instance { get; private set; }

    [SerializeField] private Vector3 minScale = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private GameObject soundSymbol;
    [SerializeField] private VinylDisc vinylDisc;

    private int currentLevel = 1;
    private int minLevel = 1;
    private int maxLevel = 5;
    private bool canScale = false;
    private bool isMouseOver = false;
    private bool isScaling = false; // ✅ ป้องกันการกดซ้ำระหว่างย่อขยาย

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;

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
        if (!canScale || !isMouseOver || isScaling) return; // ✅ ป้องกันการเปลี่ยนขนาดซ้ำซ้อน

        if (ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (Input.GetMouseButtonDown(0))
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Upsize);
                StartCoroutine(SmoothScaleCoroutine(1));
            }
            else if (Input.GetMouseButtonDown(1))
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Smallsize);
                StartCoroutine(SmoothScaleCoroutine(-1));
            }
        }
    }

    private IEnumerator SmoothScaleCoroutine(int change)
    {
        isScaling = true;
        int targetLevel = Mathf.Clamp(currentLevel + change, minLevel, maxLevel);

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.Lerp(minScale, maxScale, (float)(targetLevel - minLevel) / (maxLevel - minLevel));

        float duration = 0.3f; // ✅ ใช้เวลา 0.3 วินาทีในการย่อขยาย
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);

            // ✅ ปรับระดับเสียงของมินิเกมแบบ Smooth
            float volume = Mathf.Lerp(0f, 1f, (float)(targetLevel - minLevel) / (maxLevel - minLevel));
            SoundManager.Instance.SetMinigameVolume(volume);

            yield return null;
        }

        transform.localScale = targetScale;
        currentLevel = targetLevel;
        isScaling = false;
        UpdateSpeakerSize();
    }

    public void EnableScaling(bool enable)
    {
        canScale = enable;
    }

    public int CurrentLevel { get { return currentLevel; } }

    public void SetInitialVolume(int level)
    {
        currentLevel = Mathf.Clamp(level, minLevel, maxLevel);
        UpdateSpeakerSize();
    }

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


    private void UpdateSpeakerSize()
    {
        float t = (float)(currentLevel - minLevel) / (maxLevel - minLevel);
        //transform.localScale = Vector3.Lerp(minScale, maxScale, t);

        // ✅ ปรับ Alpha ของ Sound Symbol ตามระดับเสียง
        float alpha = Mathf.Lerp(0f, 1f, t);
        soundSymbol.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);

        //SoundManager.Instance.SetMinigameVolume(alpha);
    }
}
