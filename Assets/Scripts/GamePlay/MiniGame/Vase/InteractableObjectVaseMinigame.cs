using UnityEngine;
using System.Collections;

public class InteractableObjectVaseMinigame : MonoBehaviour
{
    public GameObject targetObject; // วัตถุเป้าหมาย (ที่ต้องประกอบเข้าด้วยกัน)
    public float snapDistance = 0.5f; // ระยะที่ดึงวัตถุเข้าไป
    public Vector3 correctScale; // ขนาดที่ถูกต้อง
    public float scaleTolerance = 0.1f; // ความคลาดเคลื่อนของขนาดที่ยอมรับได้
    public float scaleStep = 0.1f; // ค่าการเพิ่ม/ลดของการขยาย
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // ขนาดต่ำสุด
    public Vector3 maxScale = new Vector3(3f, 3f, 3f); // ขนาดสูงสุด
    public float snapYOffset = 0.0f; // Offset แกน Y สำหรับตำแหน่ง Snap
    public float scaleDuration = 0.1f; // ✅ เวลาที่ใช้ในการย่อขยาย (เพิ่มความสมูท)

    private bool isDragging = false;
    private bool isSnapped = false; // เพิ่มตัวแปรสถานะเพื่อบล็อกการ drag เมื่อ snap แล้ว
    private bool isScaling = false; // ✅ ป้องกันการเรียกซ้ำขณะกำลังปรับขนาด

    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;

    private SpriteRenderer FrogTargetspriteRenderer;
    [SerializeField] private GameObject FrogTarget;
    private Sprite FrogTargetdefaultSprite;
    [SerializeField] private Sprite FrogTargethighlightedSprite;

    private void Awake()
    {
        if (FrogTarget != null)
        {
            FrogTargetspriteRenderer = FrogTarget.GetComponent<SpriteRenderer>();
            if (FrogTargetspriteRenderer != null)
            {
                FrogTargetdefaultSprite = FrogTargetspriteRenderer.sprite;
            }
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
    }

    void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging && !isSnapped)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;

            if (targetObject != null && IsScaleCorrect() && IsWithinSnapDistance())
            {
                SnapToTarget();
            }
        }

        if (IsScaleCorrect())
        {
            if (FrogTargetspriteRenderer != null)
            {
                FrogTargetspriteRenderer.sprite = FrogTargethighlightedSprite;
            }
        }
        else
        {
            if (FrogTargetspriteRenderer != null)
            {
                FrogTargetspriteRenderer.sprite = FrogTargetdefaultSprite;
            }
        }
    }

    private void OnMouseDown()
    {
        if (!isSnapped)
        {
            if (ToolManager.Instance.CurrentMode == "Hand")
            {
                isDragging = true;
            }
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void OnMouseOver()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped && !isScaling)
        {
            if (Input.GetMouseButtonDown(0)) // คลิกซ้ายเพื่อขยาย
            {
                StartCoroutine(ModifyScaleSmoothly(scaleStep));
            }
            else if (Input.GetMouseButtonDown(1)) // คลิกขวาเพื่อลดขนาด
            {
                StartCoroutine(ModifyScaleSmoothly(-scaleStep));
            }
        }

        if (MinigameManager.Instance.IsPlayingMinigame && (ToolManager.Instance.CurrentMode == "Hand" || ToolManager.Instance.CurrentMode == "Magnifier"))
        {
            if (!isSnapped)
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
    }

    private IEnumerator ModifyScaleSmoothly(float scaleChange)
    {
        isScaling = true;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale + Vector3.one * scaleChange;

        targetScale.x = Mathf.Clamp(targetScale.x, minScale.x, maxScale.x);
        targetScale.y = Mathf.Clamp(targetScale.y, minScale.y, maxScale.y);
        targetScale.z = Mathf.Clamp(targetScale.z, minScale.z, maxScale.z);

        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        isScaling = false;
    }

    private bool IsScaleCorrect()
    {
        return Mathf.Approximately(transform.localScale.x, correctScale.x) &&
               Mathf.Approximately(transform.localScale.y, correctScale.y) &&
               Mathf.Approximately(transform.localScale.z, correctScale.z);
    }

    private bool IsWithinSnapDistance()
    {
        return Vector3.Distance(transform.position, targetObject.transform.position) <= snapDistance;
    }

    private void SnapToTarget()
    {
        Vector3 snapPosition = targetObject.transform.position;
        snapPosition.y += snapYOffset;

        transform.position = snapPosition;

        if (IsScaleCorrect())
        {
            Debug.Log($"Snapped to target {targetObject.name} successfully!");
            isSnapped = true;
            VaseMinigame.Instance.CompletePart();
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
