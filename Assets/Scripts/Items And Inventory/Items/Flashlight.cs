using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : Item
{
    GameObject light;
    bool toggled = true;

    public Flashlight(GameObject stateManager) : base(stateManager){
        LoadItem("Flashlight");
        light = itemObj.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        gameState.FlashlightObtained();
    }

    public override void Primary(){
        toggled = !toggled ? true : false;
        light.SetActive(toggled);
        SwitchSFX();
    }

    // SFX
    private void SwitchSFX()
    {
        // flashlight switch sound
        const string eventName = "event:/SFX/Items/Flashlight/Switch-1";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
}
