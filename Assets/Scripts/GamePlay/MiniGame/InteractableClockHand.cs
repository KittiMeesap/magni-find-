// InteractableClockHand.cs
using UnityEngine;

public class InteractableClockHand : MonoBehaviour
{
    [System.Serializable]
    public struct ClockPosition
    {
        public Vector3 position; // ���˹觢ͧ���
        public Vector3 rotation; // �����ع�ͧ���
    }

    public ClockPosition[] positions; // ���˹���С����ع������ (1-12)
    public int correctPositionIndex; // ���˹觷��١��ͧ (0-11)
    public int initialPositionIndex = 0; // ���˹�������鹢ͧ���

    private int currentPositionIndex; // ���˹觻Ѩ�غѹ
    private bool isSnapped = false;

    private void Start()
    {
        // ��駤�ҵ��˹��������
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
        // ����͹仵��˹觶Ѵ� (ǹ�ٻ 0-11)
        currentPositionIndex = (currentPositionIndex + 1) % positions.Length;

        // ��駤�ҵ��˹���С����ع������˹觻Ѩ�غѹ
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
