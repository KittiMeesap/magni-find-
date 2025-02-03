using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // เพิ่มการใช้งาน SceneManager
using System.Collections;

public class DialogueCutscene : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject[] dialoguePanels;  // เก็บ UI Panel ที่เตรียมไว้
    public Transform dialogueContainer;  // Parent ของ Dialogue Panels (Content ของ ScrollView)
    public ScrollRect scrollRect;        // ตัว ScrollView

    private int currentDialogueIndex = 0;
    private bool dialogueFinished = false;  // ตัวแปรสำหรับเช็คว่าไดอาล็อคจบแล้ว

    private IEnumerator Start()
    {
        yield return null; // รอให้ UI โหลดก่อน

        // เปิด ScrollView ให้พร้อมใช้งาน
        scrollRect.gameObject.SetActive(true);

        // ตรวจสอบและแสดง Dialogue Panel แรก
        if (dialoguePanels.Length > 0)
        {
            ShowNextDialogue();
        }

        // บังคับให้ Unity อัปเดต Layout
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueContainer.GetComponent<RectTransform>());

        // เลื่อน ScrollView ให้ไปที่ด้านล่างสุด
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void ShowNextDialogue()
    {
        if (currentDialogueIndex < dialoguePanels.Length)
        {
            GameObject newDialogue = dialoguePanels[currentDialogueIndex];

            if (newDialogue == null)
            {
                Debug.LogError($"❌ dialoguePanels[{currentDialogueIndex}] เป็น null! ข้ามไป");
                currentDialogueIndex++;
                return;
            }

            // เปิด Dialogue Panel
            newDialogue.SetActive(true);
            StartCoroutine(FadeIn(newDialogue));
            StartCoroutine(UpdateScrollView());

            currentDialogueIndex++;
        }
        else
        {
            // เมื่อแสดงไดอาล็อคทั้งหมดแล้ว
            dialogueFinished = true;
            // เปลี่ยนไปซีนเล่นเกมเมื่อไดอาล็อคทั้งหมดจบ
            ChangeSceneToGame();
        }
    }

    private IEnumerator UpdateScrollView()
    {
        yield return null;  // รอให้ Layout อัปเดต

        // รีเฟรช Layout ใหม่
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueContainer.GetComponent<RectTransform>());
        yield return null;  // รอให้ Unity อัปเดตอีกรอบ

        // เลื่อน ScrollView ไปที่ด้านล่างสุด
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private IEnumerator FadeIn(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private void ChangeSceneToGame()
    {
        // ใช้ SceneManager เพื่อเปลี่ยนไปยังซีนเกม
        // กำหนดชื่อซีนของเกมที่คุณต้องการให้โหลด
        SceneManager.LoadScene("Gameplay");  // "GameScene" คือตัวอย่างชื่อซีนที่คุณต้องการ
    }

    // หากคุณต้องการให้การกดปุ่มถัดไปหลังจากไดอาล็อคหมดไปทำการเปลี่ยนซีน
    public void OnNextButtonPressed()
    {
        if (dialogueFinished)
        {
            // ถ้าไดอาล็อคหมดแล้ว ให้เปลี่ยนไปซีนเกม
            ChangeSceneToGame();
        }
        else
        {
            ShowNextDialogue();
        }
    }
}
