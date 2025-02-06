using UnityEngine;
using System.Collections;

public class InteractableApple : MonoBehaviour
{
    public float scaleStep = 0.1f;
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 maxScale = new Vector3(2.0f, 2.0f, 2.0f);
    public float animationSpeed = 5f; // ความเร็วในการเปลี่ยนขนาดแบบสมูธ

    private void Update()
    {
        if (ToolManager.Instance.CurrentMode == "Magnifier")
        {
            if (Input.GetMouseButtonDown(0))
            {
                ModifyScale(scaleStep);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ModifyScale(-scaleStep);
            }
        }
    }

    private void ModifyScale(float scaleChange)
    {
        Vector3 targetScale = transform.localScale + Vector3.one * scaleChange;

        // Clamp scale within min and max
        targetScale.x = Mathf.Clamp(targetScale.x, minScale.x, maxScale.x);
        targetScale.y = Mathf.Clamp(targetScale.y, minScale.y, maxScale.y);
        targetScale.z = Mathf.Clamp(targetScale.z, minScale.z, maxScale.z);

        StopAllCoroutines();
        StartCoroutine(AnimateScale(targetScale));

        BirdMinigame.Instance.CheckAppleSize();
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime);
            elapsedTime += Time.deltaTime * animationSpeed;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
