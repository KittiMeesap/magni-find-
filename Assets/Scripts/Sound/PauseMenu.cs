using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [SerializeField] private GameObject settingsPanel; // ✅ ตัว UI Settings Menu
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

        settingsPanel.SetActive(false); // ✅ ปิดเมนูไว้ก่อน
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        settingsPanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // ✅ หยุดเกม
        }
        else
        {
            Time.timeScale = 1f; // ✅ กลับมาเล่นเกมต่อ
        }
    }
}
