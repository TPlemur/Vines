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

    [Header("Input")]
    public KeyCode interactKey = KeyCode.F;
    public KeyCode cycleRightKey = KeyCode.E;
    public KeyCode cycleLeftKey = KeyCode.Q;

    [Header("Item Pick Up Related")]
    public Camera playerCam;
    public LayerMask interactLayer;

    [Header("Electrical Equipment Related")]
    public LayerMask panelLayer;

    [Header("PVTM Related")]
    public GameObject RealPVTMCamera;
    public LayerMask  PVTMCamLayer;
    public LayerMask  MonsterPicLayer;
    public Material   FlashMat;

    [Header("Game State Related")]
    public GameObject gameStateManager;

    private Vector3 eeOrigPos;
    private Quaternion eeOrigRot;

    void Start()
    {
        inventory = new Inventory();
        inventory.Add(new ElectricalEquipment(playerCam, panelLayer, gameStateManager, Electrical_Controls));
        eeOrigPos = inventory.GetEquippedGameObject().transform.localPosition;
        eeOrigRot = inventory.GetEquippedGameObject().transform.localRotation;
    }

    void Update()
    {
        CheckOutlines();
        // interact
        if (Input.GetKeyDown(interactKey)){
            Interact();
        }

        if (inventory.EquippedIsElectricalEquipment()){
            if (Input.GetMouseButton(0)){
                inventory.GetEquippedGameObject().transform.localPosition = new Vector3((float) -0.08, (float) 0.1, (float) 0);
                inventory.GetEquippedGameObject().transform.localRotation = Quaternion.Euler((float) 0, (float) 0, (float) 38.892);
                inventory.EquippedPrimary(); //allows access to update funct for time based things
            }
            else{
                inventory.GetEquippedGameObject().transform.localPosition = eeOrigPos;
                inventory.GetEquippedGameObject().transform.localRotation = eeOrigRot;
            }
        }

        //update outlines as necessasary
        //CheckOutlines();

        // cycle left and right
        if (Input.GetKeyDown(cycleRightKey)){
            if(inventory.EquippedIsToggled() && inventory.EquippedIsPVTM()){
                inventory.equipped.CycleRight();
            }
            else{
                inventory.CycleRight();
            }
        }
        else if(Input.GetKeyDown(cycleLeftKey)){
            if(inventory.EquippedIsToggled() && inventory.EquippedIsPVTM()){
                inventory.equipped.CycleLeft();
            }
            else{
                inventory.CycleLeft();
            }
        }

        // left click
        if(Input.GetMouseButtonDown(0)){
            inventory.EquippedPrimary();
        }

        // right click
        if(Input.GetMouseButtonDown(1)){
            inventory.EquippedSecondary();
        }

        // SECRET DEV TOOLS SHHHHH DONT TELL ANYONE
        if(GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha7)){
            inventory.Add(new ElectricalEquipment(playerCam, panelLayer, gameStateManager, Electrical_Controls));
        }
        if(GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha8)){
            inventory.Add(new PVTM(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer, FlashMat, gameStateManager, PVTM_Controls));
        }
        if(GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha9)){
            inventory.Add(new Shield(gameStateManager, Shield_Controls));
        }
        if(GameStateManager.debug && Input.GetKeyDown(KeyCode.Alpha0)){
            inventory.Add(new Flashlight(gameStateManager, Flashlight_Controls));
        }
    }

    // Shoot Raycast to detect items that can be picked up
    private void Interact(){
        if (PText.activeSelf) { PText.SetActive(false); }
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f, interactLayer)){
            var interact = hit.transform;
            if (interact.tag == "PVTM"){
                inventory.Add(new PVTM(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer, FlashMat, gameStateManager, PVTM_Controls));
            }
            if(interact.tag == "Shield"){
                inventory.Add(new Shield(gameStateManager, Shield_Controls));
            }
            if(interact.tag == "Flashlight"){
                inventory.Add(new Flashlight(gameStateManager, Flashlight_Controls));
            }
            Destroy(interact.gameObject);
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
            if (interact.tag == "PVTM" || interact.tag == "Shield" || interact.tag == "Flashlight")
            {
                PText.transform.GetComponent<TextMeshProUGUI>().text = "Press F to pick up " + interact.tag;
                PText.SetActive(true);
                lastOutline = interact.GetComponent<OutlineToggle>();
                lastOutline.On();
            }
            else if (interact.tag == "ElectricalPanel")
            {
                lastOutline = interact.GetComponent<OutlineToggle>();
                lastOutline.On();
            }
            else if(lastOutline != null)
            {
                PText.SetActive(false);
                lastOutline.Off();
            }
        }
        else if (lastOutline != null)
        {
            PText.SetActive(false);
            lastOutline.Off();
        }
    }
}
