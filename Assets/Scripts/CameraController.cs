using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField] private Camera mainCamera;

    public float moveSpeed = 5f;
    public float edgeThreshold = 50f;
    public Transform background;
    public float defaultCameraSize = 5f;

    private float minX, maxX;
    private float halfScreenWidth;
    private Vector3 lastCameraPosition;
    private float lastCameraSize;
    private Vector3 zoomedCameraPosition;
    private float zoomedCameraSize;
    private bool isMinigameActive = false;
    private bool isZooming = false;

    public bool IsMinigameActive => isMinigameActive;
    public bool IsZooming => isZooming;

    [Header("Zoom Setting")]
    public bool enableZoom = true;
    [Range(0f, 1f)] public float zoomPercentage = 1.0f;
    [SerializeField] private float duration = 1f;
    public float zoomSpeed = 1f;
    public float zoomPadding = 1.2f;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        defaultCameraSize = mainCamera.orthographicSize;

        if (background == null)
        {
            Debug.LogError("Background is not assigned to CameraController!");
            return;
        }

        float screenRatio = (float)Screen.width / Screen.height;
        halfScreenWidth = Camera.main.orthographicSize * screenRatio;

        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
        if (bgRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the background object!");
            return;
        }

        float bgMinX = background.position.x - bgRenderer.bounds.extents.x;
        float bgMaxX = background.position.x + bgRenderer.bounds.extents.x;

        minX = bgMinX + halfScreenWidth;
        maxX = bgMaxX - halfScreenWidth;
    }

    private void Update()
    {
        if (isMinigameActive || isZooming) return;

        Vector3 mousePos = Input.mousePosition;
        float screenWidth = Screen.width;
        float moveDirection = 0;

        if (mousePos.x < edgeThreshold) moveDirection = -1;
        else if (mousePos.x > screenWidth - edgeThreshold) moveDirection = 1;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveDirection = -1;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveDirection = 1;

        if (moveDirection != 0)
        {
            MoveCamera(moveDirection);
        }
    }

    private void MoveCamera(float direction)
    {
        float newX = transform.position.x + direction * moveSpeed * Time.deltaTime;
        newX = Mathf.Clamp(newX, minX, maxX);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    // ✅ **ซูมเข้าไปที่ Object ตามเปอร์เซ็นต์ที่ตั้ง**
    public void ZoomToObject(Transform target, System.Action onComplete)
    {
        if (isZooming) return;

        lastCameraPosition = transform.position;
        lastCameraSize = mainCamera.orthographicSize;

        if (!enableZoom)
        {
            transform.position = new Vector3(lastCameraPosition.x, lastCameraPosition.y, transform.position.z); // ✅ คงค่า Z
            mainCamera.orthographicSize = defaultCameraSize;
            onComplete?.Invoke();
            return;
        }

        // ✅ **ดึงค่ากลางของ Object**
        SpriteRenderer targetRenderer = target.GetComponent<SpriteRenderer>();
        Vector3 targetCenter = targetRenderer != null ? targetRenderer.bounds.center : target.position;

        // ✅ **คำนวณขนาดที่ต้องซูมโดยใช้เปอร์เซ็นต์**
        float maxZoomSize = CalculateZoomSize(targetRenderer);
        float zoomDifference = maxZoomSize - defaultCameraSize;
        zoomedCameraSize = defaultCameraSize + (zoomDifference * zoomPercentage);

        // ✅ **กำหนดตำแหน่งโดยใช้ค่า Zoom Percentage (ไม่ยุ่งกับ Z)**
        Vector2 directionToTarget = (new Vector2(targetCenter.x, targetCenter.y) - new Vector2(lastCameraPosition.x, lastCameraPosition.y)).normalized;
        float distanceToMove = Vector2.Distance(new Vector2(lastCameraPosition.x, lastCameraPosition.y), new Vector2(targetCenter.x, targetCenter.y)) * zoomPercentage;

        zoomedCameraPosition = new Vector3(
            lastCameraPosition.x + (directionToTarget.x * distanceToMove),
            lastCameraPosition.y + (directionToTarget.y * distanceToMove),
            lastCameraPosition.z // ✅ คงค่า Z เดิมไว้
        );

        isZooming = true;
        StartCoroutine(MoveAndZoomCoroutine(zoomedCameraPosition, zoomedCameraSize, () =>
        {
            isZooming = false;
            onComplete?.Invoke();
        }));
    }


    // ✅ **เข้า Minigame → กล้องตัดไปที่ `(0,0)` แต่ไม่เซฟค่ากล้องใหม่**
    public void EnterMinigame()
    {
        if (isMinigameActive) return;
        isMinigameActive = true;

        transform.position = new Vector3(0, 15, -10);
        mainCamera.orthographicSize = defaultCameraSize;
    }

    // ✅ **ออกจาก Minigame → กลับไปที่ซูมเข้าไป แล้วค่อยซูมออก**
    public void ExitMinigame()
    {
        if (!isMinigameActive) return;
        isMinigameActive = false;

        transform.position = zoomedCameraPosition;
        mainCamera.orthographicSize = zoomedCameraSize;

        if (!enableZoom)
        {
            // ✅ ถ้า enableZoom = false → ตัดไปตำแหน่งเดิมทันที
            transform.position = lastCameraPosition;
            mainCamera.orthographicSize = lastCameraSize;
            ToolManager.Instance.SetToolMode("Eye");
            return;
        }

        isZooming = true;
        StartCoroutine(MoveAndZoomCoroutine(lastCameraPosition, lastCameraSize, () =>
        {
            isZooming = false;
            ToolManager.Instance.SetToolMode("Eye");
        }));
    }

    // ✅ **ฟังก์ชันคำนวณระยะซูมให้เห็น Object พอดี**
    private float CalculateZoomSize(SpriteRenderer targetRenderer)
    {
        if (targetRenderer == null) return lastCameraSize;

        float objectHeight = targetRenderer.bounds.size.y;
        float objectWidth = targetRenderer.bounds.size.x;

        float screenRatio = (float)Screen.width / Screen.height;
        float zoomByHeight = (objectHeight / 2) * zoomPadding;
        float zoomByWidth = ((objectWidth / 2) / screenRatio) * zoomPadding;

        return Mathf.Max(zoomByHeight, zoomByWidth);
    }

    private IEnumerator MoveAndZoomCoroutine(Vector3 targetPosition, float zoomSize, System.Action onComplete)
    {
        if (!enableZoom)
        {
            transform.position = targetPosition;
            mainCamera.orthographicSize = zoomSize;
            onComplete?.Invoke();
            yield break;
        }

        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        float startSize = mainCamera.orthographicSize;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / duration);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, zoomSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime * zoomSpeed;
            yield return null;
        }

        transform.position = targetPosition;
        mainCamera.orthographicSize = zoomSize;
        onComplete?.Invoke();
    }
}
