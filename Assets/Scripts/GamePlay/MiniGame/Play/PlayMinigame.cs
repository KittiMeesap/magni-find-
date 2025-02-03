using UnityEngine;
using System.Collections;

public class PlayMinigame : MonoBehaviour
{
    public static PlayMinigame Instance { get; private set; }

    private int totalParts = 4;
    private int completedParts = 0;
    private int savedCompletedParts = 0;

    [SerializeField] private Transform playTransform;
    [SerializeField] private float snapDistance = 0.5f;
    [SerializeField] private Vector3 correctScale;

    [SerializeField] private Transform pTransform;
    [SerializeField] private Transform lTransform;
    [SerializeField] private Transform aTransform;
    [SerializeField] private Transform yTransform;

    [SerializeField] private Transform pShadowTransform;
    [SerializeField] private Transform lShadowTransform;
    [SerializeField] private Transform aShadowTransform;
    [SerializeField] private Transform yShadowTransform;

    private bool isMinigameCompleted = false;
    private Transform draggingLetter = null;

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

    private void Update()
    {
        HandleDragging();

        if (isMinigameCompleted && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Starting the next level...");
            MainMenuManager.Instance.LoadNextLevel(); // เรียกใช้ MainMenuManager ให้โหลดด่านต่อไป
        }
    }

    private Transform GetShadowForLetter(DraggableTextUI letter)
    {
        if (letter.name.StartsWith("P")) return pShadowTransform;
        if (letter.name.StartsWith("L")) return lShadowTransform;
        if (letter.name.StartsWith("A")) return aShadowTransform;
        if (letter.name.StartsWith("Y")) return yShadowTransform;

        return null; // ถ้าไม่มีตัวอักษรที่ตรงกัน
    }


    private void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ตรวจสอบว่าคลิกโดนตัวอักษรหรือไม่
            draggingLetter = GetLetterUnderMouse();
        }
        else if (Input.GetMouseButton(0) && draggingLetter != null)
        {
            // ลากตัวอักษร
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggingLetter.position = mousePos;
        }
        else if (Input.GetMouseButtonUp(0) && draggingLetter != null)
        {
            // ปล่อยเมาส์ → ตรวจสอบการ Snap
            CheckForSnap(draggingLetter);
            draggingLetter = null;
        }
    }

    private Transform GetLetterUnderMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null)
        {
            if (hit.transform == pTransform || hit.transform == lTransform || hit.transform == aTransform || hit.transform == yTransform)
            {
                return hit.transform;
            }
        }

        return null;
    }

    private void CheckForSnap(Transform letter)
    {
        Transform shadowTarget = null;

        if (letter == pTransform) shadowTarget = pShadowTransform;
        else if (letter == lTransform) shadowTarget = lShadowTransform;
        else if (letter == aTransform) shadowTarget = aShadowTransform;
        else if (letter == yTransform) shadowTarget = yShadowTransform;

        if (shadowTarget != null)
        {
            if (Vector3.Distance(letter.position, shadowTarget.position) <= snapDistance && letter.localScale == correctScale)
            {
                letter.position = shadowTarget.position; // Snap เข้าตำแหน่งเงา
                CompletePart();
            }
        }
    }

    public void CheckLetterPlacement(DraggableTextUI letter)
    {
        // ตรวจสอบว่าตัวอักษรอยู่ใกล้เงาของมันหรือไม่
        float snapDistance = 50f; // กำหนดระยะที่ถือว่า "วางถูกต้อง"
        Transform shadowTransform = GetShadowForLetter(letter);

        if (shadowTransform != null && Vector2.Distance(letter.transform.position, shadowTransform.position) <= snapDistance)
        {
            letter.transform.position = shadowTransform.position; // Snap เข้าที่
            CompletePart(); // นับว่าเสร็จไปหนึ่งตัว
        }
    }

    public void CompletePart()
    {
        completedParts++;
        savedCompletedParts = completedParts;
        Debug.Log($"Play Part completed: {completedParts}/{totalParts}");

        if (completedParts >= totalParts)
        {
            isMinigameCompleted = true;
            Debug.Log("All letters placed correctly. Press Spacebar to continue.");
            playTransform.gameObject.SetActive(true);
        }
    }
}
