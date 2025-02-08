using UnityEngine;
using System.Collections;

public class InteractableBird : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite readyToEatSprite;
    [SerializeField] private Sprite walkingSprite;
    [SerializeField] private Sprite eatingSprite;
    private Collider2D birdCollider;
    private Quaternion originalRotation;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        birdCollider = GetComponent<Collider2D>();
        originalRotation = transform.rotation; // ✅ เก็บ Rotation เดิมไว้
    }

    public void SetBirdState(string state)
    {
        switch (state)
        {
            case "idle":
                spriteRenderer.sprite = idleSprite;
                break;
            case "readyToEat":
                spriteRenderer.sprite = readyToEatSprite;
                break;
            case "walking":
                spriteRenderer.sprite = walkingSprite;
                break;
            case "eating":
                StartCoroutine(WaitForAppleToDisappear());
                break;
        }
    }

    private IEnumerator WaitForAppleToDisappear()
    {
        Debug.Log("WaitForAppleToDisappear");
        float tiltAngle = 20f; // ✅ หมุนไปซ้าย-ขวา 20 องศา
        float tiltDuration = 0.1f; // ✅ เวลาที่ใช้ในการโยกแต่ละครั้ง
        float totalDuration = 0.5f; // ✅ เวลารวมที่ต้องโยกหัว
        float elapsedTotalTime = 0f;

        Quaternion leftRotation = Quaternion.Euler(0, 0, tiltAngle);
        Quaternion rightRotation = Quaternion.Euler(0, 0, 0);

        while (elapsedTotalTime < totalDuration)
        {
            // ✅ โยกไปทางซ้าย 20°
            yield return RotateHead(rightRotation, leftRotation, tiltDuration);
            elapsedTotalTime += tiltDuration;
            if (elapsedTotalTime >= totalDuration) break;

            // ✅ โยกกลับมาทางขวา 0°
            yield return RotateHead(leftRotation, rightRotation, tiltDuration);
            elapsedTotalTime += tiltDuration;
        }

        // ✅ ค่อยๆ กลับ Rotation ไปเป็นปกติ
        yield return ResetRotation(0.3f);

        yield return new WaitForSeconds(0.3f); // ✅ รออีกนิด ก่อนเปลี่ยน sprite
        spriteRenderer.sprite = eatingSprite;
        StartCoroutine(DisableColliderAfterEating());
    }

    // ✅ ฟังก์ชันช่วยหมุนหัวนก
    private IEnumerator RotateHead(Quaternion from, Quaternion to, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(from, to, elapsedTime / duration);
            yield return null;
        }
        transform.rotation = to;
    }

    // ✅ ฟังก์ชันให้ Rotation ค่อยๆ กลับไปเป็นปกติ
    private IEnumerator ResetRotation(float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, originalRotation, elapsedTime / duration);
            yield return null;
        }
        transform.rotation = originalRotation;
    }

    private IEnumerator DisableColliderAfterEating()
    {
        yield return new WaitForSeconds(1f); // ✅ รอให้กินเสร็จก่อน
        if (birdCollider != null)
        {
            birdCollider.enabled = false; // ✅ ปิด Collider ของนกหลังจากกินเสร็จ
        }
    }
}
