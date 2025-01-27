using UnityEngine;

public class InteractableObjectVaseMinigame : MonoBehaviour
{
    public GameObject targetObject; // �ѵ��������� (����ͧ��Сͺ��Ҵ��¡ѹ)
    public float snapDistance = 0.5f; // ���з��֧�ѵ������
    public Vector3 correctScale; // ��Ҵ���١��ͧ
    public float scaleTolerance = 0.1f; // ������Ҵ����͹�ͧ��Ҵ�������Ѻ��
    public float scaleStep = 0.1f; // ��ҡ������/Ŵ�ͧ��â���
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // ��Ҵ����ش
    public Vector3 maxScale = new Vector3(3f, 3f, 3f); // ��Ҵ�٧�ش
    public float snapYOffset = 0.0f; // Offset ᡹ Y ����Ѻ���˹� Snap

    private bool isDragging = false;
    private bool isSnapped = false; // ���������ʶҹ����ͺ��͡��� drag ����� snap ����

    [SerializeField] private Material defaultMaterial; // ��ʴ��������
    [SerializeField] private Material highlightMaterial; // ��ʴ������ Drag ���� Resize


    void Update()
    {
        // Handle dragging in Hand mode
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging && !isSnapped)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;

            // ��Ǩ�ͺ���͹䢡�͹ Snap
            if (targetObject != null && IsScaleCorrect() && IsWithinSnapDistance())
            {
                SnapToTarget(); // Snap ������������
            }
        }

        // Handle resizing in Magnify mode
        if (ToolManager.Instance.CurrentMode == "Magnifier" && !isSnapped)
        {
            HandleMagnify();
        }
    }

    private void OnMouseDown()
    {
        if (!isSnapped) // ��� snap ���Ǩ��������ö drag ��
        {
            if (ToolManager.Instance.CurrentMode == "Hand")
            {
                // Start dragging only if clicked on this object
                isDragging = true;

                // ����¹��ʴ��� highlightMaterial ����� Drag �������
                ChangeMaterial(highlightMaterial);
            }
            else if (ToolManager.Instance.CurrentMode == "Magnifier")
            {
                // Set this object as the target for magnifying
                ToolManager.Instance.SetSelectedObject(gameObject);

                // ����¹��ʴ��� highlightMaterial ��������͡ Resize
                ChangeMaterial(highlightMaterial);
            }
        }
    }


    private void OnMouseUp()
    {
        isDragging = false;

        // ����¹��ʴء�Ѻ�� defaultMaterial ����ͻ�����ѵ��
        ChangeMaterial(defaultMaterial);
    }


    private void HandleMagnify()
    {
        GameObject selectedObject = ToolManager.Instance.GetSelectedObject();
        if (selectedObject == gameObject && !isSnapped) // ��Ǩ�ͺ��� snap ���������ѧ
        {
            if (Input.GetMouseButtonDown(0)) // ��ԡ�������͢���
            {
                ModifyScale(scaleStep);
            }
            else if (Input.GetMouseButtonDown(1)) // ��ԡ�������Ŵ��Ҵ
            {
                ModifyScale(-scaleStep);
            }
        }
    }

    private void ModifyScale(float scaleChange)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

        // Clamp scale within min and max
        newScale.x = Mathf.Clamp(newScale.x, minScale.x, maxScale.x);
        newScale.y = Mathf.Clamp(newScale.y, minScale.y, maxScale.y);
        newScale.z = Mathf.Clamp(newScale.z, minScale.z, maxScale.z);

        transform.localScale = newScale;

        // ����¹��ʴ��� highlightMaterial �����ҧ��� Resize
        ChangeMaterial(highlightMaterial);

        Debug.Log($"Modified scale of {gameObject.name} to {newScale}");
    }

    private void ChangeMaterial(Material newMaterial)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && newMaterial != null)
        {
            spriteRenderer.material = newMaterial;
        }
    }



    private bool IsScaleCorrect()
    {
        // ��Ǩ�ͺ��� Scale �ͧ�ѵ����ҡѺ Scale ����˹����ء᡹
        return transform.localScale == correctScale;
    }

    private bool IsWithinSnapDistance()
    {
        // ��Ǩ�ͺ���������ҧ�����ҧ�ѵ������������ <= snapDistance
        return Vector3.Distance(transform.position, targetObject.transform.position) <= snapDistance;
    }

    private void SnapToTarget()
    {
        // �ӹǳ���˹� Snap ���� Offset ᡹ Y
        Vector3 snapPosition = targetObject.transform.position;
        snapPosition.y += snapYOffset;

        // Snap ��ѧ���˹��������
        transform.position = snapPosition;

        // ��Ǩ�ͺ����ա�û�Сͺ��Т�Ҵ�١��ͧ�������
        if (IsScaleCorrect())
        {
            Debug.Log($"Snapped to target {targetObject.name} successfully!");
            isSnapped = true; // ��駤�� isSnapped �� true ���ͺ��͡��� drag
            VaseMinigame.Instance.CompletePart(); // ����Ҫ�鹹�����������
        }
    }

}
