using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Level1");
    }

    public void ExitButton()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
