using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public float moveSpeed = 5f;
    public float edgeThreshold = 50f;
    public Transform background;
    public float zoomSpeed = 1f;
    public float zoomPadding = 1.2f;
    public float defaultCameraSize = 5f;

    private float minX, maxX;
    private float halfScreenWidth;
    private Vector3 lastCameraPosition;   // ✅ ตำแหน่งก่อนซูม
    private float lastCameraSize;         // ✅ ขนาดก่อนซูม
    private Vector3 zoomedCameraPosition; // ✅ ตำแหน่งที่ซูมเข้าไป
    private float zoomedCameraSize;       // ✅ ขนาดที่ซูมเข้าไป
    public bool isMinigameActive = false;
    public bool isZooming = false;
    [SerializeField] private float duration = 1f;

    private Camera mainCamera;

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

    // ✅ **ซูมเข้าไปที่ Object และบันทึกค่าก่อนเข้า Minigame**
    public void ZoomToObject(Transform target, System.Action onComplete)
    {
        if (isZooming) return;

        lastCameraPosition = transform.position;   // ✅ บันทึกค่าก่อนซูม
        lastCameraSize = mainCamera.orthographicSize;

        // ✅ **ดึงค่ากลางของ Object**
        SpriteRenderer targetRenderer = target.GetComponent<SpriteRenderer>();
        Vector3 targetCenter = targetRenderer != null ? targetRenderer.bounds.center : target.position;

        // ✅ **คำนวณขนาดที่ต้องซูม**
        float zoomSize = CalculateZoomSize(targetRenderer);

        // ✅ **กำหนดค่าตำแหน่งที่ซูมเข้าไป**
        zoomedCameraPosition = new Vector3(targetCenter.x, targetCenter.y, transform.position.z);
        zoomedCameraSize = zoomSize;

        isZooming = true;
        StartCoroutine(MoveAndZoomCoroutine(zoomedCameraPosition, zoomSize, () =>
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

        // ✅ **ตัดไปที่ `(0,0)` ทันที**
        transform.position = new Vector3(0, 0, transform.position.z);
        mainCamera.orthographicSize = defaultCameraSize;
    }

    // ✅ **ออกจาก Minigame → กลับไปที่ซูมเข้าไป แล้วค่อยซูมออก**
    public void ExitMinigame()
    {
        if (!isMinigameActive) return;
        isMinigameActive = false;

        transform.position = zoomedCameraPosition;
        mainCamera.orthographicSize = zoomedCameraSize;

        isZooming = true;
        StartCoroutine(MoveAndZoomCoroutine(lastCameraPosition, lastCameraSize, () =>
        {
            isZooming = false;
            ToolManager.Instance.SetToolMode("Eye"); // ✅ เรียกใช้หลังจากซูมออกเสร็จ
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
