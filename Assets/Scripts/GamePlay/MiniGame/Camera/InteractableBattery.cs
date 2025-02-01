using UnityEngine;

public class InteractableBattery : MonoBehaviour
{
    private bool isDragging = false;

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Hand" && isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
        }
        else if (ToolManager.Instance.CurrentMode == "Magnifier")
        {
            HandleScaling();
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
        CameraMinigame.Instance.InsertNewBattery();
    }

    private void HandleScaling()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ModifyScale(0.1f);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ModifyScale(-0.1f);
        }
    }

    private void ModifyScale(float scaleStep)
    {
        Vector3 newScale = transform.localScale + Vector3.one * scaleStep;
        newScale.x = Mathf.Clamp(newScale.x, 0.5f, 1f);
        newScale.y = Mathf.Clamp(newScale.y, 0.5f, 1f);
        newScale.z = Mathf.Clamp(newScale.z, 0.5f, 1f);
        transform.localScale = newScale;
    }
}
