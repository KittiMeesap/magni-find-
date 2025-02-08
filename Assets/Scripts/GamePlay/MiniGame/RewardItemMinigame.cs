using UnityEngine;

public class RewardItemMinigame : MonoBehaviour
{
    [SerializeField] private RewardMinigameType minigameType;

    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;

        Debug.Log($"Reward collected from {minigameType}. Closing minigame...");
        //MinigameManager.Instance.OnRewardCollected();
    }
}

public enum RewardMinigameType
{
    Clock,
    Vase
}
