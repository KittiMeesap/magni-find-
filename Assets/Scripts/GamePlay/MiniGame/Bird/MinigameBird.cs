using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.XR;

public class BirdMinigame : MonoBehaviour
{
    public static BirdMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private DialogueSystem dialogueHint;
    [SerializeField] private DialogueSystem dialogue;
    [SerializeField] private GameObject birdMinigameObject;
    [SerializeField] private GameObject rewardItem; // ✅ แหวน
    [SerializeField] private GameObject bird;
    [SerializeField] private GameObject apple;
    public GameObject Apple => apple;

    [SerializeField] private Transform appleFallPosition;
    public Transform AppleFallPosition => appleFallPosition;// ✅ ตำแหน่งที่แอปเปิ้ลร่วง
    [SerializeField] private Transform appleTrayPosition;
    public Transform AppleTrayPosition => appleTrayPosition;// ✅ ตำแหน่งถาดใส่อาหารนก
    [SerializeField] private Transform birdEatPosition;
    public Transform BirdEatPosition => birdEatPosition;// ✅ ตำแหน่งที่นกจะเดินไปกิน
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

    private bool isShakingTray = false;

    private Vector3 initialTrayPosition;
    private Quaternion initialTrayRotation;

    [SerializeField] private GameObject ring;

    [SerializeField] private InteractObject interactObject;

    [SerializeField] private AudioClip sfx_AppleFall;
    [SerializeField] private AudioClip sfx_AppleTray;
    [SerializeField] private AudioClip sfx_BirdEating;
    [SerializeField] private AudioClip sfx_BirdAfterEat;
    [SerializeField] private AudioClip sfx_RingFall;

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

        initialTrayPosition = appleTrayPosition.position;
        initialTrayRotation = appleTrayPosition.rotation;

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
        DialogueUI.Instance.DialogueButton(false);

        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlaySFX(sfx_AppleFall);
        float elapsedTime = 0f;
        Vector3 startPos = apple.transform.position;
        Vector3 endPos = appleFallPosition.position;
        Vector3 controlPoint = new Vector3((startPos.x + endPos.x) / 2, startPos.y + 1.5f, startPos.z);

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fallDuration;
            apple.transform.position = BezierCurve(startPos, controlPoint, endPos, t);
            yield return null;
        }

        apple.transform.position = appleFallPosition.position;
        CanPickUpApple = true;
        apple.GetComponent<InteractableApple>().MarkAsFallen();
        DialogueUI.Instance.DialogueButton(true);
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
        SoundManager.Instance.PlaySFX(sfx_AppleFall);
        StartCoroutine(BirdEatApple());
    }

    private IEnumerator BirdEatApple()
    {
        DialogueUI.Instance.DialogueButton(false);
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
        SoundManager.Instance.PlaySFX(sfx_BirdEating);

        yield return new WaitForSeconds(1f); // ✅ รออีก 1 วิ ก่อนแอปเปิ้ลค่อยๆ หายไป
        StartCoroutine(FadeOutApple());

        yield return new WaitForSeconds(1f); // ✅ รออีก 1 วิ ก่อนแหวนร่วงลงมา
        StartCoroutine(DropReward());
    }

    private IEnumerator FadeOutApple()
    {
        SpriteRenderer appleSR = apple.GetComponent<SpriteRenderer>();
        if (appleSR == null) yield break;

        float elapsedTime = 0f;
        float fadeDuration = 1f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            appleSR.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        apple.SetActive(false);
        SoundManager.Instance.PlaySFX(sfx_BirdAfterEat);
        DialogueUI.Instance.DialogueButton(true);
    }


    private IEnumerator DropReward()
    {
        rewardItem.SetActive(true);
        SoundManager.Instance.PlaySFX(sfx_RingFall);
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
        DialogueUI.Instance.DialogueButton(true);
        ring.SetActive(true);
        dialogue.dialogueText = "\"Something just fell from that bird??\"";
        DialogueUI.Instance.DialogueUpdate(dialogue.dialogueText);
        dialogueHint.dialogueText = "\"The Ring is gone\"";
        interactObject.CheckMinigameDone();
    }

    public IEnumerator ShakeTray()
    {
        if (isShakingTray) yield break;
        isShakingTray = true;

        float shakeAngle = 1f; // ✅ กำหนดองศาที่จะโยก
        float shakeDuration = 0.5f; // ✅ ระยะเวลาโยก
        float elapsedTime = 0f;
        float direction = 1f; // ✅ ใช้กำหนดทิศทางการโยกซ้าย-ขวา

        Quaternion originalRotation = appleTrayPosition.rotation;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Sin(elapsedTime / shakeDuration * Mathf.PI); // ✅ ทำให้โยกไปกลับแบบ smooth
            float angle = shakeAngle * t * direction;

            appleTrayPosition.rotation = originalRotation * Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        appleTrayPosition.rotation = originalRotation;
        isShakingTray = false;
    }


    public void ResetTray()
    {
        appleTrayPosition.position = initialTrayPosition; // ✅ รีเซ็ต Tray ไปที่ตำแหน่งเริ่มต้น
        appleTrayPosition.rotation = initialTrayRotation; // ✅ รีเซ็ตการหมุนของ Tray
    }
}
