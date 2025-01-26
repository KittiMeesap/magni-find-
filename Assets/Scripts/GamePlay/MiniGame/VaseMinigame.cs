using UnityEngine;

public class VaseMinigame : MonoBehaviour
{
    public static VaseMinigame Instance { get; private set; }

    private int totalParts = 0; // �ӹǹ�����ǹ��������Թ���
    private int completedParts = 0; // �ӹǹ�����ǹ������������
    public bool isPlayingVaseMinigame = false;

    [SerializeField] private GameObject vaseminigame;

    // �������������Ѻ�� DialogueSystem �������Ǣ�ͧ
    [SerializeField] private DialogueSystem targetDialogueSystem;

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

    public void StartMinigame(int totalParts)
    {
        vaseminigame.SetActive(true);
        this.totalParts = totalParts;
        completedParts = 0;
        isPlayingVaseMinigame = true;
        Debug.Log($"Started Vase Minigame with {totalParts} parts.");
    }

    public void CompletePart()
    {
        completedParts++;
        Debug.Log($"Part completed: {completedParts}/{totalParts}");

        if (completedParts >= totalParts) // �������稤ú�ء���
        {
            EndMinigame(true); // ���Թ��������
        }
    }

    public void EndMinigame(bool isSuccess)
    {
        if (isPlayingVaseMinigame)
        {
            vaseminigame.SetActive(false);
            isPlayingVaseMinigame = false;

            if (isSuccess)
            {
                Debug.Log("Vase Minigame completed successfully!");

                // ��ҵ�� hasMagnifierMessage �͡�ҡ DialogueSystem �������
                if (targetDialogueSystem != null)
                {
                    targetDialogueSystem.hasMagnifierMessage = false;
                    Debug.Log($"Set hasMagnifierMessage to false for {targetDialogueSystem.gameObject.name}");
                }
                else
                {
                    Debug.LogWarning("No target DialogueSystem assigned!");
                }
            }
            else
            {
                Debug.Log("Vase Minigame failed.");
            }
        }
    }
}
