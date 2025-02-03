using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get;  set; }

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
        SetToolMode("Eye");

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

        /*if (CurrentMode == "Hand" && Input.GetMouseButtonDown(0))
        {
            if (!MinigameManager.Instance.IsPlayingMinigame)
            {
                SelectObject();
            }
        }

        if (CurrentMode == "Eye")
        {
            DialogueSystem dialogueSystem = GetComponent<DialogueSystem>();
            if (dialogueSystem != null)
            {
                dialogueSystem.ShowDialogue();
            }
            /*GameObject targetObject = Instance.GetSelectedObject();

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


                    //MiniGame Start
                    InteractableObject interactableObject = targetObject.GetComponent<InteractableObject>();

                    if (interactableObject.IsMiniGameObject == true)
                    {
                        SetToolMode("Hand");

                        if (selectedObject)
                        {
                            ApplyMaterial(selectedObject, defaultMaterial); // ÃÕà«çµ Material à»ç¹¤èÒàÃÔèÁµé¹
                            selectedObject = null; // Â¡àÅÔ¡¡ÒÃàÅ×Í¡
                            Debug.Log("Deselected the object.");
                        }

                        if (interactableObject.minigameNumber == 1)
                        {
                            VaseMinigame.Instance.StartMinigame(3);
                        }
                        else if (interactableObject.minigameNumber == 2)
                        {
                            ClockMinigame.Instance.StartMinigame();
                        }
                    }
                }
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
        }*/
    }

    /*void ModifyObjectScale(GameObject target, float scaleChange)
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
    }*/

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
        //กันเปลี่ยนเครื่องมือตอนออกเข้ามินิเกม
        if (CameraController.Instance != null && CameraController.Instance.isZooming) return;

        

        if (DialogueUI.Instance != null && DialogueUI.Instance.IsOpenDialog)
        {
            if (MinigameManager.Instance != null && MinigameManager.Instance.IsPlayingMinigame)
            {
                if (CurrentMode == "Hand")
                {
                    SetToolMode("Magnifier");
                }
                else if (CurrentMode == "Magnifier")
                {
                    SetToolMode("Hand");
                }
                
                return;
            }
            if (CurrentMode != "Eye" || CurrentMode == "Magnifier")
            {
                SetToolMode("Hand");
            }
        }

        else
        {
            if (CurrentMode != "Eye")
            {
                SetToolMode("Eye");
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

            if (clickedObject.CompareTag("Object"))
            {
                // ¶éÒ¤ÅÔ¡ÇÑµ¶Ø·ÕèàÅ×Í¡ÍÂÙè ãËéÂ¡àÅÔ¡¡ÒÃàÅ×Í¡
                if (selectedObject == clickedObject)
                {
                    ApplyMaterial(selectedObject, defaultMaterial); // ÃÕà«çµ Material à»ç¹¤èÒàÃÔèÁµé¹
                    selectedObject = null; // Â¡àÅÔ¡¡ÒÃàÅ×Í¡
                    Debug.Log("Deselected the object.");
                }
                else
                {
                    // ¶éÒÁÕÇÑµ¶ØÍ×è¹¶Ù¡àÅ×Í¡ÍÂÙè ãËéÃÕà«çµ¡èÍ¹
                    if (selectedObject != null)
                    {
                        ApplyMaterial(selectedObject, defaultMaterial); // ÃÕà«çµ Material à»ç¹¤èÒàÃÔèÁµé¹
                    }

                    // àÅ×Í¡ÇÑµ¶ØãËÁè
                    selectedObject = clickedObject;
                    ApplyMaterial(selectedObject, outlineMaterial); // ãÊè Material áºº Outline
                    Debug.Log($"Selected: {selectedObject.name}");
                }
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
            sr.material = material; // à»ÅÕèÂ¹ Material ¢Í§ SpriteRenderer
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
