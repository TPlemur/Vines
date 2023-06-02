using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public Inventory inventory;
    public GameObject PText;
    public GameObject PVTM_Controls;
    public GameObject Flashlight_Controls;
    public GameObject Shield_Controls;
    public GameObject Electrical_Controls;
    public GameObject Beacon_Controls;
    //public GameObject Scanner_Controls;

    [Header("Input")]
    //public KeyCode interactKey = KeyCode.E;
    //public KeyCode cycleRightKey = KeyCode.E;
    //public KeyCode cycleLeftKey = KeyCode.Q;

    [Header("Item Pick Up Related")]
    public Camera playerCam;
    public LayerMask interactLayer;

    [Header("Electrical Equipment Related")]
    public LayerMask panelLayer;
    public GameObject progressUI;
    public GameObject LMBtext;

    [Header("PVTM Related")]
    public GameObject RealPVTMCamera;
    public LayerMask PVTMCamLayer;
    public LayerMask MonsterPicLayer;
    public Material FlashMat;

    [Header("Chirper Related")]

    [Header("Game State Related")]
    public GameObject gameStateManager;

    private Vector3 eeOrigPos;
    private Quaternion eeOrigRot;

    //Scanner related
    List<GameObject> scannerTargets;
    ElectricalEquipment EEScript; //passed to beacons

    void Start()
    {
        //instantiate EE as monobehavior within unity higerarchy
        inventory = new Inventory();
        scannerTargets = new List<GameObject>();
        addItem(typeof(ElectricalEquipment));

        eeOrigPos = inventory.GetEquippedGameObject().transform.localPosition;
        eeOrigRot = inventory.GetEquippedGameObject().transform.localRotation;
        scannerTargets = new List<GameObject>();
    }

    void Update()
    {
        CheckOutlines();
        // interact
        if (Input.GetKeyDown(KeyMapper.interact))
        {
            Interact();
        }

        if (inventory.EquippedIsElectricalEquipment())
        {
            if (Input.GetMouseButton(0) && Time.timeScale != 0)
            {
                inventory.GetEquippedGameObject().transform.localPosition = new Vector3((float)-0.08, (float)0.1, (float)0);
                inventory.GetEquippedGameObject().transform.localRotation = Quaternion.Euler((float)0, (float)0, (float)38.892);
                inventory.EquippedPrimary(); //allows access to update funct for time based things
            }
            else
            {
                inventory.GetEquippedGameObject().transform.localPosition = eeOrigPos;
                inventory.GetEquippedGameObject().transform.localRotation = eeOrigRot;
            }
        }

        //update outlines as necessasary
        //CheckOutlines();
        /*
        // cycle left and right
        if (Input.GetKeyDown(cycleRightKey))
        {
            if (inventory.EquippedIsToggled() && inventory.EquippedIsPVTM())
            {
                inventory.equipped.CycleRight();
            }
            else
            {
                inventory.CycleRight();
            }
        }

        else if (Input.GetKeyDown(cycleLeftKey))
        {
            if (inventory.EquippedIsToggled() && inventory.EquippedIsPVTM())
            {
                inventory.equipped.CycleLeft();
            }
            else
            {
                inventory.CycleLeft();
            }
        }
        */

        // left click
        if (Input.GetMouseButtonDown(0) && Time.timeScale != 0 && !Input.GetKey(KeyMapper.itemWheel))
        {
            inventory.EquippedPrimary();
        }

        // right click
        if (Input.GetMouseButtonDown(1) && Time.timeScale != 0 && !Input.GetKey(KeyMapper.itemWheel))
        {
            inventory.EquippedSecondary();
        }

        //SECRET DEV TOOLS SHHHHH DONT TELL ANYONE
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha5))
        {
            addItem(typeof(Chirper));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha6))
        {
            addItem(typeof(ScannerBeacon));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha7))
        {
            addItem(typeof(ElectricalEquipment));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha8))
        {
            addItem(typeof(PVTM));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha9))
        {
            addItem(typeof(Shield));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha0))
        {
            addItem(typeof(Flashlight));
        }
    }

    // Shoot Raycast to detect items that can be picked up
    private void Interact()
    {
        if (PText.activeSelf) { PText.SetActive(false); }
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.7f, interactLayer))
        {
            var interact = hit.transform;
            //add approprate item to inventory
            if (interact.tag == "PVTM" && !inventory.Has(typeof(PVTM)))
            {
                addItem(typeof(PVTM));
                Destroy(interact.gameObject);
            }
            if (interact.tag == "Shield" && !inventory.Has(typeof(Shield)))
            {
                addItem(typeof(Shield));
                Destroy(interact.gameObject);
            }
            if (interact.tag == "Flashlight" && !inventory.Has(typeof(Flashlight)))
            {
                addItem(typeof(Flashlight));
                Destroy(interact.gameObject);
            }
            if (interact.tag == "ScannerBeacon" && !inventory.Has(typeof(ScannerBeacon)))
            {
                addItem(typeof(ScannerBeacon));
                Destroy(interact.gameObject);
            }
            if (interact.tag == "Chirper" && !inventory.Has(typeof(Chirper)))
            {
                addItem(typeof(Chirper));
                Destroy(interact.gameObject);
                Brain.currentTarget = Brain.target.player;
            }
            //Activate valve
            if (interact.tag == "Valve")
            {
                interact.gameObject.GetComponent<ValveInteractable>().startInteract(KeyMapper.interact);
            }

        }
    }

    //RunOutlines
    private OutlineToggle lastOutline;
    private void CheckOutlines()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.7f,interactLayer))
        {
            var interact = hit.transform;
            //Toggle Items
            if (interact.tag == "PVTM" || interact.tag == "Shield" || interact.tag == "Flashlight" || interact.tag == "ScannerBeacon" || interact.tag == "Chirper")
            {
                PText.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Pick up " + interact.tag;
                PText.SetActive(true);
                lastOutline = interact.GetComponent<OutlineToggle>();
                lastOutline.On();
            }
            //toggle valves
            else if (interact.tag == "Valve")
            {

                PText.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Turn Valve";
                PText.SetActive(true);
                lastOutline = interact.GetComponent<OutlineToggle>();
                lastOutline.On();
            }
            //toggle electrical panel
            else if (interact.tag == "ElectricalPanel")
            {
                LMBtext.transform.GetComponentInChildren<TextMeshProUGUI>().text = "GENERATOR";
                LMBtext.SetActive(true);
                lastOutline = interact.GetComponent<OutlineToggle>();
                lastOutline.On();
            }
            else if (interact.tag == "ContainmentButton")
            {
                LMBtext.transform.GetComponentInChildren<TextMeshProUGUI>().text = "CONTAINMENT CELL";
                LMBtext.SetActive(true);
                lastOutline = interact.GetComponent<OutlineToggle>();
                lastOutline.On();
            }
            else if (interact.tag == "ElevatorConsole")
            {
                LMBtext.transform.GetComponentInChildren<TextMeshProUGUI>().text = "ELEVATOR INTERCOM (RETURN HERE WITH EVIDENCE)";
                LMBtext.SetActive(true);
                lastOutline = interact.GetComponent<OutlineToggle>();
                lastOutline.On();
            }
            //toggle off
            else if (lastOutline != null)
            {

                PText.SetActive(false);
                LMBtext.SetActive(false);
                lastOutline.Off();
            }
        }
        //also toggle off
        else if (lastOutline != null)
        {
            PText.SetActive(false);
            LMBtext.SetActive(false);
            lastOutline.Off();
        }
    }

    void addItem(System.Type type)
    {
        //checked if already in inventory
        if (!inventory.Has(type))
        {

            //properly instantiate item
            GameObject itemObj = new GameObject("item" + inventory.items.Count);
            itemObj.transform.parent = transform;
            itemObj.AddComponent(type);
            Item item = itemObj.GetComponent<Item>();

            //do approprate setup
            switch (item)
            {
                case ElectricalEquipment EE:
                    EEScript = EE;
                    EE.setup(playerCam, interactLayer, gameStateManager, Electrical_Controls, progressUI, scannerTargets);
                    break;
                case PVTM pvtm:
                    pvtm.setup(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer, FlashMat, gameStateManager, PVTM_Controls);
                    break;
                case Shield SH:
                    SH.setup(gameStateManager, Shield_Controls);
                    break;
                case Flashlight F:
                    F.setup(gameStateManager, Flashlight_Controls);
                    break;
                case ScannerBeacon S:
                    S.setup(playerCam, interactLayer, gameStateManager, Beacon_Controls, EEScript);
                    break;
                case Chirper C:
                    C.setup(playerCam, interactLayer, gameStateManager, Beacon_Controls);
                    break;
            }
            //add new item to inventory
            inventory.Add(item);
        }
    }

    IEnumerator addItemOnDelay(System.Type type)
    {
        yield return new WaitForSeconds(0.1f);
        addItem(type);
    }

    public bool Has(System.Type item)
    {
        return inventory.Has(item);
    }

    public void setActiveItem(System.Type item)
    {
        if (!inventory.Has(item)) { Debug.Log("Item Type Not in Inv");  return; }
        inventory.setCurrent(inventory.getIndexOfType(item));
    }
    public System.Type typeofCurrent()
    {
        return inventory.equipped.GetType();
    }
}

