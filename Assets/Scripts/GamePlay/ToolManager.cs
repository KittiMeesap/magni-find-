using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    public string CurrentMode { get; private set; } = "Hand"; // โหมดเริ่มต้น
    public Texture2D handCursor;
    public Texture2D magnifierCursor;

    private GameObject selectedObject;
    private CollectibleItem[] collectibleItems; // เก็บ CollectibleItems ที่ส่งมาจาก GameController

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
        if (CurrentMode == "Hand" && Input.GetMouseButtonDown(0)) // ใช้มือเลือกวัตถุ
        {
            SelectObject();
        }
    }

    public void Initialize(CollectibleItem[] items)
    {
        collectibleItems = items; // เก็บ CollectibleItems ที่ได้รับ
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
                ChangeObjectColor(selectedObject, Color.white); // รีเซ็ตสีวัตถุเดิม
            }

            selectedObject = hit.collider.gameObject; // กำหนดวัตถุที่เลือก
            ChangeObjectColor(selectedObject, Color.yellow); // เปลี่ยนสีวัตถุที่เลือก
            Debug.Log($"Selected: {selectedObject.name}");
        }
        else
        {
            Debug.LogWarning("No object selected!");
        }
    }

    public GameObject GetSelectedObject()
    {
        return selectedObject; // คืนค่าวัตถุที่ถูกเลือก
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
