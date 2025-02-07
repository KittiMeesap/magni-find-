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
    [SerializeField] private float appleFadeDuration = 1f; // ✅ ระยะเวลาเฟดหายของแอปเปิ้ล
    [SerializeField] private float rotationSpeed = 720f; // ✅ ความเร็วในการหมุนแหวนระหว่างขยับไปตำแหน่ง
    private SpriteRenderer birdRenderer;
    private Collider2D birdCollider;
    private SpriteRenderer appleRenderer;
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
        appleRenderer = apple.GetComponent<SpriteRenderer>();

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
        Vector3 endPos = appleFallPosition.position;

        // ✅ คำนวณจุดโค้งกลาง ให้ตกลงแบบสมจริง
        Vector3 controlPoint = new Vector3((startPos.x + endPos.x) / 2, startPos.y + 1.5f, startPos.z);

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fallDuration;

            // ✅ ใช้ Bezier Curve สำหรับการเคลื่อนที่แบบพาราโบลา
            apple.transform.position = BezierCurve(startPos, controlPoint, endPos, t);

            yield return null;
        }

        apple.transform.position = appleFallPosition.position;
        CanPickUpApple = true; // ✅ ตอนนี้สามารถลากแอปเปิ้ลไปวางที่ถาดได้
    }

    // ✅ ฟังก์ชัน Bezier Curve สำหรับการทำให้แอปเปิ้ลตกโค้ง
    private Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }


    public void PlaceAppleInTray()
    {
        if (!CanPickUpApple) return;

        apple.transform.position = appleTrayPosition.position; // ✅ แค่ย้ายตำแหน่ง ไม่ปิด Object
        CanPickUpApple = false; // ✅ ป้องกันการลากอีก
        StartCoroutine(BirdEatApple());
    }

    private IEnumerator BirdEatApple()
    {
        yield return new WaitForSeconds(1f); // ✅ รอ 1 วินาทีก่อนที่นกจะเดินมากิน

        bird.GetComponent<InteractableBird>().SetBirdState("walking");
        float elapsedTime = 0f;
        Vector3 startPos = bird.transform.position;
        Vector3 endPos = birdEatPosition.position;

        // ✅ ขยับไปทางขวาเล็กน้อย (เพิ่มค่า X)
        Vector3 controlPoint = new Vector3((startPos.x + endPos.x) / 2 + 1.0f, startPos.y, startPos.z);

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            // ✅ ใช้ Bezier Curve เพื่อให้ขยับแบบมีโค้งไปทางขวา
            bird.transform.position = BezierCurve(startPos, controlPoint, endPos, t);

            yield return null;
        }

        bird.transform.position = birdEatPosition.position;
        bird.GetComponent<InteractableBird>().SetBirdState("eating");

        yield return new WaitForSeconds(1f); // ✅ รออีก 1 วิ ก่อนแอปเปิ้ลค่อยๆ หายไป
        StartCoroutine(FadeOutApple());

        yield return new WaitForSeconds(1f); // ✅ รออีก 1 วิ ก่อนแหวนร่วงลงมา
        StartCoroutine(DropReward());
    }

    private IEnumerator FadeOutApple()
    {
        float elapsedTime = 0f;
        Color startColor = appleRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // ✅ ค่อยๆ ทำให้โปร่งใส

        while (elapsedTime < appleFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            appleRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / appleFadeDuration);
            yield return null;
        }

        apple.SetActive(false); // ✅ ซ่อนแอปเปิ้ลเมื่อ Fade เสร็จ
    }

    private IEnumerator DropReward()
    {
        rewardItem.SetActive(true);
        float elapsedTime = 0f;
        Vector3 startPos = rewardItem.transform.position;
        Vector3 endPos = rewardFallPosition.position;
        float currentRotation = 0f;

        // ✅ คำนวณจุดกึ่งกลางให้แหวนตกเป็นโค้ง
        Vector3 controlPoint = new Vector3((startPos.x + endPos.x) / 2, startPos.y + 1.5f, startPos.z);

        while (elapsedTime < rewardMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rewardMoveDuration;

            // ✅ ใช้ Bezier Curve เพื่อให้แหวนตกแบบโค้ง
            rewardItem.transform.position = BezierCurve(startPos, controlPoint, endPos, t);

            // ✅ หมุนแหวน เร่งความเร็วตอนต้น และช้าลงตอนท้าย
            float rotationSpeedAdjusted = Mathf.Lerp(rotationSpeed, 0f, t);
            currentRotation += rotationSpeedAdjusted * Time.deltaTime;
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
