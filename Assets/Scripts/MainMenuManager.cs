using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject playMinigameObject;
    [SerializeField] private Transform playTransform;
    [SerializeField] private GameObject triggerObject;

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

    public void StartMinigame()
    {
        // เริ่มมินิเกม
        playMinigameObject.SetActive(true);
        playTransform.gameObject.SetActive(true);

        Debug.Log("Minigame started from MainMenuManager!");
    }

    public void SetMinigameTrigger(GameObject trigger)
    {
        triggerObject = trigger;
        Debug.Log("Minigame trigger set.");
    }
}
