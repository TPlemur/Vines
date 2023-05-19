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
    public KeyCode interactKey = KeyCode.F;
    public KeyCode cycleRightKey = KeyCode.E;
    public KeyCode cycleLeftKey = KeyCode.Q;

    [Header("Item Pick Up Related")]
    public Camera playerCam;
    public LayerMask interactLayer;

    [Header("Electrical Equipment Related")]
    public LayerMask panelLayer;
    public GameObject progressUI;

    [Header("PVTM Related")]
    public GameObject RealPVTMCamera;
    public LayerMask PVTMCamLayer;
    public LayerMask MonsterPicLayer;
    public Material FlashMat;

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
        if (Input.GetKeyDown(interactKey)) {
            Interact();
        }

        if (inventory.EquippedIsElectricalEquipment()) {
            if (Input.GetMouseButton(0) && Time.timeScale != 0) {
                inventory.GetEquippedGameObject().transform.localPosition = new Vector3((float)-0.08, (float)0.1, (float)0);
                inventory.GetEquippedGameObject().transform.localRotation = Quaternion.Euler((float)0, (float)0, (float)38.892);
                inventory.EquippedPrimary(); //allows access to update funct for time based things
            }
            else {
                inventory.GetEquippedGameObject().transform.localPosition = eeOrigPos;
                inventory.GetEquippedGameObject().transform.localRotation = eeOrigRot;
            }
        }

        //update outlines as necessasary
        //CheckOutlines();

        // cycle left and right
        if (Input.GetKeyDown(cycleRightKey)) {
            if (inventory.EquippedIsToggled() && inventory.EquippedIsPVTM()) {
                inventory.equipped.CycleRight();
            }
            else {
                inventory.CycleRight();
            }
        }
        else if (Input.GetKeyDown(cycleLeftKey)) {
            if (inventory.EquippedIsToggled() && inventory.EquippedIsPVTM()) {
                inventory.equipped.CycleLeft();
            }
            else {
                inventory.CycleLeft();
            }
        }

        // left click
        if (Input.GetMouseButtonDown(0) && Time.timeScale !=0) {
            inventory.EquippedPrimary();
        }

        // right click
        if (Input.GetMouseButtonDown(1) && Time.timeScale != 0) {
            inventory.EquippedSecondary();
        }

        //SECRET DEV TOOLS SHHHHH DONT TELL ANYONE
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha6)){
            addItem(typeof(ScannerBeacon));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha7)) {
            addItem(typeof(ElectricalEquipment));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha8)) {
            addItem(typeof(PVTM));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha9)) {
            addItem(typeof(Shield));
        }
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha0)) {
            addItem(typeof(Flashlight));
        }
    }

    // Shoot Raycast to detect items that can be picked up
    private void Interact() {
        if (PText.activeSelf) { PText.SetActive(false); }
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f, interactLayer)) {
            var interact = hit.transform;
            //add approprate item to inventory
            if (interact.tag == "PVTM" && !inventory.Has(typeof(PVTM))) {
                addItem(typeof(PVTM));
                Destroy(interact.gameObject);
            }
            if (interact.tag == "Shield" && !inventory.Has(typeof(Shield)))
            {
                addItem(typeof(Shield));
                Destroy(interact.gameObject);
            }
            if (interact.tag == "Flashlight" && !inventory.Has(typeof(Flashlight))) {
                addItem(typeof(Flashlight));
                Destroy(interact.gameObject);
            }
            if(interact.tag == "ScannerBeacon" && !inventory.Has(typeof(ScannerBeacon)))
            {
                addItem(typeof(ScannerBeacon));
                Destroy(interact.gameObject);
            }
            //Activate valve
            if (interact.tag == "Valve")
            {
                interact.gameObject.GetComponent<ValveInteractable>().startInteract(interactKey);
            }

        }
    }

    //RunOutlines
    private OutlineToggle lastOutline;
    private void CheckOutlines()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f))
        {
            var interact = hit.transform;
            //Toggle Items
            if (interact.tag == "PVTM" || interact.tag == "Shield" || interact.tag == "Flashlight" || interact.tag == "ScannerBeacon")
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
            else if (interact.tag == "ElectricalPanel" )
            {
                PText.transform.GetComponentInChildren<TextMeshProUGUI>().text = "FIX ELECTRONICS";
                PText.SetActive(true);
                lastOutline = interact.GetComponent<OutlineToggle>();
                lastOutline.On();
            }
            //toggle off
            else if (lastOutline != null)
            {

                PText.SetActive(false);
                lastOutline.Off();
            }
        }
        //also toggle off
        else if (lastOutline != null)
        {
            PText.SetActive(false);
            lastOutline.Off();
        }
    }

    void addItem(System.Type type)
    {
        //checked if already in inventory
        if (!inventory.Has(type)) {

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
                    EE.setup(playerCam, panelLayer, gameStateManager, Electrical_Controls, progressUI, scannerTargets);
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
                    S.setup(playerCam, panelLayer, gameStateManager, Beacon_Controls, EEScript);
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
}

