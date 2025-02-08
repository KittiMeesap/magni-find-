using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get;  set; }

    public string CurrentMode { get; private set; } = "Hand";
    public Texture2D handCursor;
    public Texture2D magnifierCursor;
    public Texture2D eyeCursor;

    private GameObject selectedObject;
    private CollectibleItem[] collectibleItems;

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
        //กันเปลี่ยนเครื่องมือตอนออกเข้ามินิเกม
        if (CameraController.Instance != null && CameraController.Instance.IsZooming) return;

        

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
            if (SceneManager.GetActiveScene().name == "Gameplay")
            {
                if (CurrentMode != "Eye")
                {
                    SetToolMode("Eye");
                }
            }

            else if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                if (CurrentMode == "Hand")
                {
                    SetToolMode("Magnifier");
                }
                else
                {
                    SetToolMode("Hand");
                }
                return;
            }

            else if (CurrentMode != "Eye")
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
                if (selectedObject == clickedObject)
                {
                    selectedObject = null;
                    Debug.Log("Deselected the object.");
                }
                else
                {
                    selectedObject = clickedObject;
                    Debug.Log($"Selected: {selectedObject.name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("No object selected!");
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
