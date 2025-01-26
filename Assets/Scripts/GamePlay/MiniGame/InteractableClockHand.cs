// InteractableClockHand.cs
using System;
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

    public Material defaultMaterial; // ��ʴ��������
    public Material selectedMaterial; // ��ʴ���������͡

    private int currentPositionIndex; // ���˹觻Ѩ�غѹ
    private bool isSnapped = false;
    private static GameObject selectedObject; // �ѹ�֡������١���͡

    private void Start()
    {
        // ��駤�ҵ��˹��������
        currentPositionIndex = initialPositionIndex;
        transform.localPosition = positions[currentPositionIndex].position;
        transform.localRotation = Quaternion.Euler(positions[currentPositionIndex].rotation);
        SetMaterial(defaultMaterial); // �����ʴ��������
    }

    private void OnMouseDown()
    {
        if (ClockMinigame.Instance.isPlayingClockMinigame && ToolManager.Instance.CurrentMode == "Hand")
        {
            // ¡��ԡ��ʴآͧ���������͡��͹˹��
            if (selectedObject != null && selectedObject != gameObject)
            {
                selectedObject.GetComponent<InteractableClockHand>().SetMaterial(defaultMaterial);
            }

            // ���͡����������ͤ�ԡ
            selectedObject = gameObject;
            SetMaterial(selectedMaterial); // ����¹��ʴ�����ʴ���������͡
            Debug.Log($"{gameObject.name} selected.");
        }
    }

    private void Update()
    {
        if (ClockMinigame.Instance.isPlayingClockMinigame)
        {
            // ��Ǩ�ͺ��Ҥ�ԡ��鹷����ҧ
            if (Input.GetMouseButtonDown(0) && !IsMouseOverAnyObject())
            {
                if (selectedObject != null)
                {
                    selectedObject.GetComponent<InteractableClockHand>().SetMaterial(defaultMaterial);
                    selectedObject = null; // ¡��ԡ������͡
                    Debug.Log("Deselected the clock hand.");
                }
            }

            // ��Ǩ�ͺ������͡���
            if (selectedObject == gameObject && IsMouseOverObject() && ToolManager.Instance.CurrentMode == "Hand")
            {
                if (Input.GetMouseButtonDown(0)) // ��ԡ���·��˹�Ҩ����͢�Ѻ仢�ҧ˹��
                {
                    HandleLeftClick();
                }
                else if (Input.GetMouseButtonDown(1)) // ��ԡ��ҷ��˹�Ҩ����͢�Ѻ��͹��Ѻ
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

    private bool IsMouseOverObject()
    {
        // ��Ǩ�ͺ�������������˹���ѵ��
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = GetComponent<Collider2D>();
        return collider != null && collider.OverlapPoint(mousePos);
    }

    private bool IsMouseOverAnyObject()
    {
        // ��Ǩ�ͺ�������������˹���ѵ��� � 㹩ҡ
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

    private void SetMaterial(Material material)
    {
        // ��駤����ʴآͧ Sprite Renderer
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    public void ResetToInitialPosition(int initialPositionIndex)
    {
        // ���絵��˹觻Ѩ�غѹ���ç�Ѻ���˹��������
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
