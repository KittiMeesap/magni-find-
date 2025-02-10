using UnityEngine;
using UnityEngine.SceneManagement; // สำหรับโหลดฉากใหม่

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject playMinigameObject;

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

    // ฟังก์ชันที่ทำงานเมื่อกดปุ่ม Play
    public void StartMinigame()
    {
        playMinigameObject.SetActive(true);
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
