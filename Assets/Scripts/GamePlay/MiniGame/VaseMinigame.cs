using UnityEngine;
using System.Collections;

public class VaseMinigame : MonoBehaviour
{
    public static VaseMinigame Instance { get; private set; }

    private int totalParts = 0;
    private int completedParts = 0;
    public bool isPlayingVaseMinigame = false;

    [SerializeField] private GameObject vaseminigame;
    [SerializeField] private DialogueSystem targetDialogueSystem;

    // ����Ѻ Animation
    [SerializeField] private Transform vaseTransform;
    [SerializeField] private float animationDuration = 1.0f;

    private Vector3 initialScale;
    private Vector3 initialPosition;

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

        if (vaseTransform != null)
        {
            initialScale = vaseTransform.localScale;
            initialPosition = vaseTransform.localPosition;
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

        if (completedParts >= totalParts)
        {
            StartCoroutine(HandleMinigameCompletion());
        }
    }

    private IEnumerator HandleMinigameCompletion()
    {
        isPlayingVaseMinigame = false;
        Debug.Log("Vase Minigame completed successfully! Starting fade-out animation...");

        // �� SpriteRenderer �������ͧ�ѵ������١�ͧ�ѹ
        SpriteRenderer[] spriteRenderers = vaseTransform.GetComponentsInChildren<SpriteRenderer>();

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            Debug.LogError("No SpriteRenderers found on vaseTransform or its children. Cannot fade out.");
            yield break;
        }

        float elapsedTime = 0f;

        // ����������鹢ͧ SpriteRenderer �ء���
        Color[] initialColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            initialColors[i] = spriteRenderers[i].color;
        }

        // ����� Animation ��èҧ���
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                // Lerp ��� Alpha �ͧ SpriteRenderer �ء���
                Color targetColor = new Color(initialColors[i].r, initialColors[i].g, initialColors[i].b, 0f);
                spriteRenderers[i].color = Color.Lerp(initialColors[i], targetColor, t);
            }

            yield return null; // ������Ѵ�
        }

        // ��駤����������ҷء SpriteRenderer �� Alpha = 0
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color finalColor = new Color(initialColors[i].r, initialColors[i].g, initialColors[i].b, 0f);
            spriteRenderers[i].color = finalColor;
        }

        Debug.Log("Vase and its children fade-out animation completed. Closing minigame.");

        // �Դ�Թ����ѹ����ѧ�ҡ Animation ����
        EndMinigame(true);
    }

    public void EndMinigame(bool isSuccess)
    {
        if (vaseminigame != null)
        {
            vaseminigame.SetActive(false);
        }

        if (isSuccess)
        {
            Debug.Log("Vase Minigame completed successfully!");

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

    public void ResetMinigame()
    {
        if (vaseTransform != null)
        {
            vaseTransform.localScale = initialScale;
            vaseTransform.localPosition = initialPosition;
        }
        completedParts = 0;
        Debug.Log("Vase Minigame reset.");
    }
}
