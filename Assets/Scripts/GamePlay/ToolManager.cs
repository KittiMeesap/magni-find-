using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    public string CurrentMode { get; private set; } = "Hand"; // �����������
    public Texture2D handCursor;
    public Texture2D magnifierCursor;

    private GameObject selectedObject;
    private CollectibleItem[] collectibleItems; // �� CollectibleItems ������Ҩҡ GameController

    private void Awake()
    {
        SetToolMode("Hand");
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
        if (CurrentMode == "Hand" && Input.GetMouseButtonDown(0)) // ��������͡�ѵ��
        {
            SelectObject();
        }
    }

    public void Initialize(CollectibleItem[] items)
    {
        collectibleItems = items; // �� CollectibleItems ������Ѻ
        Debug.Log("ToolManager initialized with collectible items.");
    }

    public void SetToolMode(string mode)
    {
        CurrentMode = mode;
        Cursor.SetCursor(mode == "Hand" ? handCursor : magnifierCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SelectObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            if (selectedObject != null)
            {
                ChangeObjectColor(selectedObject, Color.white); // �������ѵ�����
            }

            selectedObject = hit.collider.gameObject; // ��˹��ѵ�ط�����͡
            ChangeObjectColor(selectedObject, Color.yellow); // ����¹���ѵ�ط�����͡
            Debug.Log($"Selected: {selectedObject.name}");
        }
        else
        {
            Debug.LogWarning("No object selected!");
        }
    }

    public GameObject GetSelectedObject()
    {
        return selectedObject; // �׹����ѵ�ط��١���͡
    }

    private void ChangeObjectColor(GameObject obj, Color color)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }
}
