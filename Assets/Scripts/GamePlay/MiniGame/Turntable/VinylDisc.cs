using UnityEngine;

public class VinylDisc : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    [SerializeField] private Transform trayPosition; // ✅ จุดที่แผ่นเสียงจะไปวาง

    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;

        if (!TurntableMinigame.Instance.CanInsertVinyl) return; // ✅ ป้องกันลากเข้าไปซ้ำ
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        if (Vector3.Distance(transform.position, trayPosition.position) < 1.0f) // ✅ ถ้าอยู่ใกล้ถาด
        {
            transform.position = trayPosition.position;
            TurntableMinigame.Instance.InsertVinyl();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
