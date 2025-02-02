using UnityEngine;
using System.Collections;

public class InteractHintObject : MonoBehaviour
{
    [SerializeField] private HintType hintType;

    public enum HintType
    {
        Null,
        Book,
        Note
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
        if (HintManager.Instance.IsPlayingHint == false && ToolManager.Instance.CurrentMode == "Eye")
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
        if (HintManager.Instance.IsPlayingHint == false && ToolManager.Instance.CurrentMode == "Eye")
        {
            ToolManager.Instance.SetToolMode("Hand");

            bool isHint = hintType != HintType.Null;

            // ✅ **ให้กล้องซูมไปที่ Object ก่อนทำงาน**
            CameraController.Instance.ZoomToObject(transform, () =>
            {
                // ✅ เปิด Dialogue ก่อน Minigame
                Dialogue dialogueSystem = GetComponent<Dialogue>();  // ใช้ Dialogue แทน DialogueManager

                if (dialogueSystem != null)
                {
                    // เช็คว่าใช้ Magnifier หรือไม่ และเลือกข้อความที่ต้องการ
                    string messageToShow = ToolManager.Instance.CurrentMode == "Magnifier" && dialogueSystem.hasMagnifierMessage
                        ? dialogueSystem.dialogueText.text
                        : dialogueSystem.dialogueText.text;

                    // ส่งข้อความไปยัง DialogueManager เพื่อแสดง
                    DialogueManager.Instance.ShowDialogue(messageToShow, gameObject);
                }

                if (isHint)
                {
                    CameraController.Instance.EnterHint();
                    StartHint();
                }
            });
        }
    }

    

    private void StartHint()
    {
        switch (hintType)
        {
            case HintType.Book:
                BookHint.Instance.StartHint();
                break;
            //case HintType.Note:
                //NoteHint.Instance.StartMinigame();
                //break;
            default:
                Debug.LogWarning("Not Minigame Object");
                break;
        }
    }

    public Transform centerPoint; // ✅ จุดศูนย์กลางที่ให้กล้องโฟกัส
}
