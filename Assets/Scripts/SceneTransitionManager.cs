using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private Image fadeOverlay; // ✅ UI Image สำหรับเฟดดำ
    [SerializeField] private float fadeDuration = 1.0f; // ✅ ระยะเวลา Fade
    [SerializeField] private float waitIn = 0;
    [SerializeField] private float waitOutBefore = 0;
    [SerializeField] private float waitOutAfter = 0;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        fadeOverlay.color = new Color(0, 0, 0, 255);
        //DontDestroyOnLoad(gameObject); // ✅ ให้ Script คงอยู่ข้ามซีน
        fadeOverlay.gameObject.SetActive(true);
        StartCoroutine(FadeIn()); // ✅ ทำ Fade In ตอนเริ่ม
    }

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionScene(sceneName));
        }
    }

    private IEnumerator TransitionScene(string sceneName)
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.1f); // ✅ รอให้ซีนโหลดเสร็จ
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(waitOutBefore);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        yield return new WaitForSeconds(waitOutAfter);
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(waitIn);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            fadeOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
