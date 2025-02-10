using UnityEngine;
using System.Collections;

public class TurntableMinigame : MonoBehaviour
{
    public static TurntableMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject minigameObject;
    [SerializeField] private GameObject vinylDiscAll;
    [SerializeField] private GameObject vinylDisc;
    [SerializeField] private GameObject turntable;
    [SerializeField] private GameObject speaker;
    [SerializeField] private GameObject powerButton;
    [SerializeField] private GameObject soundSymbol;
    [SerializeField] private AudioClip minigameBGM;
    [SerializeField] private Transform needle;
    [SerializeField] private DialogueSystem dialogue;

    private bool isOn = false;
    public bool IsOn => isOn;
    private bool isMinigameComplete = false;

    public bool CanInsertVinyl { get; private set; } = true;
    public bool CanPressPower { get; private set; } = false;

    private bool hasNeedleMoved = false;
    private bool isSpinning = false;
    private float spinSpeed = 180f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        soundSymbol.SetActive(false);
    }

    private void Update()
    {
        if (isSpinning)
        {
            vinylDisc.transform.Rotate(Vector3.forward, -spinSpeed * Time.deltaTime);
        }
    }

    public void StartMinigame()
    {
        MinigameManager.Instance.StartMinigame(minigameObject, turntable.transform, null, true);
        MinigameManager.Instance.SetMinigameTrigger(triggerObject);
    }

    public void InsertVinyl()
    {
        if (!CanInsertVinyl || isOn) return;

        CanInsertVinyl = false;
        CanPressPower = true;
        Debug.Log("แผ่นเสียงถูกใส่เข้าไปแล้ว!");

        // ✅ ย้ายแผ่นเสียงไปที่ตำแหน่ง x = 2 และขยาย Scale เป็น 1.2
        StartCoroutine(MoveAndScaleObject(vinylDiscAll.transform, new Vector3(2f, vinylDiscAll.transform.position.y, vinylDiscAll.transform.position.z), new Vector3(1.2f, 1.2f, 1.2f)));
    }

    private IEnumerator MoveAndScaleObject(Transform obj, Vector3 targetPosition, Vector3 targetScale)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 startPosition = obj.position;
        Vector3 startScale = obj.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            obj.position = Vector3.Lerp(startPosition, targetPosition, t);
            obj.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        obj.position = targetPosition;
        obj.localScale = targetScale;
    }

    public void TurnOn()
    {
        if (!CanPressPower || isOn) return;

        isOn = true;
        Debug.Log("เครื่องเล่นเปิดแล้ว!");
        soundSymbol.SetActive(true);
        Speaker.Instance.EnableScaling(true);
        needle.GetComponent<SpriteRenderer>().sortingOrder += 2;
        StartCoroutine(StartTurntableSequence());
    }

    private IEnumerator StartTurntableSequence()
    {
        // ✅ หมุนเข็มก่อน
        yield return StartCoroutine(MoveNeedle(true));

        // ✅ เริ่มหมุนแผ่นเสียง
        isSpinning = true;

        // ✅ เล่นเพลงของมินิเกม
        SoundManager.Instance.PlayMinigameBGM(minigameBGM, Speaker.Instance.GetVolumeMultiplier());

        DialogueUI.Instance.DialogueButton(false);
    }

    private IEnumerator MoveNeedle(bool moveDown)
    {
        DialogueUI.Instance.DialogueButton(false);
        float duration = 0.5f;
        float elapsedTime = 0f;
        Quaternion startRotation = needle.rotation;
        Quaternion targetRotation = moveDown ? Quaternion.Euler(0, 0, -35f) : Quaternion.identity;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            needle.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            yield return null;
        }

        needle.rotation = targetRotation;
        dialogue.dialogueText = "\"It’s still working fine, but the sound seems a bit low.\"";
        DialogueUI.Instance.DialogueUpdate(dialogue.dialogueText);
    }

    public void TurnOff()
    {
        if (!isOn) return;

        isOn = false;
        Debug.Log("เครื่องเล่นปิดแล้ว!");
        Speaker.Instance.EnableScaling(false);

        StartCoroutine(StopTurntableSequence());
    }

    private IEnumerator StopTurntableSequence()
    {
        // ✅ หยุดแผ่นเสียงก่อน
        isSpinning = false;

        // ✅ หยุดเสียงเพลง
        SoundManager.Instance.StopMinigameBGM();

        // ✅ หมุนเข็มกลับที่เดิม
        yield return StartCoroutine(MoveNeedle(false));

        DialogueUI.Instance.DialogueButton(true);
    }

    public void CompleteMinigame()
    {
        if (isMinigameComplete) return;
        isMinigameComplete = true;
        Debug.Log("มินิเกมเครื่องเสียงสำเร็จ!");
    }
}
