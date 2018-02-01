using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    public static void LoadStartScene()
    {
        SceneManager.LoadScene("Start");
    }

    public static void LoadMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
