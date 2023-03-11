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
    
    public Shield(GameObject stateManager, GameObject UIElement) : base(stateManager, UIElement){
        LoadItem("Shield");
        original = itemObj.transform.localPosition;
        gameState.ShieldObtained();
        PickupSFX();
        mobBrain = GameObject.FindGameObjectWithTag("Monster").GetComponentInChildren<Brain>();
    }

    public override void Secondary(){
        toggled = !toggled ? true : false;
        string controls = toggled ? "RIGHT CLICK - UNPRIME SHIELD" : "RIGHT CLICK - PRIME SHIELD";
        ItemUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = controls;
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
            mobBrain.isShielded = true;
            primed = false;
            Debug.Log("Primed: " + primed);
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
}
