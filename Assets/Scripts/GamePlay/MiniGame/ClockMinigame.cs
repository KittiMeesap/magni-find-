using UnityEngine;
using System.Collections;

public class ClockMinigame : MonoBehaviour
{
    public static ClockMinigame Instance { get; private set; }

    [SerializeField] private GameObject clockMinigameObject; // GameObject ของมินิเกมที่ซ่อนไว้ในซีน
    [SerializeField] private GameObject rewardItemPrefab; // Prefab ของไอเท็มรางวัล
    [SerializeField] private Transform clockTransform; // Transform ของตัวนาฬิกา
    [SerializeField] private GameObject clockStorageSprite; // Sprite ช่องเก็บของที่เป็นรูปภาพ
    [SerializeField] private GameObject[] clockHands; // เข็มนาฬิกา
    [SerializeField] private GameObject[] clockNumbers; // ตัวเลขนาฬิกา
    [SerializeField] private float animationDuration = 1.0f; // ระยะเวลาของ Animation
    [SerializeField] private float rewardItemSlideDistance = 2.0f; // ระยะทางที่ไอเท็มเลื่อนออกมา
    [SerializeField] private float clockStorageSlideDistance = 1.5f; // ระยะทางที่ช่องเก็บของเลื่อนออกมา
    [SerializeField] private Vector3 clockTargetScale = new Vector3(0.8f, 0.8f, 0.8f); // ขนาดเป้าหมายของนาฬิกา
    [SerializeField] private Vector3 clockTargetPositionOffset = new Vector3(0, 2.0f, 0); // การเลื่อนตำแหน่งเป้าหมาย
    [SerializeField] private int totalParts = 15; // 12 ตัวเลข + 3 เข็ม

    private Vector3 initialClockScale;
    private Vector3 initialClockPosition;
    private Vector3 initialStoragePosition;
    private Vector3[] initialHandsPositions;
    private Quaternion[] initialHandsRotations;
    private int[] initialHandsIndices; // เก็บค่า Initial Position Index ของเข็ม
    private Vector3[] initialNumbersPositions;
    private Quaternion[] initialNumbersRotations;
    private int completedParts = 0;
    public bool isPlayingClockMinigame = false;

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

        // เก็บตำแหน่งและขนาดเริ่มต้น
        initialClockScale = clockTransform.localScale;
        initialClockPosition = clockTransform.localPosition;

        if (clockStorageSprite != null)
        {
            initialStoragePosition = clockStorageSprite.transform.localPosition;
        }

        // เก็บตำแหน่งและการหมุนเริ่มต้นของเข็มนาฬิกา
        initialHandsPositions = new Vector3[clockHands.Length];
        initialHandsRotations = new Quaternion[clockHands.Length];
        initialHandsIndices = new int[clockHands.Length];
        for (int i = 0; i < clockHands.Length; i++)
        {
            initialHandsPositions[i] = clockHands[i].transform.localPosition;
            initialHandsRotations[i] = clockHands[i].transform.localRotation;

            // เก็บค่า Initial Position Index จาก InteractableClockHand
            var clockHandScript = clockHands[i].GetComponent<InteractableClockHand>();
            if (clockHandScript != null)
            {
                initialHandsIndices[i] = clockHandScript.initialPositionIndex;
            }
        }

