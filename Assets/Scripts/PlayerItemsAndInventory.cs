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
    public Camera playerCam;
    public LayerMask interactable;
    public GameObject RealPVTMCamera;
    public LayerMask PVTMCameras;

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
                // inventory.ChangeEquipped(CycleDirection.RIGHT);
                inventory.CycleEquippedItem(CycleDirection.RIGHT);
            }
            else if(Input.GetKeyDown(cycleLeftKey)){
                // inventory.ChangeEquipped(CycleDirection.LEFT);
                inventory.CycleEquippedItem(CycleDirection.LEFT);
            }
        }
        if(usingPVTM){
            if(Input.GetKeyDown(cycleRightKey)){
                // int index = inventory.equippedIndex;
                // var pvtm = inventory.items[index];
                // pvtm.ChangeCam(CycleDirection.RIGHT);
                Debug.Log("GOING RIGHT");
                inventory.equippedItem.ChangeCam(CycleDirection.RIGHT);
                // inventory.CycleEquippedItem(CycleDirection.RIGHT);
            }
            else if(Input.GetKeyDown(cycleLeftKey)){
                // int index = inventory.equippedIndex;
                // inventory.items[index].ChangeCam(CycleDirection.LEFT);
                Debug.Log("GOING LEFT");
                inventory.equippedItem.ChangeCam(CycleDirection.LEFT);
                // inventory.CycleEquippedItem(CycleDirection.LEFT);
            }
        }

        // PRESSED
        if(Input.GetMouseButtonDown(0)){ // left click
            inventory.ItemPrimaryPressed();
        }
        if(Input.GetMouseButtonDown(1)){ // right click
            // inventory.ItemSecondaryPressed();
            inventory.UseEquippedItem();
        }

        // NOT PRESSED
        // if(Input.GetMouseButtonDown(0)){ // left click
        //     inventory.ItemPrimaryPressed();
        // }
        // if(Input.GetMouseButtonDown(1)){ // right click
        //     inventory.ItemSecondaryPressed();
        // }
    }

    private void Interact(){
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f, interactable)){
            var interact = hit.transform;
            Debug.Log("HIT OBJECT NAMED " + interact.tag);
            if(interact.tag == "PVTM"){
                inventory.AddItem(new PVTM(playerCam, PVTMCameras, RealPVTMCamera));
            }
            if(interact.tag == "Med Kit"){
                inventory.AddItem(new MedKit());
            }
            if(interact.tag == "Flashlight"){
                inventory.AddItem(new Flashlight());
            }
            Destroy(interact.gameObject);
        }
    }
}

// inventory  class which manages adding items, swaping items and using them
public class Inventory{
    public List<Item> items = new List<Item>();
    public int equippedIndex = -1;
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

    // PRESSED
    public void ItemPrimaryPressed(){
        if(equippedIndex != -1){
            equippedItem.Primary();
        }
    }

    public void ItemSecondaryPressed(){
        if(equippedIndex != -1){
            equippedItem.Secondary();
        }
    }

    // public void ChangeEquipped(CycleDirection dir){
    //     equippedItem.Dequip();
    //     equippedIndex = this.Cycle(dir, items, equippedIndex);
    //     Debug.Log("HERE" + equippedIndex);
    //     items[equippedIndex].Equip();
    // }

    // NOT PRESSED
    // public void ItemPrimaryPressed(){
    //     item[equippedIndex].Primary();
    // }

    // public void ItemSecondaryPressed(){
    //     item[equippedIndex].Secondary();
    // }

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
    public virtual void Primary(){
        // overwritten by sub class
    }
    public virtual void Secondary(){
        // overwritten by sub class
    }
    public virtual void ChangeCam(CycleDirection dir){
        // overwritten
    }
    public GameObject itemLoad(string objName){
        var loadedItem = Resources.Load("Prefabs/" + objName);
        GameObject iPos = GameObject.Find("ItemPos");
        GameObject itemInst = (GameObject)Instantiate(loadedItem, iPos.transform.position, iPos.transform.rotation, iPos.transform);
        itemInst.GetComponent<Collider>().enabled = false;
        return itemInst;
        return null;
    }
}

// PVTM subclass
public class PVTM : Item{
    // PVMT GameObject
    GameObject PVTMinst = null;
    Vector3 PVTMorigPos;

    // Accessible Camera Related
    List<GameObject> activeCams = new List<GameObject>();
    Camera playerCam;
    GameObject real;
    LayerMask mask;
    GameObject currentCam;
    int currentIndex = -1;

    public PVTM(Camera cam, LayerMask camMask, GameObject realPVTM){
        playerCam = cam;
        mask = camMask;
        real = realPVTM;
    }

    public override void Equip(){
        // code/animation/sound for equipping PVTM
        if (PVTMinst == null){
            PVTMinst = itemLoad("PVTM_orig");
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

    public override void Primary(){
        // shoot raycast
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 10f, mask)){
            GameObject interact = hit.transform.gameObject;
            interact.gameObject.GetComponent<Collider>().enabled = false;
            activeCams.Add(interact);
            currentCam = activeCams[activeCams.Count - 1];
            real.transform.SetParent(interact.transform);
            real.transform.localPosition = interact.transform.GetChild(0).gameObject.transform.localPosition;
            real.transform.localRotation = interact.transform.GetChild(0).gameObject.transform.localRotation;
        }
        return;
    }

    public override void Secondary(){
        return;
    }

    public override void ChangeCam(CycleDirection dir){
        // move real cam to here
        if(activeCams.Count > 1){
            if(dir == CycleDirection.LEFT){
                // subtract indexes and move to the left
                // -1 * (items.Count - 1) subtracts a negative number to
                // equippedIndex and puts us back at the index items.Count - 1
                currentIndex -= currentIndex == 0 ? -1 * (activeCams.Count - 1) : 1;
            }
            else if(dir == CycleDirection.RIGHT){
                // add indexes and move to the right
                // -1 * (items.Count - 1) adds a negative number to
                // equippedIndex and puts us back at the index 0
                currentIndex += currentIndex == activeCams.Count - 1 ? -1 * (activeCams.Count - 1) : 1;
            }
            currentCam = activeCams[currentIndex];
            real.transform.SetParent(currentCam.transform);
            real.transform.localPosition = currentCam.transform.GetChild(0).gameObject.transform.localPosition;
            real.transform.localRotation = currentCam.transform.GetChild(0).gameObject.transform.localRotation;
        }
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