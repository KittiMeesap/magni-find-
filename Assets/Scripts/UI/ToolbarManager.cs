using UnityEngine;
using UnityEngine.UI;

public class ToolbarManager : MonoBehaviour
{
    public RectTransform toolbar; // RectTransform �ͧ Toolbar
    public float visibleXPosition = 0f; // ���˹� X ����� Toolbar �ʴ�
    public float hiddenXPosition = -300f; // ���˹� X ����� Toolbar ��͹
    public float moveSpeed = 5f; // ��������㹡������͹
    public Button toggleButton; // ��������Ѻ�Դ/�Դ Toolbar
    public Image arrowImage; // Image �ͧ�١��
    public Sprite arrowOpenSprite; // Sprite ����Ѻ�ʴ� Toolbar
    public Sprite arrowCloseSprite; // Sprite ����Ѻ��͹ Toolbar

    private bool isVisible = true; // ��������� "�ʴ�"

    void Start()
    {
        // �١�ѧ��ѹ ToggleToolbar �Ѻ����
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleToolbar);
        }
    }

    void ToggleToolbar()
    {
        isVisible = !isVisible; // ����¹ʶҹ�
        float targetX = isVisible ? visibleXPosition : hiddenXPosition;

        // ����¹ Sprite �١��
        UpdateArrowSprite();

        // ����͹ Toolbar ��ѧ���˹��������
        StopAllCoroutines();
        StartCoroutine(MoveToolbar(targetX));
    }

    void UpdateArrowSprite()
    {
        if (arrowImage != null)
        {
            arrowImage.sprite = isVisible ? arrowCloseSprite : arrowOpenSprite;
        }
    }

    void AutoHideToolbar()
    {
        // ������鹫�͹ Toolbar ��ѧ�ҡ�ʴ��͹�������
        isVisible = false;
        float targetX = hiddenXPosition;

        // ����¹ Sprite �١��
        UpdateArrowSprite();

        // ����͹ Toolbar ��ѧ���˹觫�͹
        StopAllCoroutines();
        StartCoroutine(MoveToolbar(targetX));
    }

    System.Collections.IEnumerator MoveToolbar(float targetX)
    {
        while (Mathf.Abs(toolbar.anchoredPosition.x - targetX) > 0.01f)
        {
            float newX = Mathf.Lerp(toolbar.anchoredPosition.x, targetX, Time.deltaTime * moveSpeed);
            toolbar.anchoredPosition = new Vector2(newX, toolbar.anchoredPosition.y);
            yield return null;
        }

        toolbar.anchoredPosition = new Vector2(targetX, toolbar.anchoredPosition.y);
    }
}
