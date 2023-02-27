using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Player Camera Related")]
    public Camera playerCam;

    [Header("PVTM Related")]
    public LayerMask interactLayer;
    public GameObject RealPVTMCamera;
    public LayerMask PVTMCamLayer;
    public LayerMask MonsterPicLayer;
    public Material FlashMat;

    [Header("Generator Related")]
    public GameObject warehouseObj;
    public LayerMask electricalLayer;
    public static bool generatorOn = false;
    public bool generatorHuntOn = false;
    public static bool picTaken = false;
    public bool validPic = false;

    public Inventory inventory;

    public static bool usingShield = false;
    public static bool usingPVTM = false;
    public static bool isFlash = false;
    private float flashTimer = 0;

    private Color color;

    void Start(){
        Debug.Log("BRUH");
    }
}    

//     void Start(){
//         inventory = new Inventory();

//         inventory.AddItem(new ElectricalEquipment(warehouseObj, electricalLayer, playerCam));
//     }

//     void Update()
//     {
//         if(generatorOn){
//             generatorHuntOn = true;
//         }
//         if(picTaken){
//             validPic = true;
//         }
//         // check for inputs 
//         if (!usingPVTM){
//             if(Input.GetKeyDown(interactKey)){
//                 Interact();
//             }
//             if(Input.GetKeyDown(cycleRightKey)){
//                 inventory.CycleEquippedItem(CycleDirection.RIGHT);
//             }
//             else if(Input.GetKeyDown(cycleLeftKey)){
//                 inventory.CycleEquippedItem(CycleDirection.LEFT);
//             }
//         }
//         if (usingPVTM){
//             if(Input.GetKeyDown(cycleRightKey)){
//                 inventory.equippedItem.ChangeCam(CycleDirection.RIGHT);
//             }
//             else if(Input.GetKeyDown(cycleLeftKey)){
//                 inventory.equippedItem.ChangeCam(CycleDirection.LEFT);
//             }
//         }

//         //Pic flash code (needs debugging)
//         if (isFlash){
//             if (flashTimer < .1f){
//                 color = FlashMat.color;
//                 color.a = 1f;
//                 FlashMat.color = color;
//             }
//             if (color.a > .01f){
//                 color = FlashMat.color;
//                 color.a -= .75f * Time.deltaTime;
//                 FlashMat.color =  color;
//             }
//             flashTimer += Time.deltaTime;
//             if (flashTimer > 1.5f){
//                 isFlash = false;
//             }
//         }
//         if (!isFlash){
//             color = FlashMat.color;
//             color.a = 0;
//             FlashMat.color = color;
//             flashTimer = 0;
//         }
//         //Debug.Log(color.a);

//         // PRESSED
//         if(Input.GetMouseButtonDown(0)){ // left click
//             inventory.ItemPrimaryPressed();
//         }
//         if(Input.GetMouseButtonDown(1)){ // right click
//             // inventory.ItemSecondaryPressed();
//             inventory.UseEquippedItem();
//         }

//         // NOT PRESSED
//         // if(Input.GetMouseButtonDown(0)){ // left click
//         //     inventory.ItemPrimaryPressed();
//         // }
//         // if(Input.GetMouseButtonDown(1)){ // right click
//         //     inventory.ItemSecondaryPressed();
//         // }
//     }

//     private void Interact(){
//         RaycastHit hit;
//         if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f, interactLayer)){
//             var interact = hit.transform;
//             Debug.Log("HIT OBJECT NAMED " + interact.tag);
//             if(interact.tag == "PVTM"){
//                 inventory.AddItem(new PVTM(playerCam, PVTMCamLayer, RealPVTMCamera, MonsterPicLayer));
//                 ObjectiveScript.PVTMObjBool = false;
//                 ObjectiveScript.DocumentObjBool = true;
//             }
//             if(interact.tag == "Shield"){
//                 inventory.AddItem(new Shield());
//             }
//             if(interact.tag == "Flashlight"){
//                 inventory.AddItem(new Flashlight());
//                 ObjectiveScript.FlashObjBool = false;
//                 ObjectiveScript.PowerObjBool = true;
//             }
//             usingPVTM = false;
//             Destroy(interact.gameObject);
//         }
//     }
// }

// // inventory  class which manages adding items, swaping items and using them
// public class Inventory{
//     public List<Item> items = new List<Item>();
//     public int equippedIndex = -1;
//     public Item equippedItem;

