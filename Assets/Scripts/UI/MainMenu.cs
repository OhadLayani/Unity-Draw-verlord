using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("GameplayScene"); //remember to switch to the scene that would contain the mvp
    }

    public void QuitGame()
    {
        Application.Quit();
    }
   
}
