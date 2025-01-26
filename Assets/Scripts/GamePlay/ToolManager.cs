using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    public string CurrentMode { get; private set; } = "Hand";
    public Texture2D handCursor;
    public Texture2D magnifierCursor;
    public Texture2D eyeCursor;

    private GameObject selectedObject;
    private CollectibleItem[] collectibleItems;

    public Material defaultMaterial;
    public Material outlineMaterial;

    public float zoomAmount = 0.1f;

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

        if (CurrentMode == "Eye")
        {

            GameObject targetObject = Instance.GetSelectedObject();

            if (targetObject != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    DialogueSystem dialogueSystem = targetObject.GetComponent<DialogueSystem>();
                    if (dialogueSystem != null)
                    {
                        dialogueSystem.ShowDialogue();
                    }
                    else
                    {
                        Debug.LogWarning("No DialogueSystem attached to the selected object.");
                    }

                    InteractableObject interactableObject = targetObject.GetComponent<InteractableObject>();

                    if (interactableObject.IsMiniGameObject == true && interactableObject.minigameNumber == 1)
                    {
                        VaseMinigame.Instance.StartMinigame(3);
                        SetToolMode("Hand");
                    }
                }
            }
            else
            {
                Debug.LogWarning("No object selected! Use Hand mode to select an object first.");
            }
        }

        if (CurrentMode == "Magnifier")
        {
            GameObject targetObject = Instance.GetSelectedObject();

            if (targetObject != null)
            {
                DialogueSystem dialogueSystem = targetObject.GetComponent<DialogueSystem>();

                if (Input.GetMouseButtonDown(0))
                {
                    if (dialogueSystem != null && dialogueSystem.hasMagnifierMessage)
                    {
                        dialogueSystem.ShowDialogue();
                    }
                    else
                    {
                        ModifyObjectScale(targetObject, zoomAmount);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (dialogueSystem != null && dialogueSystem.hasMagnifierMessage)
                    {
                        dialogueSystem.ShowDialogue();
                    }
                    else
                    {
                        ModifyObjectScale(targetObject, -zoomAmount);
                    }
                }
            }
        }
    }

    void ModifyObjectScale(GameObject target, float scaleChange)
    {
        InteractableObject breakable = target.GetComponent<InteractableObject>();
        if (breakable != null)
        {
            breakable.ModifyScale(scaleChange);
        }
        else
        {
            Debug.LogWarning("Selected object is not breakable.");
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
        VaseMinigame vaseMinigame = FindFirstObjectByType<VaseMinigame>();

        if (vaseMinigame.isPlayingVaseMinigame)
        {
            // เงื่อนไขเมื่อกำลังเล่น Vase Minigame
            if (CurrentMode == "Hand")
            {
                SetToolMode("Magnifier");
            }
            else if (CurrentMode == "Magnifier")
            {
                SetToolMode("Hand");
            }
        }
        else
        {
            // เงื่อนไขเมื่อไม่ได้เล่น Vase Minigame
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
    }


    public void SelectObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;

            // ตรวจสอบว่า GameObject มี Tag เป็น "SelectableObject"
            if (clickedObject.CompareTag("Object"))
            {
                // ถ้าคลิกวัตถุที่เลือกอยู่ ให้ยกเลิกการเลือก
                if (selectedObject == clickedObject)
                {
                    ApplyMaterial(selectedObject, defaultMaterial); // รีเซ็ต Material เป็นค่าเริ่มต้น
                    selectedObject = null; // ยกเลิกการเลือก
                    Debug.Log("Deselected the object.");
                }
                else
                {
                    // ถ้ามีวัตถุอื่นถูกเลือกอยู่ ให้รีเซ็ตก่อน
                    if (selectedObject != null)
                    {
                        ApplyMaterial(selectedObject, defaultMaterial); // รีเซ็ต Material เป็นค่าเริ่มต้น
                    }

                    // เลือกวัตถุใหม่
                    selectedObject = clickedObject;
                    ApplyMaterial(selectedObject, outlineMaterial); // ใส่ Material แบบ Outline
                    Debug.Log($"Selected: {selectedObject.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Clicked object {clickedObject.name} does not have the correct tag.");
            }
        }
        else
        {
            Debug.LogWarning("No object selected!");
        }
    }

    private void ApplyMaterial(GameObject obj, Material material)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.material = material; // เปลี่ยน Material ของ SpriteRenderer
        }
    }

    public GameObject GetSelectedObject()
    {
        return selectedObject;
    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
    }
}