//     public void AddItem(Item item){
//         if(items.Count > 0){
//             equippedItem.Dequip();
//         }
//         items.Add(item);
//         equippedItem = item;
//         item.Equip();
//         equippedIndex += 1;
//     }

//     public void UseEquippedItem(){
//         if(equippedIndex != -1){
//             equippedItem.Use();
//         }
//     }

//     // PRESSED
//     public void ItemPrimaryPressed(){
//         if(equippedIndex != -1){
//             equippedItem.Primary();
//         }
//     }

//     public void ItemSecondaryPressed(){
//         if(equippedIndex != -1){
//             equippedItem.Secondary();
//         }
//     }

//     // public void ChangeEquipped(CycleDirection dir){
//     //     equippedItem.Dequip();
//     //     equippedIndex = this.Cycle(dir, items, equippedIndex);
//     //     Debug.Log("HERE" + equippedIndex);
//     //     items[equippedIndex].Equip();
//     // }

//     // NOT PRESSED
//     // public void ItemPrimaryPressed(){
//     //     item[equippedIndex].Primary();
//     // }

//     // public void ItemSecondaryPressed(){
//     //     item[equippedIndex].Secondary();
//     // }

//     public void CycleEquippedItem(CycleDirection dir){
//         if(items.Count > 1){
//             equippedItem.Dequip();
//             if(dir == CycleDirection.LEFT){
//                 // subtract indexes and move to the left
//                 // -1 * (items.Count - 1) subtracts a negative number to
//                 // equippedIndex and puts us back at the index items.Count - 1
//                 equippedIndex -= equippedIndex == 0 ? -1 * (items.Count - 1) : 1;
//             }
//             else if(dir == CycleDirection.RIGHT){
//                 // add indexes and move to the right
//                 // -1 * (items.Count - 1) adds a negative number to
//                 // equippedIndex and puts us back at the index 0
//                 equippedIndex += equippedIndex == items.Count - 1 ? -1 * (items.Count - 1) : 1;
//             }
//             equippedItem = items[equippedIndex];
//             equippedItem.Equip();
//         }
//     }
// }

// // Item base class mainly used for clarity and the
// // ability to store items of different types to a list
// public class Item : MonoBehaviour{
//     public virtual void Equip(){
//         // overwritten by sub class
//     }
//     public virtual void Dequip(){
//         // overwritten by sub class
//     }
//     public virtual void Use(){
//         // overwritten by sub class
//     }
//     public virtual void Primary(){
//         // overwritten by sub class
//     }
//     public virtual void Secondary(){
//         // overwritten by sub class
//     }
//     public virtual void ChangeCam(CycleDirection dir){
//         // overwritten
//     }
//     public GameObject itemLoad(string objName){
//         var loadedItem = Resources.Load("Prefabs/" + objName);
//         GameObject iPos = GameObject.Find("ItemPos");
//         GameObject itemInst = (GameObject)Instantiate(loadedItem, iPos.transform.position, iPos.transform.rotation, iPos.transform);
//         //itemInst.GetComponent<Collider>().enabled = false;
//         return itemInst;
//         //return null;
//     }
// }

// // PVTM subclass
// public class PVTM : Item{
//     // PVMT GameObject
//     GameObject PVTMinst = null;
//     Vector3 PVTMorigPos;

//     // Accessible Camera Related
//     List<GameObject> activeCams = new List<GameObject>();
//     Camera playerCam;
//     GameObject real;
//     LayerMask layer;
//     LayerMask monsterPic;
//     GameObject currentCam;
//     int currentIndex = -1;

//     public PVTM(Camera cam, LayerMask camMask, GameObject realPVTM, LayerMask monPic){
//         playerCam = cam;
//         layer = camMask;
//         real = realPVTM;
//         monsterPic = monPic;
//     }

//     public override void Equip(){
//         // code/animation/sound for equipping PVTM
//         if (PVTMinst == null){
//             PVTMinst = itemLoad("PVTM_Prefab");
//             PVTMorigPos = PVTMinst.transform.localPosition;
//         }
//         else{
//             PVTMinst.SetActive(true);
//         }
//         Debug.Log("EQUIPPING PVTM");
//     }

//     public override void Dequip(){
//         PVTMinst.SetActive(false);
//     }