        // เก็บตำแหน่งและการหมุนเริ่มต้นของตัวเลขนาฬิกา
        initialNumbersPositions = new Vector3[clockNumbers.Length];
        initialNumbersRotations = new Quaternion[clockNumbers.Length];
        for (int i = 0; i < clockNumbers.Length; i++)
        {
            initialNumbersPositions[i] = clockNumbers[i].transform.localPosition;
            initialNumbersRotations[i] = clockNumbers[i].transform.localRotation;
        }
    }

    private void Update()
    {
        // ตรวจจับการกด ESC เพื่อปิดมินิเกม
        if (isPlayingClockMinigame && Input.GetKeyDown(KeyCode.Escape))
        {
            EndMinigameClick();
        }
    }

    public void StartMinigame()
    {
        if (clockMinigameObject != null)
        {
            // รีเซ็ตตำแหน่งและขนาดของนาฬิกา
            clockTransform.localScale = initialClockScale;
            clockTransform.localPosition = initialClockPosition;

            if (clockStorageSprite != null)
            {
                clockStorageSprite.transform.localPosition = initialStoragePosition;
            }

            // รีเซ็ตตำแหน่ง การหมุน และค่า Initial Position Index ของเข็มนาฬิกา
            for (int i = 0; i < clockHands.Length; i++)
            {
                clockHands[i].transform.localPosition = initialHandsPositions[i];
                clockHands[i].transform.localRotation = initialHandsRotations[i];

                var clockHandScript = clockHands[i].GetComponent<InteractableClockHand>();
                if (clockHandScript != null)
                {
                    clockHandScript.ResetToInitialPosition(initialHandsIndices[i]);
                }
            }

            // รีเซ็ตตำแหน่งและการหมุนของตัวเลขนาฬิกา
            for (int i = 0; i < clockNumbers.Length; i++)
            {
                clockNumbers[i].transform.localPosition = initialNumbersPositions[i];
                clockNumbers[i].transform.localRotation = initialNumbersRotations[i];
            }

            // เปิดใช้งาน GameObject ของมินิเกม
            clockMinigameObject.SetActive(true);
            isPlayingClockMinigame = true;
            completedParts = 0;
            Debug.Log("Clock Minigame started.");
        }
        else
        {
            Debug.LogError("Clock Minigame Object not assigned.");
        }
    }

    public void CompletePart()
    {
        completedParts++;
        Debug.Log($"Part completed: {completedParts}/{totalParts}");

        if (completedParts >= totalParts)
        {
            StartCoroutine(HandleMinigameCompletion());
        }
    }

    public void DecreaseCompletedParts()
    {
        if (completedParts > 0)
        {
            completedParts--;
            Debug.Log($"Part removed: {completedParts}/{totalParts}");
        }
    }

    private IEnumerator HandleMinigameCompletion()
    {
        isPlayingClockMinigame = false;
        Debug.Log("Clock Minigame completed successfully! Starting animation...");

        // เริ่ม Animation การย่อตัวและเลื่อนนาฬิกา
        Vector3 initialScale = clockTransform.localScale;
        Vector3 targetScale = clockTargetScale;
        Vector3 initialPosition = clockTransform.localPosition;
        Vector3 targetPosition = initialPosition + clockTargetPositionOffset;

        float elapsedTime = 0f;
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

        clockStorageSprite.SetActive(true);
        // เลื่อนกล่องเก็บของออกมาด้านล่าง
        if (clockStorageSprite != null)
        {
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

        rewardItemPrefab.SetActive(true);
        // เลื่อนไอเท็มรางวัลออกมาด้านล่าง
        if (rewardItemPrefab != null)
        {
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

        Debug.Log("Waiting for player to collect the reward.");
    }

    public void OnRewardCollected()
    {
        Debug.Log("Reward collected. Starting fade-out animation and closing minigame...");
        StartCoroutine(FadeClockAndClose());
    }

    private IEnumerator FadeClockAndClose()
    {
        // ดึง SpriteRenderer ทั้งหมดของนาฬิกาและลูกๆ
        SpriteRenderer[] spriteRenderers = clockTransform.GetComponentsInChildren<SpriteRenderer>();

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            Debug.LogError("No SpriteRenderers found on clockTransform or its children. Cannot fade out.");
            yield break;
        }

        float elapsedTime = 0f;

        // เก็บสีเริ่มต้นของ SpriteRenderer ทุกตัว
        Color[] initialColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null) // ตรวจสอบว่า SpriteRenderer ไม่เป็น null
            {
                initialColors[i] = spriteRenderers[i].color;
            }
        }

        // เริ่มการจางหาย
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] != null) // ตรวจสอบอีกครั้งก่อนปรับค่า
                {
                    // Lerp ค่า Alpha ของ SpriteRenderer ทุกตัว
                    Color targetColor = new Color(initialColors[i].r, initialColors[i].g, initialColors[i].b, 0f);
                    spriteRenderers[i].color = Color.Lerp(initialColors[i], targetColor, t);
                }
            }

            yield return null; // รอเฟรมถัดไป
        }

        // ให้แน่ใจว่า Alpha = 0 สำหรับทุก SpriteRenderer
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null) // ตรวจสอบอีกครั้งก่อนตั้งค่า
            {
                Color finalColor = new Color(initialColors[i].r, initialColors[i].g, initialColors[i].b, 0f);
                spriteRenderers[i].color = finalColor;
            }
        }

        Debug.Log("Clock fade-out animation completed. Closing minigame.");

        // ปิดมินิเกมหลังจาก Animation เสร็จสิ้น
        EndMinigame();
    }




    [SerializeField] private GameObject specificGameObject; // GameObject ที่ต้องปิด Collider2D

    public void EndMinigame()
    {
        isPlayingClockMinigame = false;

        // ปิดการใช้งาน Collider2D ใน clockMinigameObject และลูกของมัน
        if (clockMinigameObject != null)
        {
            Collider2D[] colliders = clockMinigameObject.GetComponentsInChildren<Collider2D>(true); // หา Collider2D ทั้งหมดในลูกของ clockMinigameObject
            foreach (var collider in colliders)
            {
                collider.enabled = false; // ปิด Collider2D
            }

            clockMinigameObject.SetActive(false); // ปิด GameObject
            Debug.Log("All Collider2D in Clock Minigame have been disabled.");
        }

        // ปิดการใช้งาน Collider2D ใน specificGameObject ที่กำหนดเอง
        if (specificGameObject != null)
        {
            Collider2D collider = specificGameObject.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false; // ปิด Collider2D
                Debug.Log($"Collider2D on {specificGameObject.name} has been disabled.");
            }
            else
            {
                Debug.LogWarning($"No Collider2D found on {specificGameObject.name}.");
            }
        }

        Debug.Log("Clock Minigame ended by player.");
    }

    public void EndMinigameClick()
    {
        isPlayingClockMinigame = false;
        clockMinigameObject.SetActive(false); // ปิด GameObject

        Debug.Log("Clock Minigame ended by player.");
    }
}
