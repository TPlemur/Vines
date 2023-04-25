using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private FadeController fadeController;
    void Start()
    {
        fadeController = this.AddComponent<FadeController>();
        fadeController.FadeIn(5);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    //Call to get the player to start the elevator scene
    public void StartGame() 
    {
        fadeController.FadeOutToSceen(3, 4);
    }

    //Call when the player gets killed by the monster
    public void LostScene() 
    {
        SceneManager.LoadScene(2);
    }

    public void BackToMainMenu() 
    {
        Time.timeScale = 1; //resume time if time was paused during last game
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SceneManager.LoadScene(0); //if in the main gameplay scene, immediatly leave to main menu
        } 
        else {
            fadeController.FadeOutToSceen(3, 0); //else fade away
        }
    }

    public void QuitButton()
    {
        Application.Quit(); //Closes game
    }
}