//     public override void Primary(){
//         // shoot raycast
//         if (!PlayerItemsAndInventory.usingPVTM && PlayerItemsAndInventory.generatorOn){
//             RaycastHit hit;
//             if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 10f, layer)){
//                 GameObject interact = hit.transform.gameObject;
//                 interact.gameObject.GetComponent<Collider>().enabled = false;
//                 activeCams.Add(interact);
//                 currentCam = activeCams[activeCams.Count - 1];
//                 real.transform.SetParent(interact.transform);
//                 real.transform.localPosition = interact.transform.GetChild(0).gameObject.transform.localPosition;
//                 real.transform.localRotation = interact.transform.GetChild(0).gameObject.transform.localRotation;

//                 CameraWhirUpSFX(interact);
//             }
//         }
//         //Take pic
//         else{
//             if (!PlayerItemsAndInventory.isFlash && PlayerItemsAndInventory.usingPVTM && PlayerItemsAndInventory.generatorOn){
//                 PlayerItemsAndInventory.isFlash = true;
//                 PicSFX();
//                 RaycastHit hit;
//                 if(Physics.Raycast(real.transform.position, real.transform.forward, out hit, 25f, monsterPic)){
//                     Debug.Log("VALID MONSTER PICTURE ACQUIRED");
//                     PlayerItemsAndInventory.picTaken = true;
//                     ObjectiveScript.DocumentObjBool = false;
//                     ObjectiveScript.EscapeObjBool = true;
//                 }
//             }
//         }
//         return;
//     }

//     public override void Secondary(){
//         return;
//     }

//     public override void ChangeCam(CycleDirection dir){
//         // move real cam to here
//         if(activeCams.Count > 1){
//             if(dir == CycleDirection.LEFT){
//                 currentIndex -= currentIndex == 0 ? -1 * (activeCams.Count - 1) : 1;
//             }
//             else if(dir == CycleDirection.RIGHT){
//                 currentIndex += currentIndex == activeCams.Count - 1 ? -1 * (activeCams.Count - 1) : 1;
//             }
//             currentCam = activeCams[currentIndex];
//             real.transform.SetParent(currentCam.transform);
//             real.transform.localPosition = currentCam.transform.GetChild(0).gameObject.transform.localPosition;
//             real.transform.localRotation = currentCam.transform.GetChild(0).gameObject.transform.localRotation;

//             CameraChangeSFX();
//         }
//     }

//     public override void Use(){
//         // code/animation/sound for using PVTM
//         if (!PlayerItemsAndInventory.usingPVTM){
//             PlayerItemsAndInventory.usingPVTM = true;
//             PVTMinst.transform.localPosition = new Vector3((float) 0.89, (float) 0.23, (float) -0.215);
//             //PVTMinst.transform.localPosition = new Vector3((float) 0.23, (float) 0.23, (float) -0.45);
//             //PVTMinst.transform.localRotation = Quaternion.Euler(0, 0, -15);
//         }
//         else{
//             PlayerItemsAndInventory.usingPVTM = false;
//             PVTMinst.transform.localPosition = PVTMorigPos;
//             //PVTMinst.transform.localRotation = Quaternion.Euler(0, 0, 0);
//         }
//         Debug.Log("USING PVTM");

//         CameraChangeSFX();
//     }

//     private void CameraWhirUpSFX(GameObject cam)
//     {
//         // camera whir up sound
//         const string eventName = "event:/SFX/Items/Camera/WhirUp";
//         var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
//         sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(cam));
//         sound.start();
//         sound.release();
//     }
//     private void PicSFX()
//     {
//         // pic sound
//         const string eventName = "event:/SFX/Items/Camera/Shutter2D-2";
//         var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
//         sound.start();
//         sound.release();
//     }
//     private void CameraChangeSFX()
//     {
//         // camera change noise sound
//         const string eventName = "event:/SFX/Items/Camera/WhiteNoiseStatic-2";
//         var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
//         sound.start();
//         sound.release();
//     }
// }

// // Shield subclass
// public class Shield : Item{
//     GameObject SHinst = null;
//     Vector3 SHorigPos;

//     public override void Equip(){
//         // code/animation/sound for equipping shield
//         if (SHinst == null){
//             SHinst = itemLoad("Shield");
//             SHorigPos = SHinst.transform.localPosition;
//         }
//         else{
//             SHinst.SetActive(true);
//         }
//         Debug.Log("EQUIPPING SHIELD");
//     }

