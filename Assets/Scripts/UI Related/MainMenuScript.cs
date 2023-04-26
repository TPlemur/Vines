using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenuScript : MonoBehaviour
{
    public TMP_InputField seedInput;
    public TMP_Text text;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
