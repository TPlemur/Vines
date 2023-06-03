using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    ObjectiveScript objectives;
    [SerializeField] GameObject TopLevel;
    [SerializeField] GameObject MainLevel;
    [SerializeField] GameObject SettingLevel;


    [SerializeField] GameObject EquipmentLevel;
    [SerializeField] List<GameObject> EquipPages;
    public static List<bool> EquipPagesActive;
    int EquipcurrnetPage = 0;

    [SerializeField] GameObject CaseLevel;
    [SerializeField] List<GameObject> CasePages;
    int caseCurrnetPage = 0;


    enum menuState
    {
        off,
        main,
        settings,
        equip,
        Case
    }
    menuState currentState = menuState.off;

    public enum techPage
    {
        EE = 0,
        SC = 1,
        FL = 2,
        BE = 3,
        PV = 4,
        TR = 5,
        SH = 6,
        CH = 7,
        SZ = 8
    }

    // Start is called before the first frame update
    void Start()
    {
                                             //EE    SC    FL     BE     PV     TR     SH     CH     SZ
        EquipPagesActive = new List<bool>() { true, true, false, false, false, false, false, false, false };
        
    }

    // Update is called once per frame
    void Update()
    {
        //manage page changes/pause unpause
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
                    PageToMain(SettingLevel);
                    break;
                case menuState.equip:
                    PageToMain(EquipmentLevel);
                    break;
                case menuState.Case:
                    PageToMain(CaseLevel);
                    break;
            }
        }

        //manage sub-page changes
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (currentState)
            {
                case menuState.equip:
                    advPage(EquipPages,ref EquipcurrnetPage, true);
                    break;
                case menuState.Case:
                    advPage(CasePages,ref caseCurrnetPage, true);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (currentState)
            {
                case menuState.equip:
                    advPage(EquipPages,ref EquipcurrnetPage, false);
                    break;
                case menuState.Case:
                    advPage(CasePages,ref caseCurrnetPage, false);
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
        PauseSFX();
    }

    public void ResumeGame()
    {
        currentState = menuState.off;
        TopLevel.SetActive(false);
        Time.timeScale = 1;
        //SetUIElements(true);
        Cursor.lockState = CursorLockMode.Locked; //relock cursor on resume
        PauseSFX();
    }

    //go to settings
    public void MainToSettings()
    {
        MainLevel.SetActive(false);
        SettingLevel.SetActive(true);
        currentState = menuState.settings;
    }

    //go to case files
    public void MainToCase()
    {
        MainLevel.SetActive(false);
        CaseLevel.SetActive(true);
        currentState = menuState.Case;
    }

    //go to equip
    public void MainToEquip()
    {
        MainLevel.SetActive(false);
        EquipmentLevel.SetActive(true);
        currentState = menuState.equip;
    }

    //Deactivate page, activate main menu
    public void PageToMain(GameObject page)
    {
        MainLevel.SetActive(true);
        page.SetActive(false);
        currentState = menuState.main;
        FolderSFX();
    }

    public void switchPage(bool forward)
    {
        if(currentState == menuState.equip)
        {
            advTech(ref EquipcurrnetPage, forward);
        }
        if (currentState == menuState.Case)
        {
            advPage(CasePages, ref caseCurrnetPage, forward);
        }
    }

    //Switch sub-pages
    void advPage(List<GameObject> List, ref int current, bool forward)
    {
        List[current].SetActive(false);
        if (forward){
            current++;
            if(current == List.Count) { current = 0; }
        }
        else {
            current--;
            if(current == -1) { current = List.Count - 1; }
        }
        List[current].SetActive(true);
        // trigger SFX
        PageTurnSFX();
    }

    //
    public void advTech(ref int current, bool forward)
    {
        EquipPages[current].SetActive(false);
        int currentIndex = 0;
        int cycleCount = 0;
        if (forward)
        {
            currentIndex = current + 1;
            while (!EquipPagesActive[currentIndex % EquipPagesActive.Count]) { currentIndex++; cycleCount++; if (cycleCount > 10) { break; } }
            currentIndex = currentIndex % EquipPagesActive.Count;
        }
        else
        {
            currentIndex = current - 1;
            if (currentIndex < 0) { currentIndex = EquipPagesActive.Count - 1; }
            while (!EquipPagesActive[currentIndex % EquipPagesActive.Count]) { currentIndex--; if (currentIndex < 0) { currentIndex = EquipPagesActive.Count - 1; } cycleCount++; if (cycleCount > 10) { break; } }
            currentIndex = currentIndex % EquipPagesActive.Count;
        }
        current = currentIndex;
        EquipPages[current].SetActive(true);
        PageTurnSFX();
    }

    //sets current equip page to Page, and opens the menu directly to that page
    public void openToEquip(techPage page)
    {
        EquipPages[EquipcurrnetPage].SetActive(false);
        EquipcurrnetPage = (int)page;
        EquipPages[EquipcurrnetPage].SetActive(true);
        PauseGame();
        MainToEquip();
    }

    public void openToCase()
    {
        PauseGame();
        MainToCase();
    }

    //adds new lorePage and sets it to current
    public void addCasePage(GameObject page)
    {
        CasePages.Add(page);
        CasePages[caseCurrnetPage].SetActive(false);
        caseCurrnetPage = CasePages.Count - 1;
        CasePages[caseCurrnetPage].SetActive(true);
    }

    public static void setTechActive(techPage page)
    {
        EquipPagesActive[(int)page] = true;
    }

    private void FolderSFX()
    {
        const string eventName = "event:/UI/Envelope/Folder";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
    private void PageTurnSFX()
    {
        const string eventName = "event:/UI/Envelope/PageTurn";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
    private void PauseSFX()
    {
        const string eventName = "event:/SFX/Items/Inventory/Bag Pickup";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
}
