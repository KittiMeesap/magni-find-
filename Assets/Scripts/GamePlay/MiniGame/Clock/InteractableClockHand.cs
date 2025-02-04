using SpriteGlow;
using UnityEngine;

public class InteractableClockHand : MonoBehaviour
{
    [System.Serializable]
    public struct ClockPosition
    {
        public Vector3 position; // ตำแหน่งของเข็ม
        public Vector3 rotation; // การหมุนของเข็ม
    }

    public ClockPosition[] positions; // ตำแหน่งและการหมุนทั้งหมด (1-12)
    public int correctPositionIndex; // ตำแหน่งที่ถูกต้อง (0-11)
    public int initialPositionIndex = 0; // ตำแหน่งเริ่มต้นของเข็ม

    private int currentPositionIndex; // ตำแหน่งปัจจุบัน
    public bool isSnapped = false;

    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;       // ✅ Sprite ปกติ
    [SerializeField] private Sprite highlightedSprite;   // ✅ Sprite เมื่อเอาเมาส์ไปวาง

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite; // ตั้งค่า Sprite เริ่มต้น
        }
        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name} is missing a SpriteRenderer!");
        }
    }

    private void Start()
    {
        // ตั้งค่าตำแหน่งเริ่มต้น
        currentPositionIndex = initialPositionIndex;
        transform.localPosition = positions[currentPositionIndex].position;
        transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);
    }

    private void OnMouseOver()
    {
        if (ClockMinigame.Instance != null && ClockMinigame.Instance.CompletedParts >= ClockMinigame.Instance.TotalParts)
        {
            return; //  ถ้ามินิเกมจบแล้ว ให้ return ออกทันที
        }
        if (MinigameManager.Instance.IsPlayingMinigame && ToolManager.Instance.CurrentMode == "Hand")
        {
            if (spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite; // ✅ เปลี่ยนเป็น Sprite ไฮไลท์
            }

            // ตรวจจับการกดเมาส์เมื่ออยู่บนเข็มนาฬิกา
            if (Input.GetMouseButtonDown(0)) // คลิกซ้ายเพื่อหมุนไปข้างหน้า
            {
                HandleLeftClick();
            }
            else if (Input.GetMouseButtonDown(1)) // คลิกขวาเพื่อหมุนย้อนกลับ
            {
                HandleRightClick();
            }
        }
    }

    private void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite; // ✅ กลับเป็น Sprite ปกติ
        }
    }

    private void HandleLeftClick()
    {
        if (isSnapped)
        {
            isSnapped = false;
            ClockMinigame.Instance.DecreaseCompletedParts(); // ลดจำนวนชิ้นสำเร็จ
        }

        AdvancePosition();

        if (currentPositionIndex == correctPositionIndex)
        {
            SnapToTarget();
        }
    }

    private void HandleRightClick()
    {
        if (isSnapped)
        {
            isSnapped = false;
            ClockMinigame.Instance.DecreaseCompletedParts(); // ลดจำนวนชิ้นสำเร็จ
        }

        ReversePosition();

        if (currentPositionIndex == correctPositionIndex)
        {
            SnapToTarget();
        }
    }

    private void AdvancePosition()
    {
        // เลื่อนไปตำแหน่งถัดไป (วนลูป 0-11)
        currentPositionIndex = (currentPositionIndex + 1) % positions.Length;

        // ตั้งค่าตำแหน่งและการหมุนตามตำแหน่งปัจจุบัน
        transform.localPosition = positions[currentPositionIndex].position;
        transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);

        Debug.Log($"{gameObject.name} moved to position {currentPositionIndex + 1}");
    }

    private void ReversePosition()
    {
        // เลื่อนไปตำแหน่งก่อนหน้า (วนลูป 11-0)
        currentPositionIndex = (currentPositionIndex - 1 + positions.Length) % positions.Length;

        // ตั้งค่าตำแหน่งและการหมุนตามตำแหน่งปัจจุบัน
        transform.localPosition = positions[currentPositionIndex].position;
        transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);

        Debug.Log($"{gameObject.name} moved to position {currentPositionIndex + 1}");
    }

    private void SnapToTarget()
    {
        isSnapped = true;
        Debug.Log($"{gameObject.name} snapped to correct position {correctPositionIndex + 1}!");
        ClockMinigame.Instance.CompletePart();
    }
}
