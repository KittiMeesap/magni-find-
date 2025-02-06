using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClockMinigame : MonoBehaviour
{
    public static ClockMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject clockMinigameObject;
    [SerializeField] private GameObject rewardItemPrefab;
    [SerializeField] private Transform clockTransform;
    [SerializeField] private GameObject clockStorageSprite;
    [SerializeField] private GameObject[] clockHands;
    [SerializeField] private GameObject[] clockNumbers;
    [SerializeField] private Vector3[] numberPositions;

    [SerializeField] private float animationDuration = 1.0f;
    [SerializeField] private float rewardItemSlideDistance = 2.0f;
    [SerializeField] private float clockStorageSlideDistance = 1.5f;
    [SerializeField] private Vector3 clockTargetScale = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private Vector3 clockTargetPositionOffset = new Vector3(0, 2.0f, 0);
    [SerializeField] private int totalParts = 15;

    private Vector3 initialClockScale;
    private Vector3 initialClockPosition;
    private Vector3 initialStoragePosition;
    private bool isPaused = false;
    private bool isFirstTime = true;

    private Dictionary<GameObject, Vector3> savedClockNumbersPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> savedClockNumbersScales = new Dictionary<GameObject, Vector3>();
    private int completedParts = 0;
    private int savedCompletedParts = 0; // ✅ บันทึก Part ที่ทำไปแล้ว

    public int CompletedParts => completedParts; // ✅ Getter สำหรับ CompletedParts
    public int TotalParts => totalParts; // ✅ Getter สำหรับ TotalParts
    [SerializeField] private InteractObject interactObject;

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

        initialClockScale = clockTransform.localScale;
        initialClockPosition = clockTransform.localPosition;

        if (clockStorageSprite != null)
        {
            initialStoragePosition = clockStorageSprite.transform.localPosition;
        }
    }

    private void ShuffleClockNumbers()
    {
        if (!isFirstTime) return; // ✅ สุ่มแค่ครั้งแรก ถ้าพักเกมจะไม่สุ่มใหม่

        if (clockNumbers.Length != 12 || numberPositions.Length != 12)
        {
            Debug.LogError("Clock numbers or positions are not properly assigned!");
            return;
        }

        List<int> availableIndexes = new List<int>();
        for (int i = 0; i < numberPositions.Length; i++)
        {
            availableIndexes.Add(i);
        }

        float[] fixedScales = { 0.4f, 0.5f, 0.6f }; // ✅ กำหนดขนาดตายตัว

        for (int i = 0; i < clockNumbers.Length; i++)
        {
            int randomIndex = Random.Range(0, availableIndexes.Count);
            int positionIndex = availableIndexes[randomIndex];

            clockNumbers[i].transform.localPosition = numberPositions[positionIndex];

            float randomScale = fixedScales[Random.Range(0, fixedScales.Length)]; // ✅ สุ่มขนาด ค่า
            clockNumbers[i].transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            availableIndexes.RemoveAt(randomIndex);
        }

        Debug.Log("Clock numbers shuffled successfully!");
        isFirstTime = false;
    }


    public void StartMinigame()
    {
        if (isPaused)
        {
            ResumeMinigame();
            MinigameManager.Instance.SetMinigameTrigger(triggerObject);
            return;
        }

        RestoreGameState(); // ✅ โหลดสถานะก่อนสุ่ม
        MinigameManager.Instance.StartMinigame(clockMinigameObject, clockTransform, rewardItemPrefab,true);
        MinigameManager.Instance.SetMinigameTrigger(triggerObject);
        ShuffleClockNumbers();
        SaveGameState(); // ✅ บันทึกสถานะหลังจากสุ่มเสร็จ
    }

    public void PauseMinigame()
    {
        isPaused = true;
        SaveGameState(); // ✅ บันทึกค่า `completedParts`
        MinigameManager.Instance.PauseMinigame();
    }

    public void ResumeMinigame()
    {
        isPaused = false;
        RestoreGameState(); // ✅ โหลดค่า `completedParts`
        MinigameManager.Instance.ResumeMinigame(clockMinigameObject, clockTransform, rewardItemPrefab, true);
    }

    private void SaveGameState()
    {
        savedClockNumbersPositions.Clear();
        savedClockNumbersScales.Clear();

        foreach (var number in clockNumbers)
        {
            savedClockNumbersPositions[number] = number.transform.localPosition;
            savedClockNumbersScales[number] = number.transform.localScale;
        }

        savedCompletedParts = completedParts; // ✅ บันทึก Part ที่ทำไปแล้ว

        Debug.Log($"Clock Minigame state saved. Completed Parts: {completedParts}/{totalParts}");
    }

    private void RestoreGameState()
    {
        foreach (var number in clockNumbers)
        {
            if (savedClockNumbersPositions.ContainsKey(number))
            {
                number.transform.localPosition = savedClockNumbersPositions[number];
            }
            if (savedClockNumbersScales.ContainsKey(number))
            {
                number.transform.localScale = savedClockNumbersScales[number];
            }
        }

        completedParts = savedCompletedParts; // ✅ โหลดค่า Part กลับมา
        Debug.Log($"Clock Minigame state restored. Completed Parts: {completedParts}/{totalParts}");
    }

    public void CompletePart()
    {
        completedParts++;
        SaveGameState(); // ✅ บันทึกค่าเมื่อทำสำเร็จ
        Debug.Log($"Clock Part completed: {completedParts}/{totalParts}");

        if (completedParts >= totalParts)
        {
            StartCoroutine(HandleMinigameCompletion());
        }
    }

    private IEnumerator HandleMinigameCompletion()
    {
        Debug.Log("Clock Minigame completed successfully! Starting animation...");

        Vector3 initialScale = clockTransform.localScale;
        Vector3 targetScale = clockTargetScale;
        Vector3 initialPosition = clockTransform.localPosition;
        Vector3 targetPosition = initialPosition + clockTargetPositionOffset;

        float elapsedTime = 0f;

        // ✅ ทำการขยายและขยับตำแหน่งของนาฬิกาขึ้นไป
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            clockTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            clockTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);

            yield return null;
        }

        clockTransform.localScale = targetScale;
        clockTransform.localPosition = targetPosition;

        Debug.Log("Clock animation completed. Sliding out storage first, then reward...");

        // ✅ เลื่อน Clock Storage ลงมา
        if (clockStorageSprite != null)
        {
            clockStorageSprite.SetActive(true);
            Vector3 initialStoragePosition = clockStorageSprite.transform.localPosition;
            Vector3 targetStoragePosition = initialStoragePosition - new Vector3(0, clockStorageSlideDistance, 0);

            elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / animationDuration;

                clockStorageSprite.transform.localPosition = Vector3.Lerp(initialStoragePosition, targetStoragePosition, t);
                yield return null;
            }

            clockStorageSprite.transform.localPosition = targetStoragePosition;
            Debug.Log("Clock storage slide-out completed.");
        }
        else
        {
            Debug.LogWarning("clockStorageSprite is NULL. Cannot slide out.");
        }

        // ✅ เลื่อน Reward ลงมา
        if (rewardItemPrefab != null)
        {
            rewardItemPrefab.SetActive(true);
            Vector3 initialRewardPosition = rewardItemPrefab.transform.localPosition;
            Vector3 targetRewardPosition = initialRewardPosition - new Vector3(0, rewardItemSlideDistance, 0);

            elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / animationDuration;

                rewardItemPrefab.transform.localPosition = Vector3.Lerp(initialRewardPosition, targetRewardPosition, t);
                yield return null;
            }

            rewardItemPrefab.transform.localPosition = targetRewardPosition;
            Debug.Log("Reward item slide-out completed.");
        }
        else
        {
            Debug.LogWarning("rewardItemPrefab is NULL. Cannot slide out.");
        }

        Debug.Log("Clock Minigame completion animation finished. Waiting for reward collection.");
        interactObject.CheckMinigameDone();
    }


    public void DecreaseCompletedParts()
    {
        if (completedParts > 0)
        {
            completedParts--;
            SaveGameState(); // ✅ อัปเดตค่าเมื่อทำผิด
            Debug.Log($"Clock Part removed: {completedParts}/{totalParts}");
        }
    }
}
