using UnityEngine;

public class InteractableMouse : MonoBehaviour
{
    private bool isHovering = false;

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier" && isHovering)
        {
            if (Input.GetMouseButtonDown(0)) // คลิกซ้าย = ขยาย
            {
                ModifyScale(0.1f);
            }
            else if (Input.GetMouseButtonDown(1)) // คลิกขวา = ย่อ
            {
                ModifyScale(-0.1f);
            }
        }
    }

    private void OnMouseOver()
    {
        isHovering = true;
    }

    private void OnMouseExit()
    {
        isHovering = false;
    }

    private void ModifyScale(float scaleStep)
    {
        float newSize = transform.localScale.x + scaleStep;
        newSize = Mathf.Clamp(newSize, 0.3f, 1.5f);
        transform.localScale = new Vector3(newSize, newSize, 1);

        CatMinigame.Instance.AdjustMouseSize(newSize);
    }
}
