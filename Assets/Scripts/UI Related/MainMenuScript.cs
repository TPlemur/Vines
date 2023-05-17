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
    public TMP_Text finalTime;
    public Toggle story;
    public Toggle radar;
    public Toggle timer;

    public static bool speedRun = false;
    public static bool scannerOn = true;

    void Start()
    {
        fadeController = this.AddComponent<FadeController>();
        fadeController.FadeIn(0.25f);
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
            if (speedRun) {
                finalTime.gameObject.SetActive(true);
                finalTime.text = "Time: " + ObjectiveScript.timeElapsed.ToString(@"mm\:ss\:ff");
            }
            seedInput.text = UnityEngine.Random.Range(0, 2147483647).ToString();
        }
        catch (NullReferenceException ex) {
        }
        story.isOn = PlayerPrefs.GetInt("isStory", 0) == 0;
        radar.isOn = PlayerPrefs.GetInt("isRadar", 0) == 0;
        timer.isOn = PlayerPrefs.GetInt("isTimer", 1) == 0;
    }

    //set the appropreate variable
    public void SetTimer() {
        if (timer.isOn) {
            MainMenuScript.speedRun = true;
            PlayerPrefs.SetInt("isTimer", 0);
        }
        else {
            MainMenuScript.speedRun = false;
            PlayerPrefs.SetInt("isTimer", 1);
        }
    }
    public void SetStory()
    {
        if (story.isOn)
        {
            PlayerPrefs.SetInt("isStory", 0);
        }
        else
        {
            PlayerPrefs.SetInt("isStory", 1);
        }
    }
    public void SetScanner()
    {
        if (timer.isOn)
        {
            MainMenuScript.scannerOn = true;
            PlayerPrefs.SetInt("isRadar", 0);
        }
        else
        {
            MainMenuScript.scannerOn = false;
            PlayerPrefs.SetInt("isRadar", 1);
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
        if (story.isOn) {
            fadeController.FadeOutToSceen(0.25f, 4);
        }
        else {
            fadeController.FadeOutToSceen(0.25f, 1);
        }
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
            MainMenuScript.speedRun = false;
        } 
        else {
            fadeController.FadeOutToSceen(0.25f, 0); //else fade away
            MainMenuScript.speedRun = false;
        }
    }

    public void QuitButton()
    {
        Application.Quit(); //Closes game
    }
}
