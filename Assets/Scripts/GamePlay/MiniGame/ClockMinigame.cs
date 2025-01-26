// ClockMinigame.cs
using UnityEngine;
using System.Collections;

public class ClockMinigame : MonoBehaviour
{
    public static ClockMinigame Instance { get; private set; }

    [SerializeField] private GameObject clockMinigamePrefab; // Prefab �ͧ�Թ���
    [SerializeField] private Transform minigameParent; // ���˹觷�� Prefab �ж١���ҧ
    [SerializeField] private GameObject rewardItem; // ������ҧ��ŷ���͹����㹫չ
    [SerializeField] private int totalParts = 15; // 12 ����Ţ + 3 ���
    private int completedParts = 0; // �ӹǹ�����ǹ������������

    private GameObject currentMinigameInstance; // ���Թ�ᵹ��ͧ�Թ���������ҧ
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
            // ���ҧ Prefab ����
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

        yield return new WaitForSeconds(3f); // �� 3 �Թҷ�

        // �ʴ�������ҧ��ŷ���͹����
        if (rewardItem != null)
        {
            rewardItem.SetActive(true);
            Debug.Log("Reward item is now visible. Player can collect it.");
        }

        // ź Prefab �ͧ�Թ�������ͨ�
        if (currentMinigameInstance != null)
        {
            Destroy(currentMinigameInstance);
        }
    }

    public void EndMinigame()
    {
        isPlayingClockMinigame = false;

        // ź Prefab �ͧ�Թ�������Ͷ١�Դ
        if (currentMinigameInstance != null)
        {
            Destroy(currentMinigameInstance);
        }

        Debug.Log("Clock Minigame ended by player.");
    }
}
