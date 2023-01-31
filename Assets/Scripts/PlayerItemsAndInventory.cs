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

    public ItemInventory inventory = new ItemInventory();

    void Update()
    {
        // check for inputs 
        if(Input.GetKeyDown(interactKey)){
            Interact();
        }
        if(Input.GetKeyDown(cycleRightKey)){
            inventory.CycleEquippedItem(CycleDirection.RIGHT);
        }
        else if(Input.GetKeyDown(cycleLeftKey)){
            inventory.CycleEquippedItem(CycleDirection.LEFT);
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

public class ItemInventory{
    public List<Item> items = new List<Item>();
    private int equippedIndex = -1;
    public Item equippedItem;

    public void AddItem(Item item){
        item.obtained = true;
        items.Add(item);
        if(items.Count == 1){
            equippedItem = item;
            item.Equip();
            equippedIndex = 0;
        }
    }

    public void UseEquippedItem(){
        if(equippedIndex != -1){
            items[equippedIndex].Use();
        }
    }

    public void CycleEquippedItem(CycleDirection dir){
        if(items.Count > 1){
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
public class Item{
    public bool obtained = false;
    public string name = "";
    public virtual void Equip(){
        // overwritten by sub class
    }
    public virtual void Use(){
        // overwritten by sub class
    }
}

// PVTM subclass
public class PVTM : Item{
    public new string name = "PVTM";
    public override void Equip(){
        // code/animation/sound for equipping PVTM
        Debug.Log("EQUIPPING PVTM");
    }
    public override void Use(){
        // code/animation/sound for using PVTM
        Debug.Log("USING PVTM");
    }
}

// MedKit subclass
public class MedKit : Item{
    public new string name = "Med Kit";
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
    public new string name = "Flashlight";
    public override void Equip(){
        // code/animation/sound for equipping med kit
        Debug.Log("EQUIPPING FLASHLIGHT");
    }
    public override void Use(){
        // code/animation/sound for using flashlight
        Debug.Log("USING FLASHLIGHT");
    }
}