//     public override void Dequip(){
//         SHinst.SetActive(false);
//     }

//     public override void Use(){
//         // code/animation/sound for using shield
//         if (!PlayerItemsAndInventory.usingShield){
//             PlayerItemsAndInventory.usingShield = true;
//             SHinst.transform.localPosition = new Vector3((float) 0.36, (float) 0.43, (float) -0.54);
//         }
//         else{
//             PlayerItemsAndInventory.usingShield = false;
//             SHinst.transform.localPosition = SHorigPos;
//         }
//         Debug.Log("USING SHIELD");
//     }

//     public bool explode()
//     {
//         return SHinst.GetComponentInChildren<ShieldDeployer>().findWidth(SHinst.transform);
//     }
// }

// // Flashlight subclass
// public class Flashlight : Item{
//     GameObject FLinst = null;

//     public override void Equip(){
//         // code/animation/sound for equipping flashlight
//         if (FLinst == null){
//             FLinst = itemLoad("Flashlight");
//         }
//         else{
//             FLinst.SetActive(true);
//         }
//         Debug.Log("EQUIPPING FLASHLIGHT");
//     }

//     public override void Dequip(){
//         FLinst.SetActive(false);
//     }

//     public override void Use(){
//         // code/animation/sound for using flashlight
//         if (FLinst.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.activeSelf == false){
//             FLinst.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
//         }
//         else{
//             FLinst.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
//         }
//         Debug.Log("USING FLASHLIGHT");

//         SwitchSFX();
//     }

//     private void SwitchSFX()
//     {
//         // flashlight switch sound
//         const string eventName = "event:/SFX/Items/Flashlight/Switch-1";
//         var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
//         sound.start();
//         sound.release();
//     }
// }

// public class ElectricalEquipment : Item {
//     GameObject warehouseObj = null;
//     GameObject EEinst = null;
//     Camera playerCam;
//     LayerMask layer;

//     public ElectricalEquipment(GameObject warehouse, LayerMask interactLayer, Camera cam){
//         this.warehouseObj = warehouse;
//         this.playerCam = cam;
//         this.layer = interactLayer;
//     }

//     public override void Equip(){
//         if (EEinst == null){
//             EEinst = itemLoad("ElectricalDevice");
//         }
//         else{
//             EEinst.SetActive(true);
//         }
//         Debug.Log("EQUIPPING EE");
//     }

//     public override void Dequip(){
//         EEinst.SetActive(false);
//         //Debug.Log("UNEQUIPPING EE");
//     }

//     public override void Primary(){
//         Debug.Log("USING EE");
//         RaycastHit hit;
//         if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 2.5f, layer)){
//             GameObject panel = hit.transform.gameObject;
//             // call TurnOnLights() inside of warehouse
//             Debug.Log("hit panel");
//             // mark medium successful hit spark sfx
//             SparkMediumSFX();
//             if(panel.tag == "ElectricalPanel"){
//                 // call TurnOnLights() inside of warehouse
//                 PlayerItemsAndInventory.generatorOn = true;
//                 warehouseObj.gameObject.GetComponent<WarehouseMaker>().warehouse.TurnOnLights();
//                 //Destroy(panel.gameObject); -- don't destroy just disabled collider because the object holds some audio components
//                 panel.GetComponent<Collider>().enabled = false;
//                 TurnOnGeneratorSFX(panel);
//                 ObjectiveScript.PowerObjBool = false;
//                 ObjectiveScript.PVTMObjBool = true;
//             }
//             if(panel.tag == "ContainmentButton"){
//                 if(MonsterCheck.isMonsterInside){
//                     SceneManager.LoadScene(2);
//                 }
//             }
//         }
//         else
//         {
//             // make quick no-hit spark sfx
//             SparkShortSFX();
//         }
//     }

//     private void SparkShortSFX()
//     {
//         const string eventName = "event:/SFX/Items/Electrical/Spark Short";
//         var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
//         sound.start();
//         sound.release();
//     }
//     private void SparkMediumSFX()
//     {
//         const string eventName = "event:/SFX/Items/Electrical/Spark Medium";
//         var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
//         sound.start();
//         sound.release();
//     }
//     private void TurnOnGeneratorSFX(GameObject panel)
//     {
//         foreach (var emitter in panel.GetComponents<FMODUnity.StudioEventEmitter>())
//         {
//             emitter.Play();
//         }
//     }
// }