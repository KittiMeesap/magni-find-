using SpriteGlow;
using UnityEngine;
using System.Collections;

public class InteractableClockHand : MonoBehaviour
{
    [System.Serializable]
    public struct ClockPosition
    {
        public Vector3 position; // ตำแหน่งของเข็ม
        public Vector3 rotation; // การหมุนของเข็ม
    }

    public ClockPosition[] positions; // ตำแหน่งและการหมุนทั้งหมด (1-12)
    public int correctPositionIndex; // ตำแหน่งที่ถูกต้อง (0-11)
    public int initialPositionIndex = 0; // ตำแหน่งเริ่มต้นของเข็ม

    private int currentPositionIndex; // ตำแหน่งปัจจุบัน
    public bool isSnapped = false;
    private bool isMoving = false; // ✅ ป้องกันการกดซ้ำตอนกำลังเคลื่อนที่

    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;

    [SerializeField] private float moveSpeed = 5f; // ✅ ความเร็วของการเลื่อนเข็ม

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
        else
        {
            Debug.LogError($"{gameObject.name} is missing a SpriteRenderer!");
        }
    }

    private void Start()
    {
        // ตั้งค่าตำแหน่งเริ่มต้น
        currentPositionIndex = initialPositionIndex;
        transform.localPosition = positions[currentPositionIndex].position;
        transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;

        if (ClockMinigame.Instance != null && ClockMinigame.Instance.CompletedParts >= ClockMinigame.Instance.TotalParts)
        {
            return;
        }

        if (MinigameManager.Instance.IsPlayingMinigame && ToolManager.Instance.CurrentMode == "Hand")
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite;
            }

            if (!isMoving) // ✅ ป้องกันการกดซ้ำ
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Hand);
                    HandleLeftClick();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Hand);
                    HandleRightClick();
                }
            }
        }
        else
        {
                spriteRenderer.sprite = defaultSprite;
        }
    }

    private void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    private void HandleLeftClick()
    {
        if (isSnapped)
        {
            isSnapped = false;
            ClockMinigame.Instance.DecreaseCompletedParts();
        }

        AdvancePosition();
    }

    private void HandleRightClick()
    {
        if (isSnapped)
        {
            isSnapped = false;
            ClockMinigame.Instance.DecreaseCompletedParts();
        }

        ReversePosition();
    }

    private void AdvancePosition()
    {
        if (isMoving) return;
        isMoving = true;

        int newIndex = (currentPositionIndex + 1) % positions.Length;
        StartCoroutine(MoveClockHandCoroutine(newIndex));
    }

    private void ReversePosition()
    {
        if (isMoving) return;
        isMoving = true;

        int newIndex = (currentPositionIndex - 1 + positions.Length) % positions.Length;
        StartCoroutine(MoveClockHandCoroutine(newIndex));
    }

    private IEnumerator MoveClockHandCoroutine(int newIndex)
    {
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        Vector3 targetPos = positions[newIndex].position;
        Quaternion targetRot = Quaternion.Euler(positions[newIndex].rotation);

        float elapsedTime = 0f;
        float duration = 1f / moveSpeed; // ✅ คำนวณเวลาตาม `moveSpeed`

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            transform.localRotation = Quaternion.Lerp(startRot, targetRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPos;
        transform.localRotation = targetRot;
        currentPositionIndex = newIndex;

        isMoving = false; // ✅ ปลดล็อกให้กดใหม่ได้

        Debug.Log($"{gameObject.name} moved to position {currentPositionIndex + 1}");

        if (currentPositionIndex == correctPositionIndex)
        {
            SnapToTarget();
        }
    }

    private void SnapToTarget()
    {
        isSnapped = true;
        Debug.Log($"{gameObject.name} snapped to correct position {correctPositionIndex + 1}!");
        ClockMinigame.Instance.CompletePart();
    }
}
