using UnityEngine;
using System.Collections;

public class BirdMinigame : MonoBehaviour
{
    public static BirdMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject birdMinigameObject;
    [SerializeField] private GameObject rewardItem; // ✅ แหวน
    [SerializeField] private GameObject bird;
    [SerializeField] private GameObject apple;
    public GameObject Apple => apple;

    [SerializeField] private Transform appleFallPosition; // ✅ ตำแหน่งที่แอปเปิ้ลร่วง
    [SerializeField] private Transform appleTrayPosition; // ✅ ตำแหน่งถาดใส่อาหารนก
    [SerializeField] private Transform birdEatPosition; // ✅ ตำแหน่งที่นกจะเดินไปกิน
    [SerializeField] private Transform rewardFallPosition; // ✅ ตำแหน่งที่แหวนจะตก

    [SerializeField] private Vector3 correctAppleScale;
    [SerializeField] private float fallDuration = 0.8f; // ✅ ระยะเวลาที่แอปเปิ้ลร่วง
    [SerializeField] private float moveDuration = 1f; // ✅ ระยะเวลาที่นกเดินไปกิน
    [SerializeField] private float rewardMoveDuration = 1f; // ✅ ระยะเวลาที่แหวนขยับไปตำแหน่งที่ตั้งไว้
    [SerializeField] private float rotationSpeed = 720f; // ✅ ความเร็วในการหมุนแหวนระหว่างขยับไปตำแหน่ง
    private SpriteRenderer birdRenderer;
    private Collider2D birdCollider;
    private bool isMinigameComplete = false;
    public bool IsAppleCorrectSize { get; private set; } = false;
    public bool CanPickUpApple { get; private set; } = false; // ✅ เช็คว่าแอปเปิ้ลร่วงแล้วหรือยัง

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

        birdRenderer = bird.GetComponent<SpriteRenderer>();
        birdCollider = bird.GetComponent<Collider2D>();

        // ✅ ซ่อนรางวัลตอนเริ่มเกม
        rewardItem.SetActive(false);
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

            // ✅ หลังจากผ่านไป 1 วิ ให้แอปเปิ้ลร่วงลงไป
            StartCoroutine(DropApple());
        }
        else
        {
            IsAppleCorrectSize = false;
            bird.GetComponent<InteractableBird>().SetBirdState("idle");
        }
    }

    private IEnumerator DropApple()
    {
        yield return new WaitForSeconds(1f); // ✅ รอ 1 วินาที
        float elapsedTime = 0f;
        Vector3 startPos = apple.transform.position;

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            apple.transform.position = Vector3.Lerp(startPos, appleFallPosition.position, elapsedTime / fallDuration);
            yield return null;
        }

        apple.transform.position = appleFallPosition.position;
        CanPickUpApple = true; // ✅ ตอนนี้สามารถลากแอปเปิ้ลไปวางที่ถาดได้
    }

    public void PlaceAppleInTray()
    {
        if (!CanPickUpApple) return;

        apple.transform.position = appleTrayPosition.position;
        CanPickUpApple = false; // ✅ ป้องกันการลากอีก
        StartCoroutine(BirdEatApple());
    }

    private IEnumerator BirdEatApple()
    {
        yield return new WaitForSeconds(1f); // ✅ รอ 1 วินาทีก่อนที่นกจะเดินมากิน

        bird.GetComponent<InteractableBird>().SetBirdState("walking");
        float elapsedTime = 0f;
        Vector3 startPos = bird.transform.position;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            bird.transform.position = Vector3.Lerp(startPos, birdEatPosition.position, elapsedTime / moveDuration);
            yield return null;
        }

        bird.transform.position = birdEatPosition.position;
        bird.GetComponent<InteractableBird>().SetBirdState("eating");

        yield return new WaitForSeconds(1f); // ✅ รออีก 1 วิ ก่อนแหวนร่วงลงมา

        StartCoroutine(DropReward());
    }

    private IEnumerator DropReward()
    {
        rewardItem.SetActive(true);
        float elapsedTime = 0f;
        Vector3 startPos = rewardItem.transform.position;
        float currentRotation = 0f; // ✅ ใช้คุมการหมุนของแหวน

        while (elapsedTime < rewardMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rewardMoveDuration;

            // ✅ ขยับแหวนไปตำแหน่งที่ตั้งไว้
            rewardItem.transform.position = Vector3.Lerp(startPos, rewardFallPosition.position, progress);

            // ✅ หมุนแหวนระหว่างขยับ
            currentRotation += rotationSpeed * Time.deltaTime;
            rewardItem.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

            yield return null;
        }

        // ✅ ตั้งค่าให้แหวนหยุดหมุนเมื่อถึงตำแหน่ง
        rewardItem.transform.position = rewardFallPosition.position;
        rewardItem.transform.rotation = Quaternion.identity;

        // ✅ แจ้งว่า Minigame เสร็จแล้ว
        MinigameManager.Instance.CompleteMinigame();
    }
}
