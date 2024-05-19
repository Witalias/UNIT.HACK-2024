using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void LoadLevel()
    {
        SceneManager.LoadScene("Terrain");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Main menu");
        Time.timeScale = 1.0f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
