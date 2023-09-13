using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorConsole : MonoBehaviour
{
    string CurrentCode = ""; //current code attempt
    bool inputClear = true;  //enable/disable further input

    [SerializeField] TMPro.TextMeshProUGUI outCode;
    [SerializeField] Image CodeBackground;
    [SerializeField] GameObject PedistalLocation;
    [SerializeField] GameObject consoleScreen;
    [SerializeField] GameObject Door;
    [SerializeField] GameObject DoorTrigger;

    //door vars
    float doorClosedHeight = 1.905f;
    float doorOpenHeight = -1.693f;
    float doortime = 1f;

    //vars for color changes
    Color red = new Color(0.3529412f, 0, 0, 1);
    Color black = new Color(0,0,0,1);
    Color green = new Color(0, 0.3529412f, 0, 1);
    float flashTime = 0.5f;
    float currentTime = 0;

    //objects for activation toggle
    GameObject mainCan;
    GameObject playerCam;
    GameObject Player;
    GameObject TargetLocation;
    bool active = false;

    //open the door when power is resotred
    private void Awake()
    {
        StartCoroutine(toggleDoor(doorClosedHeight, doorOpenHeight));
    }

    //Make the console active and accepting inputs
    public void activate()
    {
        active = true;
        //find necessasary gameobjects
        mainCan = GameObject.Find("Canvas");
        playerCam = GameObject.Find("Main Camera");
        Player = GameObject.Find("Player");
        TargetLocation = GameObject.Find("PSXCAM UI Target");

        //disable other UI and player movement, and free the cursor
        mainCan.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GetComponent<BoxCollider>().enabled = false;
        playerCam.GetComponent<playerCamera>().enabled = false;
        Player.GetComponent<PlayerMovement>().enabled = false;
        Player.GetComponent<InventoryManager>().enabled = false;
        
        //move the console to in front of the camera
        consoleScreen.GetComponent<Canvas>().worldCamera = GameObject.Find("RenderCamera").GetComponent<Camera>(); 
        consoleScreen.transform.position = TargetLocation.transform.position;
        consoleScreen.transform.rotation = TargetLocation.transform.rotation;
        consoleScreen.transform.localScale = TargetLocation.transform.localScale;
    }

    //put ui back on console and dissalow input
    public void deactivate()
    {
        active = false;

        //lock cursor and re-enable all other functinality
        mainCan.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GetComponent<BoxCollider>().enabled = true;
        playerCam.GetComponent<playerCamera>().enabled = true;
        Player.GetComponent<PlayerMovement>().enabled = true;
        Player.GetComponent<InventoryManager>().enabled = true;

        //put console ui back on pedistl
        consoleScreen.transform.position = PedistalLocation.transform.position;
        consoleScreen.transform.rotation = PedistalLocation.transform.rotation;
        consoleScreen.transform.localScale = PedistalLocation.transform.localScale;
    }

    //listen for esc to deactivate
    private void Update()
    {
        if (active && Input.GetKeyDown(KeyCode.Escape))
        {
            deactivate();
        }
    }

    //add newD to diget string, then check the code
    public void addDidget(int newD)
    {
        if (!inputClear) { return; }

        CurrentCode += newD;
        outCode.text = CurrentCode;
        //check first lenght then validity
        if(CurrentCode.Length == 4)
        {
            if(CurrentCode == DoorCode.code)
            {
                StartCoroutine(Success());
            }
            else { StartCoroutine(Failed()); }
        }
    }

    //open the door and permently deactivate the console
    IEnumerator Success()
    {
        inputClear = false;
        //change the color to green
        while (currentTime < flashTime)
        {
            currentTime += Time.deltaTime;
            CodeBackground.color = Color.Lerp(black, green, currentTime / flashTime);
            yield return 0;
        }
        currentTime = 0;
        CodeBackground.color = green;
        //deactivate if not already done
        if (active)
        {
            deactivate();
            GetComponent<BoxCollider>().enabled = false;
        }

        StartCoroutine(toggleDoor(doorClosedHeight, doorOpenHeight));
    }

    //reset the input
    IEnumerator Failed()
    {
        inputClear = false;
        //flash red
        Color lerpColor = Color.white;
        while (currentTime < flashTime)
        {
            currentTime += Time.deltaTime;
            lerpColor = Color.Lerp(black, red, (currentTime / flashTime));
            CodeBackground.color = lerpColor;
            yield return 0;
        }
        currentTime = 0;
        while (currentTime < flashTime)
        {
            currentTime += Time.deltaTime;
            lerpColor = Color.Lerp(red, black, (currentTime / flashTime));
            CodeBackground.color = lerpColor;
            yield return 0;
        }
        CodeBackground.color = black;
        currentTime = 0;
        //reset the code
        CurrentCode = "";
        outCode.text = CurrentCode;
        inputClear = true;
    }

    //toggle for doortrigger to close the door
    public void closeDoor()
    {
        if (GameStateManager.GeneratorOn)
        {
            DoorTrigger.SetActive(false);
            StartCoroutine(toggleDoor(doorOpenHeight, doorClosedHeight));
        }
    }

    //open door after Generator on
    public void openDoor()
    {
        StartCoroutine(toggleDoor(doorClosedHeight, doorOpenHeight));
    }

    //move the door from the start position to the end position
    IEnumerator toggleDoor(float start,float end)
    {
        float time = 0;
        while (time < doortime)
        {
            time += Time.deltaTime;
            Door.transform.position = new Vector3(Door.transform.position.x, Mathf.Lerp(start, end, time / doortime), Door.transform.position.z);
            yield return 0;
        }
        Door.transform.position = new Vector3(Door.transform.position.x, end, Door.transform.position.z);
    }
}
