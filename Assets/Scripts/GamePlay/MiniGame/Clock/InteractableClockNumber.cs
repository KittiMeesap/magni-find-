using SpriteGlow;
using UnityEngine;
using System.Collections;

public class InteractableClockNumber : MonoBehaviour
{
    public GameObject targetPosition;
    public GameObject dragBoundsObject;
    public Vector3 correctScale;
    public float scaleTolerance = 0.05f;
    public float snapDistance = 0.5f;
    public float scaleDuration = 0.15f; // ✅ เวลาที่ใช้ในการย่อขยาย (เพิ่มความสมูท)

    public bool isSnapped = false;
    private bool isDragging = false;
    private bool isScaling = false; // ✅ ป้องกันการกดซ้ำขณะกำลังปรับขนาด
    private static InteractableClockNumber selectedNumber;
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;

    private int originalSortingOrder;
    private Bounds dragBounds;
    private Vector3 dragCenter;
    private float dragRadius;

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

        originalSortingOrder = spriteRenderer.sortingOrder;

        if (dragBoundsObject != null)
        {
            GetObjectBounds(dragBoundsObject);
        }
    }

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging && !isSnapped)
        {
            DragObject();

            if (IsCloseToTarget() && IsScaleCorrect())
            {
                SnapToTarget();
            }
        }
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;

        if (ToolManager.Instance.CurrentMode == "Hand" && !isSnapped)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Hand);
            isDragging = true;
            selectedNumber = this;

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 999;
            }
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }

    private void DragObject()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (dragBoundsObject != null)
        {
            if (dragRadius > 0)
            {
                Vector2 direction = (mousePosition - (Vector2)dragCenter).normalized;
                float distance = Vector2.Distance(mousePosition, dragCenter);
                float clampedDistance = Mathf.Min(distance, dragRadius);
                transform.position = dragCenter + (Vector3)(direction * clampedDistance);
            }
            else
            {
                float clampedX = Mathf.Clamp(mousePosition.x, dragBounds.min.x, dragBounds.max.x);
                float clampedY = Mathf.Clamp(mousePosition.y, dragBounds.min.y, dragBounds.max.y);
                transform.position = new Vector2(clampedX, clampedY);
            }
        }
        else
        {
            transform.position = mousePosition;
        }
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;

        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped && !isScaling)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Upsize);
                StartCoroutine(ModifyScaleSmoothly(0.1f)); // ✅ ทำให้สมูท
            }
            else if (Input.GetMouseButtonDown(1))
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Smallsize);
                StartCoroutine(ModifyScaleSmoothly(-0.1f)); // ✅ ทำให้สมูท
            }
        }

        if (MinigameManager.Instance.IsPlayingMinigame && (ToolManager.Instance.CurrentMode == "Hand" || ToolManager.Instance.CurrentMode == "Magnifier"))
        {
            if (!isSnapped && spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite;
            }
        }
    }

    private IEnumerator ModifyScaleSmoothly(float scaleStep)
    {
        isScaling = true;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale + Vector3.one * scaleStep;

        targetScale.x = Mathf.Clamp(targetScale.x, 0.5f, 0.9f);
        targetScale.y = Mathf.Clamp(targetScale.y, 0.5f, 0.9f);
        targetScale.z = Mathf.Clamp(targetScale.z, 0.5f, 0.9f);

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

    private bool IsCloseToTarget()
    {
        float positionDistance = Vector3.Distance(transform.position, targetPosition.transform.position);
        return positionDistance <= snapDistance;
    }

    private bool IsScaleCorrect()
    {
        return Mathf.Approximately(transform.localScale.x, correctScale.x) &&
               Mathf.Approximately(transform.localScale.y, correctScale.y) &&
               Mathf.Approximately(transform.localScale.z, correctScale.z);
    }

    private void SnapToTarget()
    {
        if (!IsCloseToTarget() || !IsScaleCorrect())
        {
            Debug.LogWarning($"Cannot snap {gameObject.name} - Position or Scale incorrect.");
            return;
        }

        transform.position = targetPosition.transform.position;
        isSnapped = true;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder -= 1;
        }

        Debug.Log($"{gameObject.name} snapped to target successfully!");
        ClockMinigame.Instance.CompletePart();
    }

    private void GetObjectBounds(GameObject obj)
    {
        if (obj == null) return;

        CircleCollider2D circleCollider = obj.GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            dragCenter = obj.transform.position;
            dragRadius = circleCollider.radius * obj.transform.lossyScale.x;
            return;
        }

        BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            dragBounds = boxCollider.bounds;
            dragRadius = 0;
            return;
        }

        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            dragBounds = spriteRenderer.bounds;
            dragRadius = 0;
            return;
        }

        Debug.LogWarning($"{obj.name} has no valid Collider or SpriteRenderer! Using default bounds.");
    }

    private void OnMouseExit()
    {

        if (selectedNumber == this)
        {
            selectedNumber = null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}
