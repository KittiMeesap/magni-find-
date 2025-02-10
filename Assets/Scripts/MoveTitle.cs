using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveTitle : MonoBehaviour
{
    public ToolManager toolManager; // ตัวจัดการเครื่องมือ
    public GameObject gameTitle; // โลโก้เกม
    public GameObject guideTutorial1; // Guide ตัวแรก
    public GameObject guideTutorial2; // Guide ตัวที่สอง
    public GameObject arrow; // ลูกศร
    public GameObject playMinigameObject; // ปุ่มเล่นมินิเกม (GameObject ปกติ)
    public RectTransform startButton; // **UI Button ที่ใช้ RectTransform**

    public float targetYPosition = 2f; // ตำแหน่งปลายทางของ gameTitle
    public float moveSpeed = 2f; // ความเร็วในการเคลื่อนที่

    // 🔥 ตั้งค่าความเร็วขยับขึ้นลงของ gameTitle
    public float floatAmount = 0.2f;
    public float floatSpeed = 1.5f;

    // 🔥 ตั้งค่าความเร็วขยับขึ้นลงของลูกศร (เร็วกว่า)
    public float arrowFloatAmount = 0.3f;
    public float arrowFloatSpeed = 3f;

    private bool isTitleMoving = false;
    private bool hasMoved = false;
    private bool isToolChanged = false;

    private Transform gameTitleTransform;
    private Transform guideTutorial1Transform;
    private Transform guideTutorial2Transform;
    private Transform arrowTransform;
    private Transform playMinigameTransform;
    private RectTransform startButtonRect;
    private Collider2D gameTitleCollider;

    void Start()
    {
        // ตั้งค่าให้เริ่มต้นในโหมด "Eye"
        toolManager.SetToolMode("Eye");

        // ดึง Transform ของ GameObjects
        gameTitleTransform = gameTitle.transform;
        guideTutorial1Transform = guideTutorial1.transform;
        guideTutorial2Transform = guideTutorial2.transform;
        arrowTransform = arrow.transform;
        playMinigameTransform = playMinigameObject.transform;
        startButtonRect = startButton.GetComponent<RectTransform>();
        gameTitleCollider = gameTitle.GetComponent<Collider2D>();

        if (gameTitleCollider == null)
        {
            Debug.LogError("❌ GameTitle ไม่มี Collider2D! กรุณาเพิ่ม Collider2D เข้าไป");
        }

        // ✅ ตั้งค่าเริ่มต้นให้ `guideTutorial2`, `playMinigameObject` และ `startButton` อยู่นอกจอ
        guideTutorial2Transform.position = new Vector3(15f, guideTutorial2Transform.position.y, guideTutorial2Transform.position.z);
        playMinigameTransform.position = new Vector3(-15f, playMinigameTransform.position.y, playMinigameTransform.position.z);
        startButtonRect.anchoredPosition = new Vector2(-1500f, startButtonRect.anchoredPosition.y);

        // ✅ เริ่มอนิเมชันขยับขึ้นลง
        StartCoroutine(FloatAnimation(gameTitleTransform, floatSpeed, floatAmount));
        StartCoroutine(FloatAnimation(arrowTransform, arrowFloatSpeed, arrowFloatAmount)); // 🔥 ลูกศรขยับเร็วขึ้น
    }

    private IEnumerator FloatAnimation(Transform obj, float speed, float amount)
    {
        Vector3 startPos = obj.position;

        while (!hasMoved)
        {
            obj.position = startPos + new Vector3(0, Mathf.Sin(Time.time * speed) * amount, 0);
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTitleMoving)
        {
            if (IsMouseOverGameTitle())
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Hand);
                isTitleMoving = true;
                hasMoved = true;
                StartCoroutine(ProceedToNextStep());
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            ToggleToolMode();
        }
    }

    private bool IsMouseOverGameTitle()
    {
        if (gameTitleCollider == null) return false;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return gameTitleCollider.OverlapPoint(mousePos);
    }

    private IEnumerator ProceedToNextStep()
    {
        StopAllCoroutines(); // ✅ หยุดอนิเมชันก่อนเลื่อนขึ้น

        StartCoroutine(MoveObjectToPosition(gameTitleTransform, new Vector3(gameTitleTransform.position.x, targetYPosition, gameTitleTransform.position.z), 1f));
        StartCoroutine(MoveObjectToPosition(guideTutorial1Transform, new Vector3(15f, guideTutorial1Transform.position.y, guideTutorial1Transform.position.z), 1f));
        StartCoroutine(MoveObjectToPosition(arrowTransform, new Vector3(-15f, arrowTransform.position.y, arrowTransform.position.z), 1f));

        StartCoroutine(MoveObjectToPosition(guideTutorial2Transform, new Vector3(4.55f, guideTutorial2Transform.position.y, guideTutorial2Transform.position.z), 1f));
        StartCoroutine(MoveObjectToPosition(playMinigameTransform, new Vector3(-4f, playMinigameTransform.position.y, playMinigameTransform.position.z), 1f));
        StartCoroutine(MoveUIToPosition(startButtonRect, new Vector2(-400f, startButtonRect.anchoredPosition.y), 1f));

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator MoveObjectToPosition(Transform obj, Vector3 targetPos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = obj.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            obj.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            yield return null;
        }

        obj.position = targetPos;
        isToolChanged = true;
        toolManager.SetToolMode("Hand");
    }

    private IEnumerator MoveUIToPosition(RectTransform uiElement, Vector2 targetPos, float duration)
    {
        float elapsedTime = 0f;
        Vector2 startPos = uiElement.anchoredPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            uiElement.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            yield return null;
        }

        uiElement.anchoredPosition = targetPos;
    }

    private void ToggleToolMode()
    {
        if (isToolChanged)
        {
            if (toolManager.CurrentMode == "Hand")
            {
                toolManager.SetToolMode("Magnifier");
            }
            else if (toolManager.CurrentMode == "Magnifier")
            {
                toolManager.SetToolMode("Hand");
            }
        }
    }
}
