using UnityEngine;

public class RewardItemClock : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Reward collected. Triggering ClockMinigame to close.");
        ClockMinigame.Instance.OnRewardCollected(); // เรียกฟังก์ชันเพื่อเลื่อนนาฬิกาและปิดมินิเกม
    }
}
