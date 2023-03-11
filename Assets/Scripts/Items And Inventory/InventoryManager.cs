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
    public GameObject GameStateManager;

    void Start()
    {
        inventory = new Inventory();
        inventory.Add(new ElectricalEquipment(playerCam, panelLayer, GameStateManager, Electrical_Controls));
    }

    void Update()
    {
        CheckOutlines();
        // interact
        if (Input.GetKeyDown(interactKey)){
            Interact();
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
        if(Input.GetKeyDown(KeyCode.Alpha7)){
            inventory.Add(new ElectricalEquipment(playerCam, panelLayer, GameStateManager, Electrical_Controls));
        }
        if(Input.GetKeyDown(KeyCode.Alpha8)){
            inventory.Add(new PVTM(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer, FlashMat, GameStateManager, PVTM_Controls));
        }
        if(Input.GetKeyDown(KeyCode.Alpha9)){
            inventory.Add(new Shield(GameStateManager, Shield_Controls));
        }
        if(Input.GetKeyDown(KeyCode.Alpha0)){
            inventory.Add(new Flashlight(GameStateManager, Flashlight_Controls));
        }
    }

    // Shoot Raycast to detect items that can be picked up
    private void Interact(){
        if (PText.activeSelf) { PText.SetActive(false); }
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f, interactLayer)){
            var interact = hit.transform;
            if (interact.tag == "PVTM"){
                inventory.Add(new PVTM(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer, FlashMat, GameStateManager, PVTM_Controls));
            }
            if(interact.tag == "Shield"){
                inventory.Add(new Shield(GameStateManager, Shield_Controls));
            }
            if(interact.tag == "Flashlight"){
                inventory.Add(new Flashlight(GameStateManager, Flashlight_Controls));
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
            else if(lastOutline != null)
            {
                PText.SetActive(false);
                lastOutline.Off();
            }
        }
    }
}
