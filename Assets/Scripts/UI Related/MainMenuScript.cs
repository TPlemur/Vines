using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenuScript : MonoBehaviour
{

    private FadeController fadeController;
    public TMP_InputField seedInput;
    public TMP_Text text;

    void Start()
    {
        fadeController = this.AddComponent<FadeController>();
        fadeController.FadeIn(5);
        //set approprate lock mode
        if(SceneManager.GetActiveScene().name == "ProtoPlayScene" || SceneManager.GetActiveScene().name == "ElevatorScene")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        try {
            seedInput.text = UnityEngine.Random.Range(0, 2147483647).ToString();
        }
        catch (NullReferenceException ex) {
        }
    }
    public void randomSeed() {
        seedInput.text = UnityEngine.Random.Range(0, 2147483647).ToString();
    }
    //Check if seed is legal, if so start game else 
    public void CheckSeed() {
        int test;
        if (int.TryParse(seedInput.text, out test)) {
            UnityEngine.Random.InitState(test);
            StartGame();
        }
        else {
            text.gameObject.SetActive(true);
        }
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
