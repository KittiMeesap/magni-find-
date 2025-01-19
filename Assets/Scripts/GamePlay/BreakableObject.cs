using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public enum ActionType { Expand, Shrink } // ��á�зӷ���ͧ���
    public ActionType requiredAction; // ���͹�: �����������
    public float maxScale = 3f; // ��Ҵ�٧�ش
    public float minScale = 0.5f; // ��Ҵ����ش
    public GameObject hiddenItem; // ���������͹������ѵ��

    private bool isActionComplete = false; // ��Ǩ�ͺ������͹�����������������

    public void ModifyScale(float scaleChange)
    {
        // ��Ǩ�ͺ������ѵ�ض١���͡�����������
        if (ToolManager.Instance.GetSelectedObject() != gameObject)
        {
            Debug.LogWarning($"{gameObject.name} is not the currently selected object!");
            return; // ��ش�ӧҹ�ҡ�ѵ�ع�������١���͡
        }

        if (isActionComplete) return; // �ҡ���͹���������� ��ش��÷ӧҹ

        Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

        // �ӡѴ��Ҵ�������Թ�ͺࢵ
        if (newScale.x > maxScale) newScale = Vector3.one * maxScale;
        if (newScale.x < minScale) newScale = Vector3.one * minScale;

        transform.localScale = newScale;

        // �óշ���ͧ������ "����"
        if (requiredAction == ActionType.Expand)
        {
            if (scaleChange > 0 && newScale.x >= maxScale) // ���¨��֧ maxScale
            {
                RevealItem();
            }
            else if (scaleChange < 0 && newScale.x <= minScale) // ��ͨ��֧ minScale
            {
                HandleWrongAction();
            }
        }
        // �óշ���ͧ������ "���"
        else if (requiredAction == ActionType.Shrink)
        {
            if (scaleChange < 0 && newScale.x <= minScale) // ��ͨ��֧ minScale
            {
                RevealItem();
            }
            else if (scaleChange > 0 && newScale.x >= maxScale) // ���¨��֧ maxScale
            {
                HandleWrongAction();
            }
        }
    }

    private void RevealItem()
    {
        isActionComplete = true;
        Debug.Log($"Correct action! {gameObject.name} revealed an item.");
        if (hiddenItem != null)
        {
            hiddenItem.SetActive(true); // �ʴ������
        }
        Destroy(gameObject); // ������ѵ��
    }

    private void HandleWrongAction()
    {
        isActionComplete = true;
        Debug.Log($"Wrong action! {gameObject.name} destroyed and item lost.");
        if (hiddenItem != null)
        {
            Destroy(hiddenItem); // ����������
        }
        Destroy(gameObject); // ������ѵ��
    }
}
