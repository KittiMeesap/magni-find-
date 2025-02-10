using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private ProgressManager progressManager;
    [SerializeField] private CollectibleItem[] collectibleItems;
    [SerializeField] private Button playSoundButton; // ✅ ปุ่ม UI ที่จะกดเล่นเสียง
    [SerializeField] private AudioClip soundClip; // ✅ เสียงที่ต้องการเล่น
    [SerializeField] private Image arrowImage; // ✅ ลูกศร UI

    public float bounceHeight = 20f; // ✅ ระยะขยับขึ้นลง
    public float bounceSpeed = 1f;   // ✅ ความเร็วในการขยับ

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (progressManager == null)
        {
            Debug.LogError("ProgressManager not assigned in GameController!");
            return;
        }

        foreach (var item in collectibleItems)
        {
            item.Initialize(progressManager); // ส่ง ProgressManager ให้ CollectibleItem
        }

        ToolManager.Instance.Initialize(collectibleItems); // ส่ง Collectible Items ไปยัง ToolManager

        if (playSoundButton != null)
        {
            playSoundButton.onClick.AddListener(PlaySoundFromButton); // ✅ เพิ่ม Listener ให้ปุ่ม
        }
        else
        {
            Debug.LogError("❌ playSoundButton ไม่ได้ถูกกำหนดใน Inspector!");
        }

        // ✅ เริ่มให้ลูกศรขยับขึ้นลง
        if (arrowImage != null)
        {
            StartCoroutine(BounceArrow());
        }
        else
        {
            Debug.LogError("❌ arrowImage ไม่ได้ถูกกำหนดใน Inspector!");
        }
    }

    private void PlaySoundFromButton()
    {
        if (SoundManager.Instance != null && soundClip != null)
        {
            SoundManager.Instance.PlaySFX(soundClip);
        }
        else
        {
            Debug.LogError("❌ SoundManager.Instance หรือ soundClip เป็น Null!");
        }

        // ✅ ปิดลูกศรเมื่อกดปุ่ม
        if (arrowImage != null)
        {
            arrowImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator BounceArrow()
    {
        RectTransform arrowTransform = arrowImage.rectTransform;
        Vector2 startPos = arrowTransform.anchoredPosition;

        while (arrowImage.gameObject.activeSelf)
        {
            float elapsedTime = 0f;
            while (elapsedTime < bounceSpeed)
            {
                elapsedTime += Time.deltaTime;
                float newY = startPos.y + Mathf.Sin(elapsedTime * Mathf.PI) * bounceHeight;
                arrowTransform.anchoredPosition = new Vector2(startPos.x, newY);
                yield return null;
            }
        }
    }
}
