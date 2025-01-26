using UnityEngine;
using System.Collections;

public class ClockMinigame : MonoBehaviour
{
    public static ClockMinigame Instance { get; private set; }

    [SerializeField] private GameObject clockMinigameObject; // GameObject �ͧ�Թ�������͹���㹫չ
    [SerializeField] private GameObject rewardItemPrefab; // Prefab �ͧ������ҧ���
    [SerializeField] private Transform clockTransform; // Transform �ͧ��ǹ��ԡ�
    [SerializeField] private GameObject clockStorageSprite; // Sprite ��ͧ�红ͧ������ٻ�Ҿ
    [SerializeField] private GameObject[] clockHands; // ������ԡ�
    [SerializeField] private GameObject[] clockNumbers; // ����Ţ���ԡ�
    [SerializeField] private float animationDuration = 1.0f; // �������Ңͧ Animation
    [SerializeField] private float rewardItemSlideDistance = 2.0f; // ���зҧ������������͹�͡��
    [SerializeField] private float clockStorageSlideDistance = 1.5f; // ���зҧ����ͧ�红ͧ����͹�͡��
    [SerializeField] private Vector3 clockTargetScale = new Vector3(0.8f, 0.8f, 0.8f); // ��Ҵ������¢ͧ���ԡ�
    [SerializeField] private Vector3 clockTargetPositionOffset = new Vector3(0, 2.0f, 0); // �������͹���˹��������
    [SerializeField] private int totalParts = 15; // 12 ����Ţ + 3 ���

    private Vector3 initialClockScale;
    private Vector3 initialClockPosition;
    private Vector3 initialStoragePosition;
    private Vector3[] initialHandsPositions;
    private Quaternion[] initialHandsRotations;
    private int[] initialHandsIndices; // �纤�� Initial Position Index �ͧ���
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

        // �纵��˹���Т�Ҵ�������
        initialClockScale = clockTransform.localScale;
        initialClockPosition = clockTransform.localPosition;

        if (clockStorageSprite != null)
        {
            initialStoragePosition = clockStorageSprite.transform.localPosition;
        }

        // �纵��˹���С����ع������鹢ͧ������ԡ�
        initialHandsPositions = new Vector3[clockHands.Length];
        initialHandsRotations = new Quaternion[clockHands.Length];
        initialHandsIndices = new int[clockHands.Length];
        for (int i = 0; i < clockHands.Length; i++)
        {
            initialHandsPositions[i] = clockHands[i].transform.localPosition;
            initialHandsRotations[i] = clockHands[i].transform.localRotation;

            // �纤�� Initial Position Index �ҡ InteractableClockHand
            var clockHandScript = clockHands[i].GetComponent<InteractableClockHand>();
            if (clockHandScript != null)
            {
                initialHandsIndices[i] = clockHandScript.initialPositionIndex;
            }
        }

        // �纵��˹���С����ع������鹢ͧ����Ţ���ԡ�
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
        // ��Ǩ�Ѻ��á� ESC ���ͻԴ�Թ���
        if (isPlayingClockMinigame && Input.GetKeyDown(KeyCode.Escape))
        {
            EndMinigameClick();
        }
    }

    public void StartMinigame()
    {
        if (clockMinigameObject != null)
        {
            // ���絵��˹���Т�Ҵ�ͧ���ԡ�
            clockTransform.localScale = initialClockScale;
            clockTransform.localPosition = initialClockPosition;

            if (clockStorageSprite != null)
            {
                clockStorageSprite.transform.localPosition = initialStoragePosition;
            }

            // ���絵��˹� �����ع ��Ф�� Initial Position Index �ͧ������ԡ�
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

            // ���絵��˹���С����ع�ͧ����Ţ���ԡ�
            for (int i = 0; i < clockNumbers.Length; i++)
            {
                clockNumbers[i].transform.localPosition = initialNumbersPositions[i];
                clockNumbers[i].transform.localRotation = initialNumbersRotations[i];
            }

            // �Դ��ҹ GameObject �ͧ�Թ���
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

        // ����� Animation �����͵���������͹���ԡ�
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
        // ����͹���ͧ�红ͧ�͡�Ҵ�ҹ��ҧ
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
        // ����͹������ҧ����͡�Ҵ�ҹ��ҧ
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
        // �֧ SpriteRenderer �������ͧ���ԡ�����١�
        SpriteRenderer[] spriteRenderers = clockTransform.GetComponentsInChildren<SpriteRenderer>();

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            Debug.LogError("No SpriteRenderers found on clockTransform or its children. Cannot fade out.");
            yield break;
        }

        float elapsedTime = 0f;

        // ����������鹢ͧ SpriteRenderer �ء���
        Color[] initialColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null) // ��Ǩ�ͺ��� SpriteRenderer ����� null
            {
                initialColors[i] = spriteRenderers[i].color;
            }
        }

        // �������èҧ���
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] != null) // ��Ǩ�ͺ�ա���駡�͹��Ѻ���
                {
                    // Lerp ��� Alpha �ͧ SpriteRenderer �ء���
                    Color targetColor = new Color(initialColors[i].r, initialColors[i].g, initialColors[i].b, 0f);
                    spriteRenderers[i].color = Color.Lerp(initialColors[i], targetColor, t);
                }
            }

            yield return null; // ������Ѵ�
        }

        // ��������� Alpha = 0 ����Ѻ�ء SpriteRenderer
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null) // ��Ǩ�ͺ�ա���駡�͹��駤��
            {
                Color finalColor = new Color(initialColors[i].r, initialColors[i].g, initialColors[i].b, 0f);
                spriteRenderers[i].color = finalColor;
            }
        }

        Debug.Log("Clock fade-out animation completed. Closing minigame.");

        // �Դ�Թ�����ѧ�ҡ Animation �������
        EndMinigame();
    }




    [SerializeField] private GameObject specificGameObject; // GameObject ����ͧ�Դ Collider2D

    public void EndMinigame()
    {
        isPlayingClockMinigame = false;

        // �Դ�����ҹ Collider2D � clockMinigameObject ����١�ͧ�ѹ
        if (clockMinigameObject != null)
        {
            Collider2D[] colliders = clockMinigameObject.GetComponentsInChildren<Collider2D>(true); // �� Collider2D ��������١�ͧ clockMinigameObject
            foreach (var collider in colliders)
            {
                collider.enabled = false; // �Դ Collider2D
            }

            clockMinigameObject.SetActive(false); // �Դ GameObject
            Debug.Log("All Collider2D in Clock Minigame have been disabled.");
        }

        // �Դ�����ҹ Collider2D � specificGameObject ����˹��ͧ
        if (specificGameObject != null)
        {
            Collider2D collider = specificGameObject.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false; // �Դ Collider2D
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
        clockMinigameObject.SetActive(false); // �Դ GameObject

        Debug.Log("Clock Minigame ended by player.");
    }
}
