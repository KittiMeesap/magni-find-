using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    public string CurrentMode { get; private set; } = "Hand";
    public Texture2D handCursor;
    public Texture2D magnifierCursor;
    public Texture2D eyeCursor;

    private GameObject selectedObject;
    private CollectibleItem[] collectibleItems;

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
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            ToggleToolMode();
        }

        if (CurrentMode == "Hand" && Input.GetMouseButtonDown(0))
        {
            SelectObject();
        }

        if (CurrentMode == "Eye" && Input.GetMouseButtonDown(0))
        {
            InspectObject();
        }
    }

    public void Initialize(CollectibleItem[] items)
    {
        collectibleItems = items;
        Debug.Log("ToolManager initialized with collectible items.");
    }

    public void SetToolMode(string mode)
    {
        CurrentMode = mode;
        if (mode == "Hand")
        {
            Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (mode == "Magnifier")
        {
            Cursor.SetCursor(magnifierCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (mode == "Eye")
        {
            Cursor.SetCursor(eyeCursor, Vector2.zero, CursorMode.Auto);
        }
        Debug.Log($"Tool mode set to: {mode}");
    }

    private void ToggleToolMode()
    {
        if (CurrentMode == "Hand")
        {
            SetToolMode("Magnifier");
        }
        else if (CurrentMode == "Magnifier")
        {
            SetToolMode("Eye");
        }
        else
        {
            SetToolMode("Hand");
        }
    }

    public void SelectObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            if (selectedObject != null)
            {
                ChangeObjectColor(selectedObject, Color.white);
            }

            selectedObject = hit.collider.gameObject;
            ChangeObjectColor(selectedObject, Color.yellow);
            Debug.Log($"Selected: {selectedObject.name}");
        }
        else
        {
            Debug.LogWarning("No object selected!");
        }
    }

    private void InspectObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            DialogueSystem dialogueSystem = hit.collider.GetComponent<DialogueSystem>();
            if (dialogueSystem != null)
            {
                dialogueSystem.ShowDialogue();
            }
            else
            {
                Debug.LogWarning("No DialogueSystem attached to the object.");
            }
        }
        else
        {
            Debug.LogWarning("No object to inspect!");
        }
    }

    public GameObject GetSelectedObject()
    {
        return selectedObject;
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
