using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shield : Item
{
    private Vector3 original;
    private bool toggled = false;
    private bool primed = true;

    Brain mobBrain;

    //LEGACY DO NOT USE
    public Shield(GameObject stateManager, GameObject UIElement) : base(stateManager, UIElement){
        LoadItem("Shield");
        original = itemObj.transform.localPosition;
        ItemUI.transform.GetChild(0).gameObject.SetActive(true);
        gameState.ShieldObtained();
        PickupSFX();
        mobBrain = GameObject.FindGameObjectWithTag("Monster").GetComponentInChildren<Brain>();
    }

    public void setup(GameObject stateManager, GameObject UIElement)
    {
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        LoadItem("Shield");
        original = itemObj.transform.localPosition;
        ItemUI.transform.GetChild(0).gameObject.SetActive(true);
        gameState.ShieldObtained();
        mobBrain = GameObject.FindGameObjectWithTag("Monster").GetComponentInChildren<Brain>();
        PickupSFX();
        //I don't know why this was initializing as false, but this fixes it
        primed = true;
    }

    public override void Secondary(){
        toggled = !toggled ? true : false;
        // figure out which UI element is active and inactive
        int active = toggled ? 0 : 1;
        int inactive = toggled ? 1 : 0;
        // set active UI to be inactive and inactive to be active
        ItemUI.transform.GetChild(active).gameObject.SetActive(false);
        ItemUI.transform.GetChild(inactive).gameObject.SetActive(true);
        itemObj.transform.localPosition = toggled ? new Vector3((float) 0.36, (float) 0.43, (float) -0.54) : original;
    }

    public override bool IsToggled(){
        return toggled;
    }

    public bool explode(){
        if (primed)
        {
            Debug.Log("EXPLODING SHIELD");
            itemObj.GetComponentInChildren<ShieldDeployer>().Deploy();
            ExplodeSFX();
            mobBrain.isShielded = true;
            primed = false;
            Debug.Log("Primed: " + primed);

            //remove shield from inventrory
            Secondary();
            GameStateManager.ShieldAcquired = false;
            InventoryManager im = FindObjectOfType<InventoryManager>();
            int shieldIndex = im.inventory.getCurrentIndex();
            im.inventory.setToZeroth();
            im.inventory.items.RemoveAt(shieldIndex);
            Destroy(this);
            return true;
        }
        return false;
    }

    private void PickupSFX()
    {
        const string eventName = "event:/SFX/Items/Shield/Pickup-1";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(itemObj));
        sound.start();
        sound.release();
    }
    private void ExplodeSFX()
    {
        const string eventName = "event:/SFX/Items/Shield/Explode";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(itemObj.transform));
        sound.start();
        sound.release();
    }
}
