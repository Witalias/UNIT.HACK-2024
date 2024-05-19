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
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
