using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    //Call to get the player to start the elevator scene
    public void StartGame() 
    {
        SceneManager.LoadScene(4);
    }

    //Call when the player gets killed by the monster
    public void LostScene() 
    {
        SceneManager.LoadScene(2);
    }

    public void BackToMainMenu() 
    {
        Time.timeScale = 1; //resume time if time was paused during last game
        SceneManager.LoadScene(0);
    }

    public void QuitButton()
    {
        Application.Quit(); //Closes game
    }
}
