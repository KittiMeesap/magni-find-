using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMinigame : MonoBehaviour
{
    public static CameraMinigame Instance { get; private set; }

    [SerializeField] private GameObject cameraMinigame;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject batterySlot;
    [SerializeField] private GameObject oldBattery;
    [SerializeField] private GameObject newBattery;
    [SerializeField] private SpriteRenderer cameraScreen;
    [SerializeField] private List<Sprite> photos;
    public List<Sprite> Photos => photos;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject cameraFinish; // ✅ กล้องที่มีปุ่มกด (ใหม่)
    [SerializeField] private float fadeDuration = 1.0f;

    [SerializeField] private bool hasBatteryInserted = false;
    public bool HasBatteryInserted => hasBatteryInserted;
    [SerializeField] private bool oldBatteryRemoved = false;
    public bool OldBatteryRemoved => oldBatteryRemoved;
    private Vector3 initialOldBatteryPosition;
    private int currentPhotoIndex = 0;
    public int CurrentPhotoIndex => currentPhotoIndex;

    private bool isSliding = false;
    private bool isCameraTransitioning = false;

    // ✅ กำหนดขนาดของถ่าน (สามารถตั้งค่าได้ใน Inspector)
    [SerializeField] private Vector3 batterySize = new Vector3(1f, 1f, 1f);
    [SerializeField] private float sizeTolerance = 0.05f;
    [SerializeField] private float snapDistance = 0.5f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

        if (oldBattery != null)
        {
            initialOldBatteryPosition = oldBattery.transform.position;
        }

        cameraFinish.SetActive(false); // ✅ ซ่อนกล้องตัวใหม่ตอนเริ่มเกม
    }

    public void StartMinigame()
    {
        isSliding = false;
        isCameraTransitioning = false;
        MinigameManager.Instance.StartMinigame(cameraMinigame, cameraObject.transform, null, true);

        CheckOldBatteryState();
    }

    private void ShowCameraScreen()
    {
        cameraScreen.sprite = photos[currentPhotoIndex];
    }

    public void RemoveOldBattery()
    {
        if (!oldBatteryRemoved && oldBattery != null)
        {
            oldBatteryRemoved = true;
            StartCoroutine(FadeOutOldBattery());
        }
    }

    private IEnumerator FadeOutOldBattery()
    {
        SpriteRenderer oldBatteryRenderer = oldBattery.GetComponent<SpriteRenderer>();
        if (oldBatteryRenderer == null)
        {
            Destroy(oldBattery);
            oldBattery = null;
            yield break;
        }

        float duration = 0.8f; // ✅ กำหนดระยะเวลาในการเฟด
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            oldBatteryRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        Destroy(oldBattery);
        oldBattery = null;

        // ✅ หลังจากถ่านเก่าหายไปแล้ว ให้เฟดถ่านใหม่เข้ามา
        newBattery.SetActive(true);
        StartCoroutine(FadeInNewBattery());
    }

    private IEnumerator FadeInNewBattery()
    {
        SpriteRenderer newBatteryRenderer = newBattery.GetComponent<SpriteRenderer>();
        if (newBatteryRenderer == null) yield break;

        float duration = 0.8f; // ✅ ระยะเวลาเฟดเข้า
        float elapsedTime = 0f;
        newBatteryRenderer.color = new Color(1f, 1f, 1f, 0f); // ✅ เริ่มจากโปร่งใส

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            newBatteryRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }


    public void InsertNewBattery()
    {
        if (!hasBatteryInserted && oldBatteryRemoved && IsBatterySizeCorrect() && IsBatteryPositionCorrect())
        {
            hasBatteryInserted = true;
            newBattery.transform.position = batterySlot.transform.position;
            StartCoroutine(FadeToNewCamera()); // ✅ เปลี่ยนกล้อง
        }
        else
        {
            Debug.LogWarning("Battery insertion failed: Incorrect size.");
        }
    }

    // ✅ ตรวจสอบว่าถ่านถูกลากไปตำแหน่งที่ใส่ถ่านหรือไม่
    private bool IsBatteryPositionCorrect()
    {
        return Vector3.Distance(newBattery.transform.position, batterySlot.transform.position) <= snapDistance;
    }

    // ✅ ตรวจสอบว่าขนาดของถ่านถูกต้องหรือไม่
    private bool IsBatterySizeCorrect()
    {
        Vector3 batteryScale = newBattery.transform.localScale;
        return Mathf.Abs(batteryScale.x - batterySize.x) <= sizeTolerance &&
               Mathf.Abs(batteryScale.y - batterySize.y) <= sizeTolerance &&
               Mathf.Abs(batteryScale.z - batterySize.z) <= sizeTolerance;
    }

    private IEnumerator FadeToNewCamera()
    {
        if (isCameraTransitioning) yield break;
        isCameraTransitioning = true;

        // ✅ เฟดกล้องเก่า (รวมลูกๆ)
        yield return StartCoroutine(FadeOutWithChildren(cameraObject));

        cameraObject.SetActive(false);
        cameraFinish.SetActive(true);

        // ✅ เฟดกล้องใหม่ (รวมลูกๆ)
        yield return StartCoroutine(FadeInWithChildren(cameraFinish));

        isCameraTransitioning = false;
        ShowCameraScreen();
    }

    private IEnumerator FadeOutWithChildren(GameObject obj)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            }

            yield return null;
        }

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0f);
        }
    }

    private IEnumerator FadeInWithChildren(GameObject obj)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            }

            yield return null;
        }

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
        }
    }


    public void PauseMinigame()
    {
        MinigameManager.Instance.PauseMinigame();
    }

    public void ResumeMinigame()
    {
        MinigameManager.Instance.ResumeMinigame(cameraMinigame, cameraObject.transform, null, true);
    }

    private void CheckOldBatteryState()
    {
        if (oldBattery != null)
        {
            if (Vector3.Distance(oldBattery.transform.position, initialOldBatteryPosition) > 0.1f)
            {
                Destroy(oldBattery);
                oldBattery = null;
                oldBatteryRemoved = true;
                newBattery.SetActive(true);
            }
        }
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
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / (fadeDuration * 0.3f));
            cameraScreen.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        currentPhotoIndex += direction;
        cameraScreen.sprite = photos[currentPhotoIndex];

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration * 1f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / (fadeDuration * 0.3f));
            cameraScreen.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        isSliding = false;
    }

}
