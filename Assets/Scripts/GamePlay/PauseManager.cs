using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenuUI; // ✅ UI สำหรับ Pause Menu
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pauseMenuUI.SetActive(false); // ✅ ปิดเมนูตอนเริ่มเกม
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Escape)) // ✅ กด ESC เพื่อ Pause/Resume
        {
            TogglePause();
        }*/
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // ✅ หยุดเวลาเกม
            pauseMenuUI.SetActive(true); // ✅ เปิดเมนู Pause
        }
        else
        {
            Time.timeScale = 1f; // ✅ คืนค่าเวลาเป็นปกติ
            pauseMenuUI.SetActive(false); // ✅ ปิดเมนู Pause
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // ✅ คืนค่าเวลาก่อนออกจากเกม
        Application.Quit();
    }
}
