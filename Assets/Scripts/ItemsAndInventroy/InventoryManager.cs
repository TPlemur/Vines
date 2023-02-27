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

    [Header("Item PickUp Related")]
    public Camera playerCam;
    public LayerMask interactLayer;

    [Header("Electrical Equipment Related")]
    public LayerMask panelLayer;

    [Header("PVTM Related")]
    public GameObject RealPVTMCamera;
    public LayerMask  PVTMCamLayer;
    public LayerMask  MonsterPicLayer;
    public Material   FlashMat;



    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory();
        inventory.Add(new ElectricalEquipment(playerCam, panelLayer));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(interactKey)){
            Interact();
        }
        if(Input.GetKeyDown(cycleRightKey)){
            inventory.CycleRight();
        }
        else if(Input.GetKeyDown(cycleLeftKey)){
            inventory.CycleLeft();
        }
        if(Input.GetMouseButtonDown(0)){
            inventory.EquippedPrimary();
        }
        if(Input.GetMouseButtonDown(1)){
            inventory.EquippedSecondary();
        }
    }

    private void Interact(){
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f, interactLayer)){
            Debug.Log("INTERACTING");
            var interact = hit.transform;
            Debug.Log("HIT OBJECT NAMED " + interact.tag);
            inventory.UnEquipCurrent();
            if(interact.tag == "PVTM"){
                inventory.Add(new PVTM(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer, FlashMat));
                // ObjectiveScript.PVTMObjBool = false;
                // ObjectiveScript.DocumentObjBool = true;
            }
            if(interact.tag == "Shield"){
                inventory.Add(new Shield());
            }
            if(interact.tag == "Flashlight"){
                inventory.Add(new Flashlight());
                // ObjectiveScript.FlashObjBool = false;
                // ObjectiveScript.PowerObjBool = true;
            }
            // usingPVTM = false;
            Destroy(interact.gameObject);
        }
    }
}
