using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CycleDirection
{
    LEFT,
    RIGHT
}

public class PlayerItemsAndInventory : MonoBehaviour
{
    [Header("Input")]
    public KeyCode interactKey = KeyCode.F;
    public KeyCode cycleRightKey = KeyCode.E;
    public KeyCode cycleLeftKey = KeyCode.Q;

    [Header("Camera Related")]
    public Camera mainCamera;
    public LayerMask interactableLayer;

    public Inventory inventory = new Inventory();

    public static bool usingPVTM = false;

    void Update()
    {
        // check for inputs 
        if (!usingPVTM){
            if(Input.GetKeyDown(interactKey)){
                Interact();
            }
            if(Input.GetKeyDown(cycleRightKey)){
                inventory.CycleEquippedItem(CycleDirection.RIGHT);
            }
            else if(Input.GetKeyDown(cycleLeftKey)){
                inventory.CycleEquippedItem(CycleDirection.LEFT);
            }
        }
        if(Input.GetMouseButtonDown(1)){ // right click
            inventory.UseEquippedItem();
        }
    }

    private void Interact(){
        RaycastHit hit;
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 2.5f, interactableLayer)){
            var intereractableObject = hit.transform;
            Debug.Log("HIT OBJECT NAMED " + intereractableObject.tag);
            if(intereractableObject.tag == "PVTM"){
                inventory.AddItem(new PVTM());
            }
            if(intereractableObject.tag == "Med Kit"){
                inventory.AddItem(new MedKit());
            }
            if(intereractableObject.tag == "Flashlight"){
                inventory.AddItem(new Flashlight());
            }
            Destroy(intereractableObject.gameObject);
        }
    }
}

// inventory  class which manages adding items, swaping items and using them
public class Inventory{
    public List<Item> items = new List<Item>();
    private int equippedIndex = -1;
    public Item equippedItem;

    public void AddItem(Item item){
        if(items.Count > 0){
            equippedItem.Dequip();
        }
        items.Add(item);
        equippedItem = item;
        item.Equip();
        equippedIndex += 1;
    }

    public void UseEquippedItem(){
        if(equippedIndex != -1){
            items[equippedIndex].Use();
        }
    }

    public void CycleEquippedItem(CycleDirection dir){
        if(items.Count > 1){
            equippedItem.Dequip();
            if(dir == CycleDirection.LEFT){
                // subtract indexes and move to the left
                // -1 * (items.Count - 1) subtracts a negative number to
                // equippedIndex and puts us back at the index items.Count - 1
                equippedIndex -= equippedIndex == 0 ? -1 * (items.Count - 1) : 1;
            }
            else if(dir == CycleDirection.RIGHT){
                // add indexes and move to the right
                // -1 * (items.Count - 1) adds a negative number to
                // equippedIndex and puts us back at the index 0
                equippedIndex += equippedIndex == items.Count - 1 ? -1 * (items.Count - 1) : 1;
            }
            equippedItem = items[equippedIndex];
            equippedItem.Equip();
        }
    }
}

// Item base class mainly used for clarity and the
// ability to store items of different types to a list
public class Item : MonoBehaviour{
    public virtual void Equip(){
        // overwritten by sub class
    }
    public virtual void Dequip(){
        // overwritten by sub class
    }
    public virtual void Use(){
        // overwritten by sub class
    }
    public GameObject itemLoad(string objName){
        var loadedItem = Resources.Load("Prefabs/" + objName);
        GameObject iPos = GameObject.Find("ItemPos");
        GameObject itemInst = (GameObject)Instantiate(loadedItem, iPos.transform.position, iPos.transform.rotation, iPos.transform);
        itemInst.GetComponent<Collider>().enabled = false;
        return itemInst;
    }
}

// PVTM subclass
public class PVTM : Item{
    GameObject PVTMinst = null;
    Vector3 PVTMorigPos;
    public override void Equip(){
        // code/animation/sound for equipping PVTM
        if (PVTMinst == null){
            PVTMinst = itemLoad("PVTM");
            PVTMorigPos = PVTMinst.transform.localPosition;
        }
        else{
            PVTMinst.SetActive(true);
        }
        Debug.Log("EQUIPPING PVTM");
    }
    public override void Dequip(){
        PVTMinst.SetActive(false);
    }
    public override void Use(){
        // code/animation/sound for using PVTM
        if (!PlayerItemsAndInventory.usingPVTM){
            PlayerItemsAndInventory.usingPVTM = true;
            PVTMinst.transform.localPosition = new Vector3((float) 0.23, (float) 0.23, (float) -0.45);
            PVTMinst.transform.localRotation = Quaternion.Euler(0, 0, -15);
        }
        else{
            PlayerItemsAndInventory.usingPVTM = false;
            PVTMinst.transform.localPosition = PVTMorigPos;
            PVTMinst.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        Debug.Log("USING PVTM");
    }
}

// MedKit subclass
public class MedKit : Item{
    public override void Equip(){
        // code/animation/sound for equipping med kit
        Debug.Log("EQUIPPING MED KIT");
    }
    public override void Use(){
        // code/animation/sound for using med kit
        Debug.Log("USING MED KIT");
    }
}

// Flashlight subclass
public class Flashlight : Item{
    GameObject FLinst = null;
    public override void Equip(){
        // code/animation/sound for equipping flashlight
        if (FLinst == null){
            FLinst = itemLoad("Flashlight");
        }
        else{
            FLinst.SetActive(true);
        }
        Debug.Log("EQUIPPING FLASHLIGHT");
    }
    public override void Dequip(){
        FLinst.SetActive(false);
    }
    public override void Use(){
        // code/animation/sound for using flashlight
        if (FLinst.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.activeSelf == false){
            FLinst.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        else{
            FLinst.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        Debug.Log("USING FLASHLIGHT");
    }
}