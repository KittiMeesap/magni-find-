using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroScene : MonoBehaviour
{
    [Header("Button Settings")]
    public Button nextButton;

    [Header("Camera Settings")]
    public Camera mainCamera;
    public float cameraMoveSpeed = 5f;

    [Header("Sprites Settings")]
    public SpriteRenderer[] cutscenePanel;
    private int currentPanel;

    public void Start()
    {
        currentPanel = 0;
    }


    public void ShowingNextPanel()
    {
        if (currentPanel < cutscenePanel.Length)
        {
            SpriteRenderer spriteRenderer = cutscenePanel[currentPanel].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = 1f;
                spriteRenderer.color = newColor;
            }

            StartCoroutine(CameraFollowing(() =>
            {
                currentPanel++;
            }));
        }
    }

    public IEnumerator CameraFollowing(System.Action onComplete = null)
    {
        if (currentPanel < cutscenePanel.Length)
        {
            Vector3 targetPosition = new Vector3(
                cutscenePanel[currentPanel].transform.position.x,
                cutscenePanel[currentPanel].transform.position.y,
                mainCamera.transform.position.z
            );

            while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f)
            {
                mainCamera.transform.position = Vector3.Lerp(
                    mainCamera.transform.position,
                    targetPosition,
                    Time.deltaTime * cameraMoveSpeed
                );
                yield return null;
            }

            mainCamera.transform.position = targetPosition;
            onComplete?.Invoke();

            nextButton.gameObject.SetActive(true);
        }
    }
}
