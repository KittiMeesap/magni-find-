using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cat : MonoBehaviour
{
    [SerializeField] private GameObject ratObject; // Object หนูที่ถูกซ่อน
    [SerializeField] private Transform[] ratWaypoints; // จุดที่หนูจะเคลื่อนที่ผ่าน (เส้นโค้ง)
    [SerializeField] private float ratMoveSpeed = 3.0f; // ความเร็วของหนู
    [SerializeField] private Transform[] catWaypoints; // จุดที่แมวจะเคลื่อนที่ผ่าน (เส้นโค้ง)
    [SerializeField] private float catMoveSpeed = 2.0f; // ความเร็วของแมว
    [SerializeField] private Sprite BigCatSprite; // Sprite ใหม่ของแมวหลังหนูวิ่งเสร็จ
    [SerializeField] private Sprite SmallCatSprite; // Sprite ใหม่ของแมวหลังหนูวิ่งเสร็จ
    //[SerializeField] private bool shouldSpin = false; // ตัวแปรเช็คว่าต้องหมุนหรือไม่
    [SerializeField] private float spinDuration = 1.0f; // ระยะเวลาที่หมุน
    [SerializeField] private float curveSmoothness = 0.02f; // ค่าความโค้งของเส้น (0 - 1)

    private SpriteRenderer catRenderer;
    private bool isProcessing = false; // ป้องกันการเรียกซ้ำ
    //private InteractableObject interactableObject;
    private Vector3 previousRatScale; // เก็บค่า scale ก่อนหน้าของหนู

    private void Start()
    {
        if (ratObject != null)
        {
            ratObject.SetActive(false); // ซ่อนหนูไว้ก่อน
            previousRatScale = ratObject.transform.localScale; // เก็บค่า scale เริ่มต้นของหนู
        }

        catRenderer = GetComponent<SpriteRenderer>();
        //interactableObject = GetComponent<InteractableObject>(); // หา InteractableObject ที่อยู่บนแมว
    }

    private void Update()
    {
        // เมื่อขนาดของแมวเปลี่ยนแปลง ให้แสดงตัวหนูออกมา
        if (!isProcessing && transform.localScale.x != 1.0f)
        {
            if (ratObject != null) ratObject.SetActive(true); // แสดงหนูออกมา

            if (DFMaterial != null)
            {
                GetComponent<SpriteRenderer>().material = DFMaterial;

                Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
                foreach (var collider in colliders)
                {
                    collider.enabled = false; // Disable each collider
                }

            }
        }

        // เมื่อขนาดของหนูเปลี่ยนแปลง ให้เริ่มการวิ่ง
        if (ratObject != null && ratObject.transform.localScale != previousRatScale)
        {
            previousRatScale = ratObject.transform.localScale; // อัปเดตค่า scale
            StartCoroutine(HandleRatAndCatMovement());
            if (DFMaterial != null)
            {
                ratObject.GetComponent<SpriteRenderer>().material = DFMaterial;

                Collider2D[] colliders = ratObject.GetComponentsInChildren<Collider2D>();
                foreach (var collider in colliders)
                {
                    collider.enabled = false; // Disable each collider
                }
            }
        }
    }

    private IEnumerator HandleRatAndCatMovement()
    {
        isProcessing = true;

        if (ratObject != null && ratWaypoints.Length > 1)
        {
            yield return StartCoroutine(MoveAlongCurve(ratObject, ratWaypoints, ratMoveSpeed));
        }

        // เช็คขนาดของแมว
        if (transform.localScale.x == 0.9f)
        {
            if (catRenderer != null && SmallCatSprite != null)
            {
                catRenderer.sprite = SmallCatSprite; // เปลี่ยน Sprite ของแมว
            }

            StartCoroutine(MoveCatToPosition(false));
        }
        else if (transform.localScale.x == 1.1f)
        {
            if (catRenderer != null && BigCatSprite != null)
            {
                catRenderer.sprite = BigCatSprite; // เปลี่ยน Sprite ของแมว
            }

            StartCoroutine(MoveCatToPosition(true));
        }
    }

    [SerializeField] private float rotationSpeed = 720f; // ความเร็วในการหมุน (ตั้งค่าเองได้)
    [SerializeField] private Material DFMaterial; // Material สำหรับตอนหมุน

    private IEnumerator MoveCatToPosition(bool shouldRotate)
    {
        if (shouldRotate)
        {
            // หมุนตามค่า rotationSpeed เป็นเวลา spinDuration
            float elapsedTime = 0f;
            while (elapsedTime < spinDuration)
            {
                elapsedTime += Time.deltaTime;
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime); // ใช้ rotationSpeed ที่ตั้งไว้
                yield return null;
            }
            transform.rotation = Quaternion.identity; // รีเซ็ตการหมุน
        }

        if (catWaypoints.Length > 1)
        {
            List<Vector3> curvePoints = GenerateBezierCurve(catWaypoints, curveSmoothness);

            foreach (Vector3 targetPoint in curvePoints)
            {
                while (Vector3.Distance(transform.position, targetPoint) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPoint, catMoveSpeed * Time.deltaTime);

                    if (shouldRotate)
                    {
                        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime); // ใช้ rotationSpeed ที่กำหนดไว้
                    }

                    yield return null;
                }
            }

            // รีเซ็ตการหมุนหลังเคลื่อนที่เสร็จ
            if (shouldRotate)
            {
                transform.rotation = Quaternion.identity;
            }
        }

        // ปิด Collider หลังเดินเสร็จ
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // แสดง Reward Hidden Item จาก InteractableObject.cs
        /*if (interactableObject != null && interactableObject.hiddenItem != null)
        {
            StartCoroutine(FadeInHiddenItem(interactableObject.hiddenItem, 1f));
        }*/
    }

    private IEnumerator FadeInHiddenItem(GameObject hiddenItem, float duration)
    {
        SpriteRenderer spriteRenderer = hiddenItem.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break; // ถ้าไม่มี SpriteRenderer ก็ออกไปเลย

        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;
        startColor.a = 0f; // เริ่มจาก alpha = 0
        Color targetColor = spriteRenderer.color;
        targetColor.a = 1f; // ไปที่ alpha = 1

        spriteRenderer.color = startColor; // ตั้งค่าเริ่มต้น
        hiddenItem.SetActive(true); // เปิด Object

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        spriteRenderer.color = targetColor; // ตั้งค่า alpha ให้เต็ม 1
    }

    private IEnumerator MoveAlongCurve(GameObject obj, Transform[] waypoints, float speed)
    {
        Debug.Log($"Moving {obj.name} along the curve...");
        List<Vector3> curvePoints = GenerateBezierCurve(waypoints, curveSmoothness);

        foreach (Vector3 targetPoint in curvePoints)
        {
            while (Vector3.Distance(obj.transform.position, targetPoint) > 0.05f)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPoint, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private List<Vector3> GenerateBezierCurve(Transform[] controlPoints, float smoothness)
    {
        List<Vector3> curvePoints = new List<Vector3>();

        for (float t = 0; t <= 1; t += smoothness)
        {
            curvePoints.Add(CalculateBezierPoint(t, controlPoints));
        }

        return curvePoints;
    }

    private Vector3 CalculateBezierPoint(float t, Transform[] controlPoints)
    {
        int n = controlPoints.Length - 1;
        Vector3 point = Vector3.zero;

        for (int i = 0; i <= n; i++)
        {
            float binomial = BinomialCoefficient(n, i) * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
            point += binomial * controlPoints[i].position;
        }

        return point;
    }

    private int BinomialCoefficient(int n, int k)
    {
        if (k == 0 || k == n) return 1;
        int res = 1;
        for (int i = 1; i <= k; i++)
        {
            res = res * (n - i + 1) / i;
        }
        return res;
    }
}
