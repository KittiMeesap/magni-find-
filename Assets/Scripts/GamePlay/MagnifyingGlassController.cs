using UnityEngine;

public class MagnifyingGlassController : MonoBehaviour
{
    public float zoomAmount = 0.1f; // ����ҳ������/���µ�ͤ�ԡ
    private GameObject targetObject; // �ѵ�ط��١���͡

    void Update()
    {
        // ��Ǩ�Ѻ��ä�ԡ���͡�ѵ��
        if (Input.GetMouseButtonDown(0) && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                targetObject = hit.collider.gameObject;
                Debug.Log($"Target selected: {targetObject.name}");
            }
        }

        // ���/����੾���ѵ�ط�����͡
        if (targetObject != null && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (Input.GetMouseButtonDown(0)) // ��ԡ���� -> ����
            {
                ModifyObjectScale(zoomAmount);
            }
            else if (Input.GetMouseButtonDown(1)) // ��ԡ��� -> ���
            {
                ModifyObjectScale(-zoomAmount);
            }
        }
    }

    void ModifyObjectScale(float scaleChange)
    {
        BreakableObject breakable = targetObject.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            breakable.ModifyScale(scaleChange);
        }
        else
        {
            Debug.LogWarning("Selected object is not breakable.");
        }
    }

}
