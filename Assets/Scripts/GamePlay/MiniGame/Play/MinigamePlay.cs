using UnityEngine;

public class PlayMinigame : MonoBehaviour
{
    public static PlayMinigame Instance { get; private set; }

    [SerializeField] private int totalParts = 1; // ✅ ตอนนี้มีเพียงชิ้นเดียว
    private int completedParts = 0;

    [SerializeField] private float snapDistance = 0.5f;
    public Transform playShadowTransform; // ✅ เป้าหมายที่ต้อง Snap

    private bool isMinigameCompleted = false;

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

    public void CompleteMinigame()
    {
        completedParts++;
        if (completedParts >= totalParts)
        {
            isMinigameCompleted = true;
            Debug.Log("Minigame Completed! Press Spacebar to continue.");
            SceneTransitionManager.Instance.LoadScene("Story Scene");
        }
    }
}