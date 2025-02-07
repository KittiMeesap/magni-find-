using UnityEngine;
using System.Collections;

public class TurntableMinigame : MonoBehaviour
{
    public static TurntableMinigame Instance { get; private set; }

    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject minigameObject;
    [SerializeField] private GameObject vinylDisc;
    [SerializeField] private GameObject turntable;
    [SerializeField] private GameObject speaker;
    [SerializeField] private GameObject powerButton;
    [SerializeField] private GameObject soundSymbol;

    private bool isOn = false;
    private bool isMinigameComplete = false;

    public bool CanInsertVinyl { get; private set; } = true;
    public bool CanPressPower { get; private set; } = false;

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

        // ✅ ซ่อนเสียงและรางวัลตอนเริ่มเกม
        soundSymbol.SetActive(false);
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
    }

    public void TurnOn()
    {
        if (!CanPressPower || isOn) return;

        isOn = true;
        Debug.Log("เครื่องเล่นเปิดแล้ว!");
        soundSymbol.SetActive(true);
        Speaker.Instance.EnableScaling(true);
    }

    public void TurnOff()
    {
        if (!isOn) return;

        isOn = false;
        Debug.Log("เครื่องเล่นปิดแล้ว!");
        soundSymbol.SetActive(false);
        Speaker.Instance.EnableScaling(false);
    }

    public void CompleteMinigame()
    {
        if (isMinigameComplete) return;
        isMinigameComplete = true;

        Debug.Log("มินิเกมเครื่องเสียงสำเร็จ!");
    }
}
