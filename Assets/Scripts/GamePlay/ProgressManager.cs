using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public Image[] questIcons;
    private bool[] foundItems;
    public GameObject winScreen;

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

        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
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
            questIcons[i].color = foundItems[i] ? Color.white : Color.black;
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

    private void HandleWinCondition()
    {
        Debug.Log("You win!");
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }
    }
}
