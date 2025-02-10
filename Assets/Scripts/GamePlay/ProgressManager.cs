using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public Image[] questIcons; // ✅ ไอคอนของภารกิจ
    public Sprite[] defaultSprites; // ✅ ภาพตอนยังไม่เจอ
    public Sprite[] foundSprites; // ✅ ภาพตอนเจอแล้ว
    [SerializeField] GameObject hideUI;

    private bool[] foundItems;

    public static ProgressManager Instance { get; private set; } // Singleton

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        foundItems = new bool[questIcons.Length];
        UpdateQuestUI();
    }

    public void MarkAsFound(int index)
    {
        if (!foundItems[index])
        {
            foundItems[index] = true;
            UpdateQuestUI();

            if (CheckIfAllItemsFound())
            {
                HandleWinCondition();
            }
        }
    }

    private void UpdateQuestUI()
    {
        for (int i = 0; i < questIcons.Length; i++)
        {
            // ✅ ถ้ามีไอเทมให้เปลี่ยนเป็น foundSprites
            questIcons[i].sprite = foundItems[i] ? foundSprites[i] : defaultSprites[i];
        }
    }

    private bool CheckIfAllItemsFound()
    {
        foreach (bool found in foundItems)
        {
            if (!found) return false;
        }
        return true;
    }

    public void HandleWinCondition()
    {
        Debug.Log("You win!");
        SceneTransitionManager.Instance.LoadScene("Win");
        hideUI.SetActive(false);

    }
}
