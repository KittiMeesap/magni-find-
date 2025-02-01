using UnityEngine;

public class InteractableOldBattery : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
        }
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            isDragging = true;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (Vector3.Distance(transform.position, initialPosition) < 0.5f)
        {
            return;
        }

        CameraMinigame.Instance.RemoveOldBattery();
        //CameraMinigame.Instance.SpawnNewBattery();
    }
}
