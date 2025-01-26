// InteractableClockHand.cs
using System;
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

    public Material defaultMaterial; // วัสดุเริ่มต้น
    public Material selectedMaterial; // วัสดุเมื่อเลือก

    private int currentPositionIndex; // ตำแหน่งปัจจุบัน
    private bool isSnapped = false;
    private static GameObject selectedObject; // บันทึกเข็มที่ถูกเลือก

    private void Start()
    {
        // ตั้งค่าตำแหน่งเริ่มต้น
        currentPositionIndex = initialPositionIndex;
        transform.localPosition = positions[currentPositionIndex].position;
        transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);
        SetMaterial(defaultMaterial); // ตั้งวัสดุเริ่มต้น
    }

    private void OnMouseDown()
    {
        if (ClockMinigame.Instance.isPlayingClockMinigame && ToolManager.Instance.CurrentMode == "Hand")
        {
            // ยกเลิกวัสดุของเข็มที่เลือกก่อนหน้า
            if (selectedObject != null && selectedObject != gameObject)
            {
                selectedObject.GetComponent<InteractableClockHand>().SetMaterial(defaultMaterial);
            }

            // เลือกเข็มนี้เมื่อคลิก
            selectedObject = gameObject;
            SetMaterial(selectedMaterial); // เปลี่ยนวัสดุเป็นวัสดุเมื่อเลือก
            Debug.Log($"{gameObject.name} selected.");
        }
    }

    private void Update()
    {
        if (ClockMinigame.Instance.isPlayingClockMinigame)
        {
            // ตรวจสอบว่าคลิกพื้นที่ว่าง
            if (Input.GetMouseButtonDown(0) && !IsMouseOverAnyObject())
            {
                if (selectedObject != null)
                {
                    selectedObject.GetComponent<InteractableClockHand>().SetMaterial(defaultMaterial);
                    selectedObject = null; // ยกเลิกการเลือก
                    Debug.Log("Deselected the clock hand.");
                }
            }

            // ตรวจสอบการเลือกเข็ม
            if (selectedObject == gameObject && IsMouseOverObject() && ToolManager.Instance.CurrentMode == "Hand")
            {
                if (Input.GetMouseButtonDown(0)) // คลิกซ้ายที่หน้าจอเพื่อขยับไปข้างหน้า
                {
                    HandleLeftClick();
                }
                else if (Input.GetMouseButtonDown(1)) // คลิกขวาที่หน้าจอเพื่อขยับย้อนกลับ
                {
                    HandleRightClick();
                }
            }
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

    private bool IsMouseOverObject()
    {
        // ตรวจสอบว่าเมาส์อยู่เหนือวัตถุ
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = GetComponent<Collider2D>();
        return collider != null && collider.OverlapPoint(mousePos);
    }

    private bool IsMouseOverAnyObject()
    {
        // ตรวจสอบว่าเมาส์อยู่เหนือวัตถุใด ๆ ในฉาก
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.GetComponent<InteractableClockHand>() != null)
            {
                return true;
            }
        }
        return false;
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

    private void SetMaterial(Material material)
    {
        // ตั้งค่าวัสดุของ Sprite Renderer
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    public void ResetToInitialPosition(int initialPositionIndex)
    {
        // รีเซ็ตตำแหน่งปัจจุบันให้ตรงกับตำแหน่งเริ่มต้น
        if (positions != null && positions.Length > initialPositionIndex)
        {
            currentPositionIndex = initialPositionIndex;
            transform.localPosition = positions[currentPositionIndex].position;
            transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);
            Debug.Log($"{gameObject.name} reset to initial position: {currentPositionIndex}");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Invalid initial position index or positions array.");
        }
    }

}
