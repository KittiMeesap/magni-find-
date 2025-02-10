using UnityEngine;
using System.Collections;

public class CatMinigame : MonoBehaviour
{
    public static CatMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject catMinigameObject;
    [SerializeField] private GameObject rewardItem; // ✅ รางวัล
    [SerializeField] private GameObject cat;
    [SerializeField] private GameObject mouse;
    [SerializeField] private Transform mouseTargetPosition;
    [SerializeField] private Transform catFollowTarget;
    [SerializeField] private GameObject mouseBody;
    [SerializeField] private GameObject mouseLegs;
    [SerializeField] private GameObject fatCatEyes;
    [SerializeField] private GameObject slimCatEyes;
    [SerializeField] private Sprite newMouseSprite; // ✅ Sprite หนูที่ตั้งไว้

    [SerializeField] private Vector3 correctMouseScale; // ✅ ขนาดที่หนูต้องย่อเอง
    private bool isMinigameComplete = false;
    private bool isMouseScaling = false;
    private bool isMouseRunning = false;
    private bool isCatFollowing = false;
    private bool isMouseCorrectSize = false;
    public bool IsMouseRunning => isMouseRunning;

    [SerializeField] private InteractObject interactObject;
    [SerializeField] private DialogueSystem dialogue;

    [SerializeField] private AudioClip sfx_Mouse;
    [SerializeField] private AudioClip sfx_MouseRun;
    [SerializeField] private AudioClip sfx_CatRun;


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

        rewardItem.SetActive(false);
        mouseBody.SetActive(false);
        mouseLegs.SetActive(false);
        fatCatEyes.SetActive(false);
        slimCatEyes.SetActive(false);
    }

    public void StartMinigame()
    {
        MinigameManager.Instance.StartMinigame(catMinigameObject, cat.transform, rewardItem, true);
        MinigameManager.Instance.SetMinigameTrigger(triggerObject);
    }

    public void SetCatState(string state)
    {
        if (state == "fat")
        {
            fatCatEyes.SetActive(true);
            slimCatEyes.SetActive(false);
        }
        else if (state == "thin")
        {
            slimCatEyes.SetActive(true);
            fatCatEyes.SetActive(false);
        }
    }

    public IEnumerator FadeInMouse()
    {
        SoundManager.Instance.PlaySFX(sfx_Mouse);
        DialogueUI.Instance.DialogueButton(false);
        mouse.SetActive(true);
        SpriteRenderer mouseSR = mouse.GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;
        float fadeDuration = 1f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            mouseSR.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        DialogueUI.Instance.DialogueButton(true);
    }

    public void CheckMouseSize()
    {
        if (Vector3.Distance(mouse.transform.localScale, correctMouseScale) <= 0.05f)
        {
            isMouseCorrectSize = true;
            StartCoroutine(PrepareMouseToRun());
        }
        else
        {
            isMouseCorrectSize = false;
        }
    }

    private IEnumerator PrepareMouseToRun()
    {
        DialogueUI.Instance.DialogueButton(false);
        yield return new WaitForSeconds(0.5f);

        // ✅ เปลี่ยน Sprite ของหนูเป็นที่ตั้งไว้
        SpriteRenderer mouseRenderer = mouse.GetComponent<SpriteRenderer>();
        if (mouseRenderer != null && newMouseSprite != null)
        {
            mouseRenderer.sprite = newMouseSprite;
        }

        mouseBody.SetActive(true);
        mouseLegs.SetActive(true);

        // ✅ เฟดดวงตาแมวให้ปรากฏตอนหนูวิ่ง
        StartCoroutine(FadeInCatEyes());

        StartCoroutine(MoveMouseToTarget());
    }

    private IEnumerator FadeInCatEyes()
    {
        SpriteRenderer eyeRenderer = null;

        if (fatCatEyes.activeSelf)
        {
            eyeRenderer = fatCatEyes.GetComponent<SpriteRenderer>();
        }
        else if (slimCatEyes.activeSelf)
        {
            eyeRenderer = slimCatEyes.GetComponent<SpriteRenderer>();
        }

        if (eyeRenderer == null) yield break;

        float elapsedTime = 0f;
        float fadeDuration = 1f;
        Color startColor = new Color(1f, 1f, 1f, 0f); // ✅ เริ่มจากโปร่งใส
        Color targetColor = new Color(1f, 1f, 1f, 1f); // ✅ เปลี่ยนเป็น 100%

        eyeRenderer.color = startColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            eyeRenderer.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator MoveMouseToTarget()
    {
        SoundManager.Instance.PlaySFX(sfx_MouseRun);
        isMouseRunning = true;
        float moveDuration = 2f;
        float elapsedTime = 0f;
        Vector3 startPos = mouse.transform.position;

        // ✅ เปลี่ยนเส้นโค้งของหนูให้ตรงข้าม (น้ำเงิน)
        Vector3 controlPoint = startPos + new Vector3(2f, -2f, 0); // 🔄 โค้งไปฝั่งตรงข้าม

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            mouse.transform.position = BezierCurve(startPos, controlPoint, mouseTargetPosition.position, t);
            mouseLegs.transform.Rotate(0, 0, -5f);
            yield return null;
        }

        isMouseRunning = false;
        isCatFollowing = true;
        StartCoroutine(MoveCatToMouse());
    }

    private IEnumerator MoveCatToMouse()
    {
        SoundManager.Instance.PlaySFX(sfx_CatRun);
        float moveDuration = 2f;
        float elapsedTime = 0f;
        Vector3 startPos = cat.transform.position;

        // ✅ แมวยังคงใช้เส้นโค้งแบบเดิม (แดง)
        Vector3 controlPoint = startPos + new Vector3(-1f, -1f, 0);

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            cat.transform.position = BezierCurve(startPos, controlPoint, catFollowTarget.position, t);
            yield return null;
        }

        interactObject.CheckMinigameDone();
        isMinigameComplete = true;
        StartCoroutine(FadeInReward());
        dialogue.dialogueText = "\"I hope that mouse makes it, but I'll take this one for now.\"";
        DialogueUI.Instance.DialogueUpdate(dialogue.dialogueText);
    }


    private IEnumerator FadeInReward()
    {
        rewardItem.SetActive(true);
        SpriteRenderer rewardRenderer = rewardItem.GetComponent<SpriteRenderer>();

        if (rewardRenderer == null) yield break;

        float elapsedTime = 0f;
        float fadeDuration = 1f;
        Color startColor = new Color(1f, 1f, 1f, 0f); // ✅ เริ่มจากโปร่งใส
        Color targetColor = new Color(1f, 1f, 1f, 1f); // ✅ เปลี่ยนเป็น 100%

        rewardRenderer.color = startColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            rewardRenderer.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }

        DialogueUI.Instance.DialogueButton(true);

    }

    private Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }
}
