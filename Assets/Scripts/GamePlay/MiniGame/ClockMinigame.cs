// ClockMinigame.cs
using UnityEngine;

public class ClockMinigame : MonoBehaviour
{
    public static ClockMinigame Instance { get; private set; }

    [SerializeField] private GameObject clockMinigameUI;
    [SerializeField] private int totalParts = 15; // 12 ตัวเลข + 3 เข็ม
    private int completedParts = 0; // จำนวนชิ้นส่วนที่สำเร็จแล้ว

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
        isPlayingClockMinigame = true;
        completedParts = 0;
        clockMinigameUI.SetActive(true);
        Debug.Log("Clock Minigame started.");
    }

    public void CompletePart()
    {
        completedParts++;
        Debug.Log($"Part completed: {completedParts}/{totalParts}");

        if (completedParts >= totalParts)
        {
            EndMinigame(true);
        }
    }

    public void EndMinigame(bool success)
    {
        isPlayingClockMinigame = false;
        clockMinigameUI.SetActive(false);

        if (success)
        {
            Debug.Log("Clock Minigame completed successfully!");
        }
        else
        {
            Debug.Log("Clock Minigame failed.");
        }
    }
}
