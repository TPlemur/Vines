using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveScript : MonoBehaviour
{
    public int inputCounter = 0;

    
    public GameObject MoveText;
    public GameObject PowerObj;
    public GameObject PVTMObj;
    public GameObject DocumentObj;
    public GameObject EscapeObj;
    public GameObject FlashObj;
    public GameObject PauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        MoveText.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else if(Time.timeScale == 0)
            {
                ResumeGame();
            }
            
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && (inputCounter < 3))
        {
            inputCounter++;
            if (inputCounter == 1)
            {
                MoveText.SetActive(false);
                DisplayObj(FlashObj,true);
            }
            
        }

    }
    public void DisplayObj(GameObject objective, bool status)
    {
        objective.SetActive(status);
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
