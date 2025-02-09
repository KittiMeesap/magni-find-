using UnityEngine;
using System.Collections;

public class InteractableBattery : MonoBehaviour
{
    [SerializeField] private Transform targetPosition; // ✅ จุดที่ถ่านจะขยับไปเมื่อขนาดถูกต้อง
    [SerializeField] private float moveSpeed = 2f; // ✅ ความเร็วในการเคลื่อนที่
    [SerializeField] private float scaleSpeed = 3f; // ✅ ความเร็วในการขยาย
    private bool isMoving = false; // ✅ เช็คว่ากำลังเคลื่อนที่หรือไม่
    private bool isScaling = false; // ✅ เช็คว่ากำลังขยายหรือไม่
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;

    private Vector3 targetScale; // ✅ ขนาดเป้าหมายที่ต้องการขยาย
    private bool shouldMoveToTarget = false; // ✅ ตรวจสอบว่าต้องเคลื่อนที่หรือไม่

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
        targetScale = transform.localScale; // ✅ กำหนดขนาดเริ่มต้น
    }

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isMoving)
        {
            HandleScaling();
        }

        if (shouldMoveToTarget && !isMoving)
        {
            StartCoroutine(MoveToTarget());
            shouldMoveToTarget = false;
        }
    }

    private void HandleScaling()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Upsize);
            StartCoroutine(SmoothScale(0.1f));
        }
        else if (Input.GetMouseButtonDown(1))
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Smallsize);
            StartCoroutine(SmoothScale(-0.1f));
        }
    }

    private IEnumerator SmoothScale(float scaleStep)
    {
        if (isScaling) yield break; // ✅ ป้องกันการเรียกซ้ำ
        isScaling = true;

        Vector3 startScale = transform.localScale;
        targetScale += Vector3.one * scaleStep;
        targetScale.x = Mathf.Clamp(targetScale.x, 0.5f, 1f);
        targetScale.y = Mathf.Clamp(targetScale.y, 0.5f, 1f);
        targetScale.z = Mathf.Clamp(targetScale.z, 0.5f, 1f);

        float elapsedTime = 0f;
        while (elapsedTime < 1f / scaleSpeed)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime * scaleSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        isScaling = false;

        // ✅ เมื่อขนาดถูกต้อง ให้เล่น Animation ขยับไปที่เป้าหมาย
        if (IsCorrectSize() && !isMoving)
        {
            shouldMoveToTarget = true;
        }
    }

    private bool IsCorrectSize()
    {
        return Mathf.Approximately(transform.localScale.x, 1f) &&
               Mathf.Approximately(transform.localScale.y, 1f) &&
               Mathf.Approximately(transform.localScale.z, 1f);
    }

    private IEnumerator MoveToTarget()
    {
        DialogueUI.Instance.DialogueButton(false);
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        float duration = 1f; // ✅ ระยะเวลาการเคลื่อนที่

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        transform.position = targetPosition.position; // ✅ แก้ไขตำแหน่งให้ตรงจุด
        isMoving = false;

        // ✅ เมื่อเคลื่อนที่ถึงตำแหน่งที่ตั้งไว้ ให้แจ้งว่าใส่ถ่านเสร็จ
        CameraMinigame.Instance.InsertNewBattery();
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;
        if (MinigameManager.Instance.IsPlayingMinigame && (ToolManager.Instance.CurrentMode == "Magnifier") && !isMoving && CameraMinigame.Instance.HasBatteryInserted == false)
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite;
            }
        }
        else
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }

    private void OnMouseExit()
    {

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}
