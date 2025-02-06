using UnityEngine;
using System.Collections;

public class PictureSlot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer pictureRenderer;
    [SerializeField] private Sprite[] pictures; // รูปภาพทั้งหมดในช่อง
    [SerializeField] private int correctIndex; // รูปที่ถูกต้อง
    [SerializeField] private float slideDistance = 1.2f; // ระยะทางที่เลื่อน
    [SerializeField] private float slideSpeed = 8f; // ความเร็วของการเลื่อน

    private int currentIndex = 0;
    private bool isSliding = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isDragging = false;
    private float dragStartY;
    [SerializeField] private int nextIndex; // เก็บภาพถัดไปที่จะเปลี่ยน

    public delegate void SlotCorrect();
    public event SlotCorrect OnSlotCorrect;

    private void Awake()
    {
        pictureRenderer.sprite = pictures[currentIndex];
        startPosition = pictureRenderer.transform.localPosition;
    }

    private void OnMouseDown()
    {
        if (PictureMinigame.Instance.PictureMinigameDone) return;
        if (isSliding) return;
        isDragging = true;
        dragStartY = Input.mousePosition.y;
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        float dragAmount = Input.mousePosition.y - dragStartY;

        if (dragAmount > 50f) // ดึงลง → ภาพสไลด์ลง → เปลี่ยนเป็นภาพถัดไป
        {
            nextIndex = (currentIndex - 1 + pictures.Length) % pictures.Length; // เปลี่ยนไปภาพก่อนหน้า
            StartSlideAnimation(slideDistance); // สไลด์ลง
        }
        else if (dragAmount < -50f) // ดึงขึ้น → ภาพสไลด์ขึ้น → เปลี่ยนเป็นภาพก่อนหน้า
        {
            nextIndex = (currentIndex + 1) % pictures.Length; // เปลี่ยนไปภาพถัดไป
            StartSlideAnimation(-slideDistance); // สไลด์ขึ้น
        }
    }

    private void StartSlideAnimation(float moveDistance)
    {
        if (isSliding) return;
        targetPosition = startPosition + new Vector3(0, moveDistance, 0); // กำหนดตำแหน่งปลายทาง
        StartCoroutine(SlidePictureAnimation());
    }

    private IEnumerator SlidePictureAnimation()
    {
        isSliding = true;

        float elapsedTime = 0f;
        Vector3 initialPosition = pictureRenderer.transform.localPosition;

        while (elapsedTime < 1f / slideSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime * slideSpeed);
            pictureRenderer.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        pictureRenderer.transform.localPosition = startPosition;
        currentIndex = nextIndex; // อัปเดตภาพใหม่
        pictureRenderer.sprite = pictures[currentIndex];

        isSliding = false;

        if (IsCorrect() && OnSlotCorrect != null)
        {
            OnSlotCorrect.Invoke();
        }
    }

    public bool IsCorrect()
    {
        return currentIndex == correctIndex;
    }

    public void ResetSlot()
    {
        currentIndex = 0;
        pictureRenderer.sprite = pictures[currentIndex];
    }
}
