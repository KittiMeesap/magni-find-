using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PictureMinigame : MonoBehaviour
{
    public static PictureMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject pictureMinigame;
    [SerializeField] private Transform minigameTransform;
    [SerializeField] private GameObject rewardItem; // ✅ แว่นตา (Hidden Item)
    [SerializeField] private PictureSlot[] slots; // ✅ 3 
    [SerializeField] private GameObject oldPictureObject; // ✅ GameObject รูปเก่า
    [SerializeField] private GameObject newPictureObject; // ✅ GameObject รูปใหม่
    [SerializeField] private float fadeDuration = 1f; // ✅ ระยะเวลาเฟด
    private bool isPlaying = false;

    [SerializeField] private InteractObject interactObject;

    private int correctCount = 0;
    private int totalSlots = 3; // ✅ ต้องถูก 3 ช่อง
    private bool pictureMinigameDone = false;
    public bool PictureMinigameDone => pictureMinigameDone;

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

        // ✅ ซ่อนรูปใหม่และแว่นตาตอนเริ่ม
        newPictureObject.SetActive(false);
        rewardItem.SetActive(false);
    }

    public void StartMinigame()
    {
        isPlaying = true;
        correctCount = 0;

        foreach (var slot in slots)
        {
            slot.ResetSlot();
            slot.OnSlotCorrect += CheckCompletion;
        }

        MinigameManager.Instance.StartMinigame(pictureMinigame, minigameTransform, rewardItem, true);
        MinigameManager.Instance.SetMinigameTrigger(triggerObject);
    }

    public void CheckCompletion()
    {
        correctCount = 0;
        foreach (var slot in slots)
        {
            if (slot.IsCorrect()) correctCount++;
        }

        if (correctCount >= totalSlots)
        {
            pictureMinigameDone = true;
            StartCoroutine(FadeOldPictureAndShowNew());
        }
    }

    // ✅ เฟดภาพเก่าให้เหลือ 50% → เฟดภาพใหม่ + แว่นตาซ้อนขึ้นมา → ปิดภาพเก่า
    private IEnumerator FadeOldPictureAndShowNew()
    {
        SpriteRenderer oldSR = oldPictureObject.GetComponent<SpriteRenderer>();
        SpriteRenderer newSR = newPictureObject.GetComponent<SpriteRenderer>();
        SpriteRenderer rewardSR = rewardItem.GetComponent<SpriteRenderer>();

        if (oldSR == null || newSR == null || rewardSR == null) yield break;

        float elapsedTime = 0f;

        // ✅ เปิดภาพใหม่และรางวัลทันที แต่ยังไม่แสดง (Alpha = 0)
        newPictureObject.SetActive(true);
        rewardItem.SetActive(true);
        newSR.color = new Color(1f, 1f, 1f, 0f);
        rewardSR.color = new Color(1f, 1f, 1f, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeProgress = elapsedTime / fadeDuration;

            // ✅ ภาพเก่าค่อย ๆ หายจาก 100% → 0%
            float oldAlpha = Mathf.Lerp(1f, 0f, fadeProgress);
            oldSR.color = new Color(1f, 1f, 1f, oldAlpha);

            // ✅ ภาพใหม่ + รางวัลค่อย ๆ เฟดเข้า 0% → 100% **พร้อมกับภาพเก่าที่หายไป**
            float newAlpha = Mathf.Lerp(0f, 1f, fadeProgress);
            newSR.color = new Color(1f, 1f, 1f, newAlpha);
            rewardSR.color = new Color(1f, 1f, 1f, newAlpha);

            yield return null;
        }

        // ✅ ปิดภาพเก่าเมื่อเฟดจบ
        oldPictureObject.SetActive(false);

        // ✅ แจ้งว่า Minigame สำเร็จ
        interactObject.CheckMinigameDone();
    }



    public void PauseMinigame()
    {
        isPlaying = false;
        MinigameManager.Instance.PauseMinigame();
    }

    public void ResumeMinigame()
    {
        isPlaying = true;
        MinigameManager.Instance.ResumeMinigame(pictureMinigame, minigameTransform, rewardItem, true);
    }
}
