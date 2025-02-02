using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    public bool IsPlayingMinigame = false;
    private GameObject currentMinigame;
    private Transform minigameTransform;
    private GameObject rewardItem;

    private Dictionary<GameObject, bool> pausedMinigames = new Dictionary<GameObject, bool>(); // เก็บสถานะมินิเกมที่ถูกพัก

    [SerializeField] private float animationDuration = 1.0f;
    [SerializeField] private float rewardItemSlideDistance = 2.0f;
    [SerializeField] private Vector3 minigameTargetScale = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private Vector3 minigameTargetPositionOffset = new Vector3(0, 2.0f, 0);

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
        if (IsPlayingMinigame && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMinigame();
        }
    }

    public void StartMinigame(GameObject minigameObject, Transform objectTransform, GameObject reward, bool useFadeIn = false)
    {
        Camera.main.GetComponent<CameraController>().EnterMinigame();

        if (pausedMinigames.ContainsKey(minigameObject) && pausedMinigames[minigameObject])
        {
            ResumeMinigame(minigameObject, objectTransform, reward, useFadeIn);
            return;
        }

        currentMinigame = minigameObject;
        minigameTransform = objectTransform;
        rewardItem = reward;
        IsPlayingMinigame = true;

        if (currentMinigame != null)
        {
            currentMinigame.SetActive(true);
        }

        /*
        if (minigameTransform != null)
        {
            if (useFadeIn)
            {
                StartCoroutine(FadeIn(minigameTransform));
            }
            else
            {
                StartCoroutine(ZoomIn(minigameTransform));
            }
        }
        */

        Debug.Log("Minigame started!");
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
            obj.localScale = Vector3.Lerp(smallScale, minigameTargetScale, t);
            yield return null;
        }

        obj.localScale = minigameTargetScale;
    }

    private IEnumerator FadeIn(Transform obj)
    {
        SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();

        // ✅ **เก็บค่า Alpha ดั้งเดิมของแต่ละ SpriteRenderer**
        Dictionary<SpriteRenderer, float> originalAlphas = new Dictionary<SpriteRenderer, float>();
        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                originalAlphas[sr] = sr.color.a; // 🔹 บันทึกค่า Alpha เดิม
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // 🔹 ตั้งค่าเริ่มต้นเป็น 0
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
                    float targetAlpha = originalAlphas[sr]; // 🔹 ใช้ค่า Alpha ดั้งเดิมเป็นเป้าหมาย
                    color.a = Mathf.Lerp(0f, targetAlpha, t);
                    sr.color = color;
                }
            }
            yield return null;
        }

        // ✅ **ตรวจสอบให้แน่ใจว่า Alpha ถูกต้องหลังจบ Animation**
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


    public void PauseMinigame()
    {
        Camera.main.GetComponent<CameraController>().ExitMinigame();
        if (currentMinigame == null) return;

        pausedMinigames[currentMinigame] = true; // บันทึกว่ามินิเกมนี้ถูกพัก
        IsPlayingMinigame = false;
        currentMinigame.SetActive(false);
        Debug.Log($"Minigame paused: {currentMinigame.name}");
    }

    public void ResumeMinigame(GameObject minigameObject, Transform objectTransform, GameObject reward, bool useFadeIn = false)
    {
        currentMinigame = minigameObject;
        minigameTransform = objectTransform;
        rewardItem = reward;

        if (!pausedMinigames.ContainsKey(minigameObject) || !pausedMinigames[minigameObject]) return;

        IsPlayingMinigame = true;
        minigameObject.SetActive(true);
        pausedMinigames[minigameObject] = false; // ล้างสถานะพัก
        Debug.Log($"Minigame resumed: {minigameObject.name}");
        
        /*
        if (minigameTransform != null)
        {
            if (useFadeIn)
            {
                StartCoroutine(FadeIn(minigameTransform));
            }
            else
            {
                StartCoroutine(ZoomIn(minigameTransform));
            }
        }
        */
    }

    public void CompleteMinigame()
    {
        Debug.Log("Minigame completed! Showing reward animation...");
    }

    public void OnRewardCollected()
    {
        Debug.Log("Reward collected. Closing minigame...");
        StartCoroutine(FadeAndClose());
    }

    private IEnumerator FadeAndClose()
    {
        if (minigameTransform == null)
        {
            Debug.LogWarning("Minigame Transform is null. Skipping fade out.");
            EndMinigame();
            yield break;
        }

        SpriteRenderer[] spriteRenderers = minigameTransform.GetComponentsInChildren<SpriteRenderer>();

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

        EndMinigame();
    }

    public void EndMinigame()
    {
        IsPlayingMinigame = false;
        if (currentMinigame != null)
        {
            currentMinigame.SetActive(false);
        }

        ToolManager.Instance.SetToolMode("Eye");

        DisableMinigameTrigger();
        DialogueUI.Instance.HideDialogue();
        Camera.main.GetComponent<CameraController>().ExitMinigame();
        Debug.Log("Minigame ended.");
    }
    private GameObject minigameTrigger;
    public void SetMinigameTrigger(GameObject trigger)
    {
        minigameTrigger = trigger;
    }

    // ✅ ปิด Collider ของตัวกดเข้าเล่นมินิเกม
    public void DisableMinigameTrigger()
    {
        if (minigameTrigger != null)
        {
            Collider2D collider = minigameTrigger.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
                Debug.Log($"Minigame trigger {minigameTrigger.name} disabled.");
            }
            else
            {
                Debug.LogWarning($"No Collider2D found on {minigameTrigger.name}!");
            }
        }
        else
        {
            Debug.LogWarning("Minigame trigger is NULL!");
        }
    }
}
