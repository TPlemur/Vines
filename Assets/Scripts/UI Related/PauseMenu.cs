using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    ObjectiveScript objectives;
    [SerializeField] GameObject TopLevel;
    [SerializeField] GameObject MainLevel;
    [SerializeField] GameObject SettingLevel;


    enum menuState
    {
        off,
        main,
        settings
    }
    menuState currentState = menuState.off;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (currentState)
            {
                case menuState.off:
                    PauseGame();
                    break;
                case menuState.main:
                    ResumeGame();
                    break;
                case menuState.settings:
                    SettingsToMain();
                    break;
            }

           
        }
    }

    public void PauseGame()
    {
        currentState = menuState.main;
        TopLevel.SetActive(true);
        Time.timeScale = 0;
        //SetUIElements(false);
        Cursor.lockState = CursorLockMode.Confined; //unlock cursor on resume
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        currentState = menuState.off;
        TopLevel.SetActive(false);
        Time.timeScale = 1;
        //SetUIElements(true);
        Cursor.lockState = CursorLockMode.Locked; //relock cursor on resume
    }

    public void MainToSettings()
    {
        MainLevel.SetActive(false);
        SettingLevel.SetActive(true);
        currentState = menuState.settings;
    }

    public void SettingsToMain()
    {
        MainLevel.SetActive(true);
        SettingLevel.SetActive(false);
        currentState = menuState.main;
        FolderSFX();
    }

    private void FolderSFX()
    {
        const string eventName = "event:/UI/Envelope/Folder";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
}
