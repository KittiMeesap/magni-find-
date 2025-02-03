using UnityEngine;
using UnityEngine.SceneManagement; // สำหรับโหลดฉากใหม่

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject playMinigameObject;
    [SerializeField] private GameObject startButton; // ปุ่ม Start
    [SerializeField] private GameObject exitButton; // ปุ่ม Exit

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

        // ซ่อนปุ่ม Start และ Exit ตั้งแต่แรก
        startButton.SetActive(false);
        exitButton.SetActive(false);
    }

    // ฟังก์ชันที่ทำงานเมื่อกดปุ่ม Play
    public void StartMinigame()
    {
        playMinigameObject.SetActive(true);
        startButton.SetActive(true); // แสดงปุ่ม Start
        exitButton.SetActive(true); // แสดงปุ่ม Exit
        Debug.Log("Minigame started from MainMenuManager!");
    }

    // ฟังก์ชันโหลดฉากถัดไป
    public void LoadNextLevel()
    {
        // โหลดฉากถัดไป (ต้องเพิ่มฉากลงใน Build Settings)
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load.");
        }
    }

    // ฟังก์ชันออกจากเกม
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
