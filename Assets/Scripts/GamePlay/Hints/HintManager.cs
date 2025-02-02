using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    public bool IsPlayingHint = false;
    private GameObject currentHint;
    private Transform hintTransform;

    private Dictionary<GameObject, bool> pausedHints = new Dictionary<GameObject, bool>(); // เก็บสถานะ Hint ที่ถูกพัก

    [SerializeField] private float animationDuration = 1.0f;
    [SerializeField] private Vector3 hintTargetScale = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private Vector3 hintTargetPositionOffset = new Vector3(0, 2.0f, 0);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (IsPlayingHint && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseHint();
        }
    }

    public void StartHint(GameObject hintObject, Transform objectTransform, GameObject reward, bool useFadeIn = false)
    {
        Camera.main.GetComponent<CameraController>().EnterHint();

        if (pausedHints.ContainsKey(hintObject) && pausedHints[hintObject])
        {
            ResumeHint(hintObject, objectTransform, reward, useFadeIn);
            return;
        }

        currentHint = hintObject;
        hintTransform = objectTransform;
        IsPlayingHint = true;

        if (currentHint != null)
        {
            currentHint.SetActive(true);
        }

        // สามารถเลือกได้ว่าจะใช้ fade-in หรือ zoom-in สำหรับการแสดง Hint
        if (hintTransform != null)
        {
            if (useFadeIn)
            {
                StartCoroutine(FadeIn(hintTransform));
            }
            else
            {
                StartCoroutine(ZoomIn(hintTransform));
            }
        }

        Debug.Log("Hint started!");
    }

    private IEnumerator ZoomIn(Transform obj)
    {
        Vector3 smallScale = obj.localScale * 0.5f;
        obj.localScale = smallScale;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            obj.localScale = Vector3.Lerp(smallScale, hintTargetScale, t);
            yield return null;
        }

        obj.localScale = hintTargetScale;
    }

    private IEnumerator FadeIn(Transform obj)
    {
        SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();
        Dictionary<SpriteRenderer, float> originalAlphas = new Dictionary<SpriteRenderer, float>();

        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                originalAlphas[sr] = sr.color.a;
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // เริ่มต้นที่ Alpha 0
            }
        }

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            foreach (var sr in spriteRenderers)
            {
                if (sr != null && originalAlphas.ContainsKey(sr))
                {
                    Color color = sr.color;
                    float targetAlpha = originalAlphas[sr];
                    color.a = Mathf.Lerp(0f, targetAlpha, t);
                    sr.color = color;
                }
            }
            yield return null;
        }

        foreach (var sr in spriteRenderers)
        {
            if (sr != null && originalAlphas.ContainsKey(sr))
            {
                Color color = sr.color;
                color.a = originalAlphas[sr];
                sr.color = color;
            }
        }
    }

    public void PauseHint()
    {
        Camera.main.GetComponent<CameraController>().ExitHint();
        if (currentHint == null) return;

        pausedHints[currentHint] = true;
        IsPlayingHint = false;
        currentHint.SetActive(false);
        Debug.Log($"Hint paused: {currentHint.name}");
    }

    public void ResumeHint(GameObject hintObject, Transform objectTransform, GameObject reward, bool useFadeIn = false)
    {
        currentHint = hintObject;
        hintTransform = objectTransform;

        if (!pausedHints.ContainsKey(hintObject) || !pausedHints[hintObject]) return;

        IsPlayingHint = true;
        hintObject.SetActive(true);
        pausedHints[hintObject] = false; // ล้างสถานะพัก
        Debug.Log($"Hint resumed: {hintObject.name}");

        if (hintTransform != null)
        {
            if (useFadeIn)
            {
                StartCoroutine(FadeIn(hintTransform));
            }
            else
            {
                StartCoroutine(ZoomIn(hintTransform));
            }
        }
    }

    public void CompleteHint()
    {
        Debug.Log("Hint completed! Showing reward animation...");
    }

    public void OnRewardCollected()
    {
        Debug.Log("Reward collected. Closing hint...");
        StartCoroutine(FadeAndClose());
    }

    private IEnumerator FadeAndClose()
    {
        if (hintTransform == null)
        {
            Debug.LogWarning("Hint Transform is null. Skipping fade out.");
            EndHint();
            yield break;
        }

        SpriteRenderer[] spriteRenderers = hintTransform.GetComponentsInChildren<SpriteRenderer>();
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = Mathf.Lerp(1f, 0f, t);
                    sr.color = color;
                }
            }
            yield return null;
        }

        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0f;
                sr.color = color;
            }
        }

        EndHint();
    }

    public void EndHint()
    {
        IsPlayingHint = false;
        if (currentHint != null)
        {
            currentHint.SetActive(false);
        }

        ToolManager.Instance.SetToolMode("Eye");
        DialogueManager.Instance.HideDialogue();
        Camera.main.GetComponent<CameraController>().ExitHint();
        Debug.Log("Hint ended.");
    }

}
