using UnityEngine;
using System.Collections;


public class InteractableObjectPlayMinigame : MonoBehaviour
{
    public float snapDistance = 0.5f; // ✅ ระยะห่างสำหรับ Snap
    public Vector3 correctScale; // ✅ สเกลที่ถูกต้องเพื่อให้ Snap ได้
    public float scaleStep = 0.2f; // ✅ ปรับขนาดต่อครั้ง
    public float scaleSpeed = 5f; // ✅ ความเร็วในการเปลี่ยนขนาด (Lerp)
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // ✅ ขนาดต่ำสุด
    public Vector3 maxScale = new Vector3(3f, 3f, 3f); // ✅ ขนาดสูงสุด
    public float scaleTolerance = 0.05f; // ✅ ค่าความคลาดเคลื่อนของขนาด

    private bool isDragging = false;
    private bool isSnapped = false; // ✅ ห้ามดึงออกหลัง Snap
    private bool isSelected = false; // ✅ เช็คว่าถูกเลือกอยู่ไหม
    private Vector3 offset;
    private Vector3 targetScale; // ✅ ใช้ Lerp ให้ขยาย/ย่อ Smooth
    private Transform targetObject; // ✅ เป้าหมายที่ต้อง Snap (ดึงจาก `PlayMinigame`)

    private void Start()
    {
        targetScale = transform.localScale; // ✅ ตั้งค่าขนาดเริ่มต้น

        // ✅ ดึง `playShadowTransform` จาก `PlayMinigame.Instance`
        if (PlayMinigame.Instance != null)
        {
            targetObject = PlayMinigame.Instance.playShadowTransform;
        }
    }

    private void Update()
    {
        // ✅ ทำให้ขนาดเปลี่ยนแบบ Smooth
        if (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        // ✅ ตรวจจับการกดขยาย/ย่อเมื่ออยู่ในโหมด Magnifier
        if (isSelected && ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (Input.GetMouseButtonDown(0)) // ✅ คลิกซ้ายเพื่อขยาย
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Upsize);
                ModifyScale(scaleStep);
            }
            else if (Input.GetMouseButtonDown(1)) // ✅ คลิกขวาเพื่อลดขนาด
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Smallsize);
                ModifyScale(-scaleStep);
            }
        }
    }

    private void OnMouseDown()
    {
        if (isSnapped || Time.timeScale == 0f) return;

        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Hand);
            isDragging = true;
            offset = transform.position - GetMouseWorldPos();
            isSelected = true;
        }
        else if (ToolManager.Instance.CurrentMode == "Magnifier")
        {
            isSelected = true;
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        transform.position = GetMouseWorldPos() + offset;

        // ✅ ตรวจสอบ Snap ระหว่างลาก (ทั้งตำแหน่งและขนาด)
        if (!isSnapped && targetObject != null && CanSnap())
        {
            SnapToTarget();
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void ModifyScale(float scaleChange)
    {
        Vector3 newScale = targetScale + Vector3.one * scaleChange;

        // ✅ จำกัดขนาดให้อยู่ระหว่าง Min และ Max
        newScale.x = Mathf.Clamp(newScale.x, minScale.x, maxScale.x);
        newScale.y = Mathf.Clamp(newScale.y, minScale.y, maxScale.y);
        newScale.z = Mathf.Clamp(newScale.z, minScale.z, maxScale.z);

        targetScale = newScale; // ✅ ใช้ Lerp ให้ขยาย/ย่อ Smooth
    }

    private bool CanSnap()
    {
        // ✅ เช็คว่าระยะอยู่ในช่วง Snap หรือไม่
        bool isCloseEnough = Vector3.Distance(transform.position, targetObject.position) <= snapDistance;

        // ✅ เช็คว่าสเกลใกล้เคียง `correctScale` หรือไม่ (มี tolerance)
        bool isScaleCorrect = Vector3.Distance(transform.localScale, correctScale) <= scaleTolerance;

        return isCloseEnough && isScaleCorrect;
    }

    private void SnapToTarget()
    {
        transform.position = targetObject.position; // ✅ Snap ไปยังเป้าหมาย
        isSnapped = true;
        isDragging = false;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        PlayMinigame.Instance.CompleteMinigame(); // ✅ แจ้ง `PlayMinigame` ว่า Snap สำเร็จ
    }

    private bool isShaking = false; // ✅ ป้องกัน Shake ซ้อน
    public float shakeAngle = 5f; // ✅ องศาการสั่น (ไปทางซ้าย/ขวา)
    public float shakeSpeed = 15f; // ✅ ความเร็วในการสั่น


    // ✅ ทำให้ Object สั่นแบบหมุนเมื่อนำเมาส์ไปวาง
    private void OnMouseOver()
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeRotation());
        }
    }

    private void OnMouseExit()
    {
        isShaking = false; // ✅ หยุดการสั่นเมื่อเมาส์ออกจาก Object
    }

    private IEnumerator ShakeRotation()
    {
        isShaking = true;
        Quaternion originalRotation = transform.rotation;

        while (isShaking)
        {
            float angle = Random.Range(-shakeAngle, shakeAngle);
            Quaternion shakeRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, shakeRotation, Time.deltaTime * shakeSpeed);

            yield return new WaitForSeconds(1f / shakeSpeed);
        }

        transform.rotation = originalRotation; // ✅ กลับสู่ตำแหน่งเดิมเมื่อหยุดสั่น
    }

}
