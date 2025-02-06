using UnityEngine;
using System.Collections;

public class BirdMinigame : MonoBehaviour
{
    public static BirdMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject birdMinigameObject;
    [SerializeField] private GameObject rewardItem;
    [SerializeField] private GameObject bird;
    [SerializeField] private GameObject apple; // เก็บเป็น private
    public GameObject Apple => apple; // ใช้ Getter เพื่อให้เข้าถึงได้


    [SerializeField] private Vector3 correctAppleScale;

    public bool IsAppleCorrectSize { get; private set; } = false;
    private bool isMinigameComplete = false;

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
        MinigameManager.Instance.StartMinigame(birdMinigameObject, bird.transform, rewardItem, true);
        MinigameManager.Instance.SetMinigameTrigger(triggerObject);
    }

    public void CheckAppleSize()
    {
        if (Vector3.Distance(apple.transform.localScale, correctAppleScale) <= 0.05f)
        {
            IsAppleCorrectSize = true;
            bird.GetComponent<InteractableBird>().SetBirdState("readyToEat");
        }
        else
        {
            IsAppleCorrectSize = false;
            bird.GetComponent<InteractableBird>().SetBirdState("idle");
        }
    }

    public void TryFeedBird()
    {
        if (IsAppleCorrectSize && !isMinigameComplete)
        {
            isMinigameComplete = true;
            StartCoroutine(CompleteMinigame());
        }
    }

    private IEnumerator CompleteMinigame()
    {
        bird.GetComponent<InteractableBird>().SetBirdState("eating");
        yield return new WaitForSeconds(1.0f);
        rewardItem.SetActive(true);
        MinigameManager.Instance.CompleteMinigame();
    }
}
