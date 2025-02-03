using UnityEngine;
using System.Collections;

public class PlayMinigame : MonoBehaviour
{
    public static PlayMinigame Instance { get; private set; }

    private int totalParts = 4;
    private int completedParts = 0;
    private int savedCompletedParts = 0;

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject playMinigameObject;
    [SerializeField] private Transform playTransform;
    [SerializeField] private float animationDuration = 1f;

    [SerializeField] private Transform pTransform;
    [SerializeField] private Transform lTransform;
    [SerializeField] private Transform aTransform;
    [SerializeField] private Transform yTransform;

    [SerializeField] private Transform pShadowTransform;
    [SerializeField] private Transform lShadowTransform;
    [SerializeField] private Transform aShadowTransform;
    [SerializeField] private Transform yShadowTransform;

    private Vector3 pInitialPosition;
    private Vector3 lInitialPosition;
    private Vector3 aInitialPosition;
    private Vector3 yInitialPosition;

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

        // เก็บค่าตำแหน่งเริ่มต้น
        if (pTransform != null) pInitialPosition = pTransform.localPosition;
        if (lTransform != null) lInitialPosition = lTransform.localPosition;
        if (aTransform != null) aInitialPosition = aTransform.localPosition;
        if (yTransform != null) yInitialPosition = yTransform.localPosition;
    }

    public void StartMinigame()
    {
        completedParts = savedCompletedParts; // โหลดค่าที่ทำสำเร็จแล้ว

        // ใช้ MainMenuManager แทน MinigameManager
        MainMenuManager.Instance.StartMinigame();

        // กำหนดค่า trigger
        MainMenuManager.Instance.SetMinigameTrigger(triggerObject);
    }

    public void CompletePart()
    {
        completedParts++;
        savedCompletedParts = completedParts;

        Debug.Log($"Play Part completed: {completedParts}/{totalParts}");

        if (completedParts >= totalParts)
        {
            isMinigameCompleted = true;
            StartCoroutine(HandleMinigameCompletion());
        }
    }

    private IEnumerator HandleMinigameCompletion()
    {
        Debug.Log("Play Minigame completed successfully! Starting animation...");

        // ย้ายตัวอักษร P, L, A, Y ไปที่ตำแหน่งที่ถูกต้องของเงา
        Vector3[] targetPositions = { pShadowTransform.position, lShadowTransform.position, aShadowTransform.position, yShadowTransform.position };
        Transform[] letterTransforms = { pTransform, lTransform, aTransform, yTransform };

        for (int i = 0; i < letterTransforms.Length; i++)
        {
            yield return StartCoroutine(MoveLetterToShadow(letterTransforms[i], targetPositions[i]));
        }

        Debug.Log("All letters placed correctly. Allowing player to press the PLAY button.");

        // หลังจากที่ตัวอักษรอยู่ในตำแหน่งที่ถูกต้องแล้ว สามารถกดปุ่ม PLAY เพื่อเริ่มเกม
        playTransform.gameObject.SetActive(true); // แสดงปุ่ม PLAY
    }

    private IEnumerator MoveLetterToShadow(Transform letter, Vector3 targetPosition)
    {
        Vector3 initialPosition = letter.position;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            letter.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        letter.position = targetPosition; // กำหนดตำแหน่งสุดท้าย
    }

    public void DecreaseCompletedParts()
    {
        if (completedParts > 0)
        {
            completedParts--;
            savedCompletedParts = completedParts;
            Debug.Log($"Play Part removed: {completedParts}/{totalParts}");
        }
    }
}
