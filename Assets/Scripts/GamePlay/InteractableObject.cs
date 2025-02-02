using UnityEngine;
using System.Collections;

public class InteractObject : MonoBehaviour
{
    [SerializeField] private MinigameType minigameType;
    public enum MinigameType
    {
        Null,
        Clock,
        Vase,
        Camera,
        Cat,
        Picture,
        Instrument,
        Bird
    }

    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;       // ✅ Sprite ปกติ
    [SerializeField] private Sprite highlightedSprite;   // ✅ Sprite เมื่อเอาเมาส์ไปวาง

    private bool isHovering = false;
    private float oscillationSpeed = 3f;
    private float oscillationAmount = 0.05f;
    private Vector3 initialPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite; // ตั้งค่า Sprite เริ่มต้น
        }

        initialPosition = transform.position;
    }

    private void OnMouseOver()
    {
        if (MinigameManager.Instance.IsPlayingMinigame == false && ToolManager.Instance.CurrentMode == "Eye")
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite; // ✅ เปลี่ยนเป็น Sprite ไฮไลท์
            }

            if (!isHovering)
            {
                isHovering = true;
                StartCoroutine(SwingObject());
            }
        }
        else
        {
            ResetObject();
        }
    }

    private void OnMouseExit()
    {
        ResetObject();
    }

    private void ResetObject()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite; // ✅ กลับเป็น Sprite ปกติ
        }

        isHovering = false;
        StopCoroutine(SwingObject());
        transform.position = initialPosition;
    }

    private IEnumerator SwingObject()
    {
        float elapsedTime = 0f;

        while (isHovering)
        {
            elapsedTime += Time.deltaTime * oscillationSpeed;
            float offset = Mathf.Sin(elapsedTime) * oscillationAmount;
            transform.position = new Vector3(initialPosition.x + offset, initialPosition.y, initialPosition.z);

            yield return null;
        }
    }

    private void OnMouseDown()
    {
        if (MinigameManager.Instance.IsPlayingMinigame == false && ToolManager.Instance.CurrentMode == "Eye")
        {
            ToolManager.Instance.SetToolMode("Hand");

            bool isMinigame = minigameType != MinigameType.Null;

            // ✅ **ให้กล้องซูมไปที่ Object ก่อนทำงาน**
            CameraController.Instance.ZoomToObject(transform, () =>
            {
                // ✅ เปิด Dialogue ก่อน Minigame
                DialogueSystem dialogueSystem = GetComponent<DialogueSystem>();
                if (dialogueSystem != null)
                {
                    dialogueSystem.ShowDialogue();
                }

                if (isMinigame)
                {
                    CameraController.Instance.EnterMinigame();
                    StartMinigame();
                }
            });
        }
    }

    private void StartMinigame()
    {
        switch (minigameType)
        {
            case MinigameType.Clock:
                ClockMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Vase:
                VaseMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Camera:
                CameraMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Cat:
                CatMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Picture:
                PictureMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Instrument:
                // InstrumentMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Bird:
                // BirdMinigame.Instance.StartMinigame();
                break;
            default:
                Debug.LogWarning("Not Minigame Object");
                break;
        }
    }

    public Transform centerPoint; // ✅ จุดศูนย์กลางที่ให้กล้องโฟกัส
}
