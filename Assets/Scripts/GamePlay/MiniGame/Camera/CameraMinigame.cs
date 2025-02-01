using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMinigame : MonoBehaviour
{
    public static CameraMinigame Instance { get; private set; }

    [SerializeField] private GameObject cameraMinigame;
    [SerializeField] private Transform cameraObject;
    [SerializeField] private GameObject batterySlot;
    [SerializeField] private GameObject oldBattery;
    [SerializeField] private GameObject newBattery;
    [SerializeField] private SpriteRenderer cameraScreen;
    [SerializeField] private List<Sprite> photos;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private float fadeDuration = 1.0f;

    private bool hasBatteryInserted = false; // ✅ จำว่าถ่านใหม่ถูกใส่ไปแล้วหรือยัง
    private bool oldBatteryRemoved = false; // ✅ จำว่าถ่านเก่าถูกถอดออกไปหรือยัง
    private Vector3 initialOldBatteryPosition; // ✅ บันทึกตำแหน่งเดิมของถ่านเก่า
    private int currentPhotoIndex = 0;
    private bool isSliding = false;

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

        if (oldBattery != null)
        {
            initialOldBatteryPosition = oldBattery.transform.position; // ✅ บันทึกตำแหน่งถ่านเก่าตอนเริ่มเกม
        }
    }

    public void StartMinigame()
    {
        isSliding = false;
        MinigameManager.Instance.StartMinigame(cameraMinigame, cameraObject, null, true);

        CheckOldBatteryState(); // ✅ เช็คว่าถ่านเก่ายังอยู่ที่เดิมไหม

        if (hasBatteryInserted)
        {
            // ✅ ถ้าถ่านใหม่ถูกใส่แล้ว แสดงผลกล้องได้เลย
            cameraScreen.color = Color.white;
            cameraScreen.sprite = photos[currentPhotoIndex];
            newBattery.SetActive(false);
            if(oldBattery != null)
            {
                oldBattery.SetActive(false);
            }
            leftButton.SetActive(true);
            rightButton.SetActive(true);
        }
        else
        {
            // ✅ ถ้ายังไม่ใส่ถ่านใหม่ ให้แสดงสถานะเริ่มต้น
            cameraScreen.color = Color.black;
            cameraScreen.sprite = null;
            leftButton.SetActive(false);
            rightButton.SetActive(false);
        }
    }

    public void RemoveOldBattery()
    {
        if (!oldBatteryRemoved)
        {
            if (oldBattery != null)
            {
                oldBatteryRemoved = true;
                Destroy(oldBattery); // ✅ ถ่านเก่าถูกย้ายออก -> ลบทิ้งจริง ๆ
                oldBattery = null;
                newBattery.SetActive(true); // ✅ แสดงถ่านใหม่ให้ใช้งาน
                Debug.Log("Old battery removed, new battery available.");
            }
        }
    }

    public void InsertNewBattery()
    {
        if (!hasBatteryInserted && oldBatteryRemoved && IsBatteryInCorrectPosition())
        {
            hasBatteryInserted = true; // ✅ จำว่าถ่านถูกใส่ไปแล้ว
            newBattery.transform.position = batterySlot.transform.position;
            newBattery.SetActive(false);
            StartCoroutine(FadeInCameraScreen());
        }
    }

    private bool IsBatteryInCorrectPosition()
    {
        float distance = Vector3.Distance(newBattery.transform.position, batterySlot.transform.position);
        return distance < 0.5f;
    }

    [SerializeField] private float fadeSpeed = 2.5f;

    private IEnumerator FadeInCameraScreen()
    {
        cameraScreen.sprite = photos[currentPhotoIndex];

        float adjustedFadeDuration = fadeDuration * 0.6f;
        float elapsedTime = 0f;

        while (elapsedTime < adjustedFadeDuration)
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            float alpha = Mathf.SmoothStep(0f, 1f, elapsedTime / adjustedFadeDuration);
            cameraScreen.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        cameraScreen.color = Color.white;

        leftButton.SetActive(true);
        rightButton.SetActive(true);
    }

    public void PauseMinigame()
    {
        MinigameManager.Instance.PauseMinigame();
    }

    public void ResumeMinigame()
    {
        MinigameManager.Instance.ResumeMinigame(cameraMinigame, cameraObject, null,true);
    }

    private void CheckOldBatteryState()
    {
        if (oldBattery != null)
        {
            float distance = Vector3.Distance(oldBattery.transform.position, initialOldBatteryPosition);
            if (distance > 0.1f) // ✅ ถ่านเก่าไม่อยู่ตำแหน่งเดิม
            {
                Debug.Log("Old battery moved! Removing it.");
                Destroy(oldBattery);
                oldBattery = null;
                oldBatteryRemoved = true;
                newBattery.SetActive(true); // ✅ เปิดให้ใช้ถ่านใหม่ทันที
            }
        }

        Debug.Log($"Battery state restored. OldBatteryRemoved: {oldBatteryRemoved}, HasBatteryInserted: {hasBatteryInserted}");
    }

    public void NextPhoto()
    {
        if (currentPhotoIndex < photos.Count - 1 && !isSliding)
        {
            StartCoroutine(FadeToNextPhoto(1));
        }
    }

    public void PreviousPhoto()
    {
        if (currentPhotoIndex > 0 && !isSliding)
        {
            StartCoroutine(FadeToNextPhoto(-1));
        }
    }

    private IEnumerator FadeToNextPhoto(int direction)
    {
        if (isSliding) yield break;
        isSliding = true;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration * 0.3f)
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            float alpha = Mathf.SmoothStep(1f, 0f, elapsedTime / (fadeDuration * 0.3f));
            cameraScreen.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        currentPhotoIndex += direction;
        cameraScreen.sprite = photos[currentPhotoIndex];

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration * 0.3f)
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            float alpha = Mathf.SmoothStep(0f, 1f, elapsedTime / (fadeDuration * 0.3f));
            cameraScreen.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        isSliding = false;
    }
}
