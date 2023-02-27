using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Inventory inventory;

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
        inventory.Add(new ElectricalEquipment(playerCam, panelLayer, GameStateManager));
    }

    void Update()
    {
        // interact
        if(Input.GetKeyDown(interactKey)){
            Interact();
        }

        // cycle left and right
        if(Input.GetKeyDown(cycleRightKey)){
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
            inventory.Add(new ElectricalEquipment(playerCam, panelLayer, GameStateManager));
        }
        if(Input.GetKeyDown(KeyCode.Alpha8)){
            inventory.Add(new PVTM(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer, FlashMat, GameStateManager));
        }
        if(Input.GetKeyDown(KeyCode.Alpha9)){
            inventory.Add(new Shield(GameStateManager));
        }
        if(Input.GetKeyDown(KeyCode.Alpha0)){
            inventory.Add(new Flashlight(GameStateManager));
        }
    }

    // Shoot Raycast to detect items that can be picked up
    private void Interact(){
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f, interactLayer)){
            var interact = hit.transform;
            if(interact.tag == "PVTM"){
                inventory.Add(new PVTM(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer, FlashMat, GameStateManager));
            }
            if(interact.tag == "Shield"){
                inventory.Add(new Shield(GameStateManager));
            }
            if(interact.tag == "Flashlight"){
                inventory.Add(new Flashlight(GameStateManager));
            }
            Destroy(interact.gameObject);
        }
    }
}
