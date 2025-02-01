using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PictureMinigame : MonoBehaviour
{
    public static PictureMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject pictureMinigame;
    [SerializeField] private Transform minigameTransform;
    [SerializeField] private GameObject rewardItem;
    [SerializeField] private PictureSlot[] slots; // 3 ช่องสำหรับภาพ
    [SerializeField] private float slideSpeed = 5f;
    private bool isPlaying = false;

    private int correctCount = 0;
    private int totalSlots = 3; // ต้องถูก 3 ช่อง

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
            StartCoroutine(ShowReward());
        }
    }

    private IEnumerator ShowReward()
    {
        yield return new WaitForSeconds(0.5f);
        rewardItem.SetActive(true);
        Debug.Log("Minigame completed! Reward unlocked.");
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
