using UnityEngine;

public class InteractableClockNumber : MonoBehaviour
{
    public GameObject targetPosition; // ���˹��������
    public Vector3 correctScale; // ��Ҵ���١��ͧ
    public float scaleTolerance = 0.05f; // ���������״������硹�������Ѻ��õ�Ǩ�ͺ��Ҵ
    public float snapDistance = 0.5f; // ���� Snap

    public Material defaultMaterial; // ��ʴػ���
    public Material draggingMaterial; // ��ʴ������ҧ����ҡ
    public Material magnifierMaterial; // ��ʴ��������͢���

    private bool isSnapped = false;
    private bool isDragging = false;
    private static InteractableClockNumber selectedNumber; // ����Ţ�����ѧ�١���͡
    private SpriteRenderer spriteRenderer; // Sprite Renderer �ͧ Object

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name} is missing a SpriteRenderer!");
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

        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped && selectedNumber == this)
        {
            HandleScaling();
        }
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && !isSnapped)
        {
            isDragging = true;
            selectedNumber = this; // ��駤�ҵ���Ţ���١���͡

            // ����¹��ʴ�����ʴ��ҡ
            if (spriteRenderer != null && draggingMaterial != null)
            {
                spriteRenderer.material = draggingMaterial;
            }
        }
        else if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped)
        {
            selectedNumber = this; // ��駤�ҵ���Ţ���١���͡����͡������ Magnifier
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // ����¹��ʴء�Ѻ����ʴػ���
        if (spriteRenderer != null && defaultMaterial != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

    private void DragObject()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    private void HandleScaling()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ��ԡ���� (����)
            if (spriteRenderer != null && magnifierMaterial != null)
            {
                spriteRenderer.material = magnifierMaterial; // ����¹����ʴ�����Ѻ��͢���
            }
            ModifyScale(0.1f);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // ��ԡ��� (���)
            if (spriteRenderer != null && magnifierMaterial != null)
            {
                spriteRenderer.material = magnifierMaterial; // ����¹����ʴ�����Ѻ��͢���
            }
            ModifyScale(-0.1f);
        }

        // ����ͻ���»�����ԡ �������¹��ʴء�Ѻ�� Default
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            if (spriteRenderer != null && defaultMaterial != null)
            {
                spriteRenderer.material = defaultMaterial;
            }
        }
    }


    private void ModifyScale(float scaleStep)
    {
        // ��鹵͹�ͧ scale (��˹� step size �� 0.1)
        float stepSize = 0.1f;

        // �ӹǳ������������᡹�¡�� snap ��� step size
        float newX = Mathf.Round((transform.localScale.x + scaleStep) / stepSize) * stepSize;
        float newY = Mathf.Round((transform.localScale.y + scaleStep) / stepSize) * stepSize;
        float newZ = Mathf.Round((transform.localScale.z + scaleStep) / stepSize) * stepSize;

        // Clamp ��� scale �������㹪�ǧ����˹� (0.5 - 0.9)
        newX = Mathf.Clamp(newX, 0.5f, 0.9f);
        newY = Mathf.Clamp(newY, 0.5f, 0.9f);
        newZ = Mathf.Clamp(newZ, 0.5f, 0.9f);

        // ��駤�� scale ����
        transform.localScale = new Vector3(newX, newY, newZ);

        Debug.Log($"Modified Scale: {transform.localScale}");
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

        // �Դ��÷ӧҹ�ͧ Collider ����� Snap �����
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // ��Ѻ Order in Layer Ŵŧ 1
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder -= 1;
        }

        Debug.Log($"{gameObject.name} snapped to target successfully!");
        ClockMinigame.Instance.CompletePart();
    }
}
