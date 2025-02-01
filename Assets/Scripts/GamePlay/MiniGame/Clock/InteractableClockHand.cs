using SpriteGlow;
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
    public bool isSnapped = false;

    private SpriteGlowEffect glowEffect;

    private void Awake()
    {
        glowEffect = GetComponent<SpriteGlowEffect>();
    }

    private void Start()
    {
        // ��駤�ҵ��˹��������
        currentPositionIndex = initialPositionIndex;
        transform.localPosition = positions[currentPositionIndex].position;
        transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);
    }

    private void OnMouseOver()
    {
        if (MinigameManager.Instance.IsPlayingMinigame && ToolManager.Instance.CurrentMode == "Hand")
        {
            if (glowEffect != null)
            {
                glowEffect.enabled = true; // �Դ�Ϳ࿡�� Glow
            }

            // ��Ǩ�Ѻ��á��������������躹������ԡ�
            if (Input.GetMouseButtonDown(0)) // ��ԡ����������ع仢�ҧ˹��
            {
                HandleLeftClick();
            }
            else if (Input.GetMouseButtonDown(1)) // ��ԡ���������ع��͹��Ѻ
            {
                HandleRightClick();
            }
        }
        else
        {
            if (glowEffect != null)
            {
                glowEffect.enabled = false; // �Դ�Ϳ࿡�� Glow
            }
        }
    }

    private void OnMouseExit()
    {
        if (glowEffect != null)
        {
            glowEffect.enabled = false; // �Դ�Ϳ࿡�� Glow
        }
    }

    private void HandleLeftClick()
    {
        if (isSnapped)
        {
            isSnapped = false;
            ClockMinigame.Instance.DecreaseCompletedParts(); // Ŵ�ӹǹ��������
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
            ClockMinigame.Instance.DecreaseCompletedParts(); // Ŵ�ӹǹ��������
        }

        ReversePosition();

        if (currentPositionIndex == correctPositionIndex)
        {
            SnapToTarget();
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

    private void ReversePosition()
    {
        // ����͹仵��˹觡�͹˹�� (ǹ�ٻ 11-0)
        currentPositionIndex = (currentPositionIndex - 1 + positions.Length) % positions.Length;

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
