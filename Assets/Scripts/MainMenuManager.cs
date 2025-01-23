using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void ExitButton()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
