using UnityEngine;

public class ClockMinigame : MonoBehaviour
{
    [Header("Clock Components")]
    public Transform hourHand; // ����������
    public Transform minuteHand; // ����ҷ�
    public Vector3 correctHourHandRotation; // ������١��ͧ�ͧ����������
    public Vector3 correctMinuteHandRotation; // ������١��ͧ�ͧ����ҷ�
    public float rotationStep = 10f; // �дѺ�����ع����Ф���
    public float rotationTolerance = 5f; // ������Ҵ����͹�ͧ����������Ѻ��

    [Header("Clock Numbers")]
    public Vector3 correctScale; // ��Ҵ���١��ͧ�ͧ����Ţ
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // ��Ҵ����ش
    public Vector3 maxScale = new Vector3(3f, 3f, 3f); // ��Ҵ�٧�ش
    public float scaleStep = 0.1f; // ��鹵͹��û�Ѻ��Ҵ
    public float scaleTolerance = 0.1f; // ������Ҵ����͹�ͧ��Ҵ�������Ѻ��

    private bool isDraggingHourHand = false;
    private bool isDraggingMinuteHand = false;

    void Update()
    {
        // Handle Hand Mode
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            if (isDraggingHourHand)
            {
                RotateHand(hourHand);
            }
            else if (isDraggingMinuteHand)
            {
                RotateHand(minuteHand);
            }
        }

        // Handle Magnifier Mode
        if (ToolManager.Instance.CurrentMode == "Magnifier" && ToolManager.Instance.GetSelectedObject() == gameObject)
        {
            if (Input.GetMouseButtonDown(0)) // ��ԡ�������͢���
            {
                ModifyScale(scaleStep);
            }
            else if (Input.GetMouseButtonDown(1)) // ��ԡ�������Ŵ��Ҵ
            {
                ModifyScale(-scaleStep);
            }
        }

        // Check Completion
        if (IsClockCorrect() && IsScaleCorrect())
        {
            Debug.Log("Clock Minigame Completed!");
            //MinigameManager.Instance.CompleteMinigame(); // ������Թ������������
        }
    }

    private void RotateHand(Transform hand)
    {
        Vector3 rotation = hand.eulerAngles;
        rotation.z += Input.GetAxis("Mouse X") * rotationStep * Time.deltaTime;
        hand.eulerAngles = rotation;
    }

    private void ModifyScale(float scaleChange)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

        // Clamp scale within min and max
        newScale.x = Mathf.Clamp(newScale.x, minScale.x, maxScale.x);
        newScale.y = Mathf.Clamp(newScale.y, minScale.y, maxScale.y);
        newScale.z = Mathf.Clamp(newScale.z, minScale.z, maxScale.z);

        transform.localScale = newScale;

        Debug.Log($"Modified scale of {gameObject.name} to {newScale}");
    }

    private bool IsClockCorrect()
    {
        // ��Ǩ�ͺ���������ԡ������������١��ͧ
        return Mathf.Abs(Vector3.Distance(hourHand.eulerAngles, correctHourHandRotation)) <= rotationTolerance &&
               Mathf.Abs(Vector3.Distance(minuteHand.eulerAngles, correctMinuteHandRotation)) <= rotationTolerance;
    }

    private bool IsScaleCorrect()
    {
        // ��Ǩ�ͺ��Ң�Ҵ����Ţ����㹪�ǧ����˹�
        return Mathf.Abs(transform.localScale.x - correctScale.x) <= scaleTolerance &&
               Mathf.Abs(transform.localScale.y - correctScale.y) <= scaleTolerance &&
               Mathf.Abs(transform.localScale.z - correctScale.z) <= scaleTolerance;
    }

    private void OnMouseDown()
    {
        // �������ع���� Hand Mode
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float hourDistance = Vector2.Distance(mousePos, hourHand.position);
            float minuteDistance = Vector2.Distance(mousePos, minuteHand.position);

            if (hourDistance < minuteDistance)
            {
                isDraggingHourHand = true;
            }
            else
            {
                isDraggingMinuteHand = true;
            }
        }
    }

    private void OnMouseUp()
    {
        // ��ش��ع���
        isDraggingHourHand = false;
        isDraggingMinuteHand = false;
    }
}
