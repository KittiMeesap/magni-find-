// InteractableClockHand.cs
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
    private bool isSnapped = false;

    private void Start()
    {
        // ตั้งค่าตำแหน่งเริ่มต้น
        currentPositionIndex = initialPositionIndex;
        transform.localPosition = positions[currentPositionIndex].position;
        transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);
    }

    private void OnMouseDown()
    {
        if (ClockMinigame.Instance.isPlayingClockMinigame && !isSnapped)
        {
            AdvancePosition();

            if (currentPositionIndex == correctPositionIndex)
            {
                SnapToTarget();
            }
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

    private void SnapToTarget()
    {
        isSnapped = true;
        Debug.Log($"{gameObject.name} snapped to correct position {correctPositionIndex + 1}!");
        ClockMinigame.Instance.CompletePart();
    }
}
