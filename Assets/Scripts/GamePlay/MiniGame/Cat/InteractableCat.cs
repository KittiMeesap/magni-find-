using UnityEngine;

public class InteractableCat : MonoBehaviour
{
    private bool isHovering = false;

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier" && isHovering)
        {
            if (Input.GetMouseButtonDown(0)) // คลิกซ้าย = ขยาย
            {
                ModifyScale(0.5f);
            }
            else if (Input.GetMouseButtonDown(1)) // คลิกขวา = ย่อ
            {
                ModifyScale(-0.5f);
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
        newSize = Mathf.Clamp(newSize, 1.5f, 2.5f);
        transform.localScale = new Vector3(newSize, newSize, newSize);

        CatMinigame.Instance.AdjustCatSize(newSize);
    }
}
