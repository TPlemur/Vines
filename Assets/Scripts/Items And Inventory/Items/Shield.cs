using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Item
{
    private Vector3 original;
    private bool toggled = false;

    public Shield(GameObject stateManager) : base(stateManager){
        LoadItem("Shield");
        original = itemObj.transform.localPosition;
        gameState.ShieldObtained();
        PickupSFX();
    }

    public override void Secondary(){
        toggled = !toggled ? true : false;
        itemObj.transform.localPosition = toggled ? new Vector3((float) 0.36, (float) 0.43, (float) -0.54) : original;
    }

    public override bool IsToggled(){
        return toggled;
    }

    public bool explode(){
        Debug.Log("EXPLODING SHIELD");
        return itemObj.GetComponentInChildren<ShieldDeployer>().findWidth(itemObj.transform);
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
