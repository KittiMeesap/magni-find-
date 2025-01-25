using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private enum ActionType { Expand, Shrink } // ��������á�з�
    [SerializeField] private ActionType requiredAction; // ��á�зӷ���ͧ��� (������͢���)
    [SerializeField] private float maxScale = 3f; // ��Ҵ�٧�ش�������ö������
    [SerializeField] private float minScale = 0.5f; // ��Ҵ����ش�������ö�����
    [SerializeField] private GameObject hiddenItem; // ���������͹����

    [SerializeField] private bool canBreak = true; // ��˹��ѵ�ع��ᵡ���������
    [SerializeField] private bool canScale = true;
    private bool isActionComplete = false; // ����ҡ�á�з���������ó������������

    public void ModifyScale(float scaleChange)
    {
        if (canScale)
        {
            // ������ѵ�ع�����ѵ�ط��١���͡�������
            if (ToolManager.Instance.GetSelectedObject() != gameObject)
            {
                Debug.LogWarning($"{gameObject.name} is not the currently selected object!");
                return;
            }

            // �ҡ��á�з���������ó����� �������õ��
            if (isActionComplete) return;

            // ��Ѻ��Ҵ�ѵ��
            Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

            // �ӡѴ��Ҵ�������㹪�ǧ maxScale ��� minScale
            if (newScale.x > maxScale) newScale = Vector3.one * maxScale;
            if (newScale.x < minScale) newScale = Vector3.one * minScale;

            transform.localScale = newScale;

            // ��Ǩ�ͺ��á�з�
            if (requiredAction == ActionType.Expand) // �ҡ��ͧ��� "����"
            {
                if (scaleChange > 0 && newScale.x >= maxScale)
                {
                    RevealItem(); // ��á�зӶ١��ͧ
                }
                else if (scaleChange < 0 && newScale.x <= minScale && canBreak)
                {
                    HandleWrongAction(); // ��á�зӼԴ
                }
            }
            else if (requiredAction == ActionType.Shrink) // �ҡ��ͧ��� "���"
            {
                if (scaleChange < 0 && newScale.x <= minScale)
                {
                    RevealItem(); // ��á�зӶ١��ͧ
                }
                else if (scaleChange > 0 && newScale.x >= maxScale && canBreak)
                {
                    HandleWrongAction(); // ��á�зӼԴ
                }
            }
        }
    }

    private void RevealItem()
    {
        if (canBreak)
        {
            isActionComplete = true;
            Debug.Log($"Correct action! {gameObject.name} revealed an item.");
            if (hiddenItem != null)
            {
                hiddenItem.SetActive(true); // �ʴ����������͹����
            }
            Destroy(gameObject); // ź�ѵ�ط��ᵡ
        }
    }

    private void HandleWrongAction()
    {
        if(canBreak)
        {
            isActionComplete = true;
            Debug.Log($"Wrong action! {gameObject.name} destroyed and item lost.");
            if (hiddenItem != null)
            {
                Destroy(hiddenItem); // ź���������͹�ҡ�ӼԴ
            }
            Destroy(gameObject); // ź�ѵ�ط��ᵡ
        }
    }
}
