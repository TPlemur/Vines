using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuScript : MonoBehaviour
{
    public Text endingsDiscovered;
    public bool firstEndingFound;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (firstEndingFound){
             endingsDiscovered.GetComponent<UnityEngine.UI.Text>().text = "1/2";
        }
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
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "WinEndSceene"){
            firstEndingFound = true;
        }
        SceneManager.LoadScene(0);
    }

    public void QuitButton()
    {
        Application.Quit(); //Closes game
    }
}
