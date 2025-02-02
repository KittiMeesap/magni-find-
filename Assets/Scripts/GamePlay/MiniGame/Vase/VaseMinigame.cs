using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VaseMinigame : MonoBehaviour
{
    public static VaseMinigame Instance { get; private set; }


    private int totalParts = 3;
    private int completedParts = 0;
    private int savedCompletedParts = 0; // ✅ บันทึกแต้มที่ทำสำเร็จ

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject vaseMinigameObject;
    [SerializeField] private Transform vaseTransform;
    [SerializeField] private DialogueManager targetDialogueSystem;
    [SerializeField] private GameObject rewardItemPrefab;
    [SerializeField] private float rewardItemSlideDistance = 2.0f;
    [SerializeField] private float animationDuration = 1f;

    [SerializeField] private Transform frogKingTransform;
    [SerializeField] private Transform frogQueenTransform;
    [SerializeField] private Transform frogKnightTransform;

    private Vector3 frogKingInitialPosition;
    private Vector3 frogQueenInitialPosition;
    private Vector3 frogKnightInitialPosition;

    private Vector3 initialVaseScale;
    private Vector3 initialVasePosition;
    private bool isPaused = false; // ✅ เช็คว่าเกมถูกพักไว้หรือไม่

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // ✅ เก็บค่าตำแหน่งเริ่มต้น
        if (vaseTransform != null)
        {
            initialVaseScale = vaseTransform.localScale;
            initialVasePosition = vaseTransform.localPosition;
        }

        if (frogKingTransform != null) frogKingInitialPosition = frogKingTransform.localPosition;
        if (frogQueenTransform != null) frogQueenInitialPosition = frogQueenTransform.localPosition;
        if (frogKnightTransform != null) frogKnightInitialPosition = frogKnightTransform.localPosition;
    }

    public void StartMinigame()
    {
        if (isPaused)
        {
            ResumeMinigame();
            MinigameManager.Instance.SetMinigameTrigger(triggerObject);
            return;
        }

        completedParts = savedCompletedParts; // ✅ โหลดค่าที่ทำสำเร็จแล้ว
        MinigameManager.Instance.StartMinigame(vaseMinigameObject, vaseTransform, rewardItemPrefab, true);
        MinigameManager.Instance.SetMinigameTrigger(triggerObject);
    }

    public void PauseMinigame()
    {
        isPaused = true;
        savedCompletedParts = completedParts; // ✅ บันทึกค่าที่ทำสำเร็จแล้ว
        MinigameManager.Instance.PauseMinigame();
        Debug.Log($"Vase Minigame Paused. Saved completed parts: {savedCompletedParts}");
    }

    public void ResumeMinigame()
    {
        isPaused = false;
        completedParts = savedCompletedParts; // ✅ โหลดค่าที่บันทึกไว้
        MinigameManager.Instance.ResumeMinigame(vaseMinigameObject, vaseTransform, rewardItemPrefab, true);
        Debug.Log($"Vase Minigame Resumed. Loaded completed parts: {completedParts}");
    }

    public void CompletePart()
    {
        completedParts++;
        savedCompletedParts = completedParts; // ✅ บันทึกแต้มที่ทำสำเร็จ

        Debug.Log($"Vase Part completed: {completedParts}/{totalParts}");

        if (completedParts >= totalParts)
        {
            StartCoroutine(HandleMinigameCompletion());
        }
    }

    private IEnumerator HandleMinigameCompletion()
    {
        Debug.Log("Vase Minigame completed successfully! Starting animation...");

        Vector3 initialScale = vaseTransform.localScale;
        Vector3 targetScale = vaseTransform.localScale * 0.5f;
        Vector3 initialPosition = vaseTransform.localPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0, -2f, 0);

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            vaseTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            vaseTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);

            yield return null;
        }

        vaseTransform.localScale = targetScale;
        vaseTransform.localPosition = targetPosition;

        Debug.Log("Vase animation completed. Now making the reward float up...");

        if (rewardItemPrefab != null)
        {
            rewardItemPrefab.SetActive(true);
            Vector3 initialRewardPosition = rewardItemPrefab.transform.localPosition;
            Vector3 targetRewardPosition = initialRewardPosition + new Vector3(0, rewardItemSlideDistance, 0);

            elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / animationDuration;

                rewardItemPrefab.transform.localPosition = Vector3.Lerp(initialRewardPosition, targetRewardPosition, t);
                yield return null;
            }

            rewardItemPrefab.transform.localPosition = targetRewardPosition;
            Debug.Log("Reward item floated up successfully.");
        }

        Debug.Log("Waiting for player to collect the reward.");
    }

    public void DecreaseCompletedParts()
    {
        if (completedParts > 0)
        {
            completedParts--;
            savedCompletedParts = completedParts; // ✅ บันทึกค่าเมื่อแต้มลดลง
            Debug.Log($"Vase Part removed: {completedParts}/{totalParts}");
        }
    }
}
