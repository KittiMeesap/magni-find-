using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private ProgressManager progressManager;
    [SerializeField] private CollectibleItem[] collectibleItems;


    private void Start()
    {

        if (progressManager == null)
        {
            Debug.LogError("ProgressManager not assigned in GameController!");
            return;
        }

        foreach (var item in collectibleItems)
        {
            item.Initialize(progressManager); // �� ProgressManager ��� CollectibleItem
        }

        ToolManager.Instance.Initialize(collectibleItems); // �� Collectible Items ��ѧ ToolManager
    }
}
