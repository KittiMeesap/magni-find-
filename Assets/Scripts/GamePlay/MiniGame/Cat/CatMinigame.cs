using UnityEngine;
using System.Collections;

public class CatMinigame : MonoBehaviour
{
    public static CatMinigame Instance { get; private set; }

    [SerializeField] private GameObject catMinigame;
    [SerializeField] private Transform catObject;
    [SerializeField] private Transform initialCatPosition;
    [SerializeField] private Vector3 initialCatScale = new Vector3(2f, 2f, 1f);
    [SerializeField] private SpriteRenderer catSpriteRenderer;
    [SerializeField] private Sprite catThinSprite, catFatSprite;
    [SerializeField] private Transform mouseSpawnPoint;
    [SerializeField] private GameObject mouseObject;
    [SerializeField] private Transform mouseTargetPoint;
    [SerializeField] private Transform catTargetPoint;
    [SerializeField] private Animator catAnimator;
    [SerializeField] private Animator mouseAnimator;
    [SerializeField] private GameObject rewardItem;
    [SerializeField] private GameObject gameEntryObject; // ✅ Object ที่ใช้กดเข้ามินิเกม
    [SerializeField] private float requiredMouseScale = 0.8f;
    [SerializeField] private float moveSpeed = 2f;

    private bool hasMinigameCompleted = false;
    private bool isMouseSizedCorrectly = false;
    private bool isMouseMoving = false;
    private bool isCatMoving = false;

    private float smallCatSize = 1.5f;
    private float bigCatSize = 2.5f;
    private bool isMouseSpawned = false;

    // ✅ เพิ่มตัวแปรเก็บสถานะ
    private bool isPaused = false;
    private Vector3 savedCatPosition;
    private Vector3 savedMousePosition;
    private Vector3 savedCatScale;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ResetCatToInitialState();
        mouseObject.SetActive(false);
        rewardItem.SetActive(false);
    }

    public void StartMinigame()
    {
        MinigameManager.Instance.StartMinigame(catMinigame, catMinigame.transform, rewardItem, true);

        if (hasMinigameCompleted)
        {
            // ✅ ถ้าเล่นจบแล้ว ให้แมวอยู่ตำแหน่งเดิม และแสดงรางวัล
            catObject.position = catTargetPoint.position;
            catAnimator.SetTrigger("Idle");
            mouseObject.SetActive(false);
            rewardItem.SetActive(true);
            if (gameEntryObject != null) gameEntryObject.SetActive(false);
        }
        else if (isPaused)
        {
            // ✅ ถ้ากลับมาเล่นต่อ ให้แมวและหนูอยู่ตำแหน่งล่าสุด
            catObject.position = savedCatPosition;
            catObject.localScale = savedCatScale;
            mouseObject.transform.position = savedMousePosition;
            mouseObject.SetActive(isMouseSpawned);

            isPaused = false;
        }
        else
        {
            // ✅ เล่นใหม่ (ยังไม่จบ)
            isMouseSpawned = false;
            isMouseSizedCorrectly = false;
            isMouseMoving = false;
            isCatMoving = false;

            ResetCatToInitialState();
        }
    }

    public void PauseMinigame()
    {
        // ✅ บันทึกสถานะปัจจุบันของแมวและหนู
        savedCatPosition = catObject.position;
        savedMousePosition = mouseObject.transform.position;
        savedCatScale = catObject.localScale;
        isPaused = true;

        MinigameManager.Instance.PauseMinigame();
    }

    public void ResumeMinigame()
    {
        MinigameManager.Instance.ResumeMinigame(catMinigame, catMinigame.transform, rewardItem, true);
    }

    private void ResetCatToInitialState()
    {
        if (initialCatPosition != null)
        {
            catObject.position = initialCatPosition.position;
        }
        catObject.localScale = initialCatScale;
        catSpriteRenderer.sprite = catThinSprite;
    }

    public void AdjustCatSize(float size)
    {
        catObject.localScale = new Vector3(size, size, 1);

        if (size < smallCatSize)
        {
            catSpriteRenderer.sprite = catThinSprite;
        }
        else if (size >= bigCatSize)
        {
            catSpriteRenderer.sprite = catFatSprite;
        }

        if (!isMouseSpawned && (Mathf.Approximately(size, smallCatSize) || Mathf.Approximately(size, bigCatSize)))
        {
            isMouseSpawned = true;
            SpawnMouse();
        }
    }

    private void SpawnMouse()
    {
        mouseObject.SetActive(true);
        mouseObject.transform.position = mouseSpawnPoint.position;
    }

    public void AdjustMouseSize(float size)
    {
        mouseObject.transform.localScale = new Vector3(size, size, 1);

        if (Mathf.Approximately(size, requiredMouseScale))
        {
            isMouseSizedCorrectly = true;
            if (!isMouseMoving)
            {
                StartCoroutine(MoveMouseToTarget());
            }
        }
    }

    private IEnumerator MoveMouseToTarget()
    {
        isMouseMoving = true;
        mouseAnimator.SetTrigger("Run");

        float elapsedTime = 0f;
        Vector3 startPosition = mouseObject.transform.position;
        Vector3 controlPoint = (startPosition + mouseTargetPoint.position) / 2 + new Vector3(0, 2, 0);

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * moveSpeed;
            mouseObject.transform.position = GetQuadraticBezierPoint(startPosition, controlPoint, mouseTargetPoint.position, elapsedTime);
            yield return null;
        }

        mouseObject.transform.position = mouseTargetPoint.position;
        mouseAnimator.SetTrigger("Idle");
        yield return new WaitForSeconds(1f);

        if (!isCatMoving)
        {
            StartCoroutine(MoveCatToMouse());
        }
    }

    private IEnumerator MoveCatToMouse()
    {
        isCatMoving = true;
        catAnimator.SetTrigger(catSpriteRenderer.sprite == catThinSprite ? "WalkThin" : "WalkFat");

        float elapsedTime = 0f;
        Vector3 startPosition = catObject.position;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * moveSpeed;
            catObject.position = Vector3.Lerp(startPosition, catTargetPoint.position, elapsedTime);
            yield return null;
        }

        catObject.position = catTargetPoint.position;
        catAnimator.SetTrigger("Idle");

        rewardItem.SetActive(true);
        hasMinigameCompleted = true;

        if (gameEntryObject != null)
        {
            gameEntryObject.SetActive(false);
        }
    }

    private Vector3 GetQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
    }
}
