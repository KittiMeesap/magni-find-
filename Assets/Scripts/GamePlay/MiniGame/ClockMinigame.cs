// ClockMinigame.cs
using UnityEngine;
using System.Collections;

public class ClockMinigame : MonoBehaviour
{
    public static ClockMinigame Instance { get; private set; }

    [SerializeField] private GameObject clockMinigamePrefab; // Prefab ของมินิเกม
    [SerializeField] private Transform minigameParent; // ตำแหน่งที่ Prefab จะถูกสร้าง
    [SerializeField] private GameObject rewardItem; // ไอเท็มรางวัลที่ซ่อนอยู่ในซีน
    [SerializeField] private int totalParts = 15; // 12 ตัวเลข + 3 เข็ม
    private int completedParts = 0; // จำนวนชิ้นส่วนที่สำเร็จแล้ว

    private GameObject currentMinigameInstance; // เก็บอินสแตนซ์ของมินิเกมที่สร้าง
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
    }

    public void StartMinigame()
    {
        if (clockMinigamePrefab != null && minigameParent != null)
        {
            // สร้าง Prefab ใหม่
            currentMinigameInstance = Instantiate(clockMinigamePrefab, minigameParent);
            isPlayingClockMinigame = true;
            completedParts = 0;
            Debug.Log("Clock Minigame started.");
        }
        else
        {
            Debug.LogError("Clock Minigame Prefab or Parent not assigned.");
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
        Debug.Log("Clock Minigame completed successfully! Reward will be shown in 3 seconds...");

        yield return new WaitForSeconds(3f); // รอ 3 วินาที

        // แสดงไอเท็มรางวัลที่ซ่อนอยู่
        if (rewardItem != null)
        {
            rewardItem.SetActive(true);
            Debug.Log("Reward item is now visible. Player can collect it.");
        }

        // ลบ Prefab ของมินิเกมเมื่อจบ
        if (currentMinigameInstance != null)
        {
            Destroy(currentMinigameInstance);
        }
    }

    public void EndMinigame()
    {
        isPlayingClockMinigame = false;

        // ลบ Prefab ของมินิเกมเมื่อถูกปิด
        if (currentMinigameInstance != null)
        {
            Destroy(currentMinigameInstance);
        }

        Debug.Log("Clock Minigame ended by player.");
    }
}
