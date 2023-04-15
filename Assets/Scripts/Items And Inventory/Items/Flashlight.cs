using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : Item
{
    GameObject light;
    bool toggled = true;

    public Flashlight(GameObject stateManager, GameObject UIElement) : base(stateManager, UIElement){
        LoadItem("Flashlight");
        light = itemObj.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        gameState.FlashlightObtained();
        PickupSFX();
    }

    public static Flashlight foobar(GameObject stateManager, GameObject UIElement)
    {
        GameObject fb = new GameObject();
        fb.AddComponent<Flashlight>();
        Flashlight fl = fb.GetComponent<Flashlight>();
        fl.setup(stateManager, UIElement);
        return fl;
        
    }

    public void setup(GameObject stateManager, GameObject UIElement)
    {
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        LoadItem("Flashlight");
        light = itemObj.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        gameState.FlashlightObtained();
        PickupSFX();
    }

    public override void Primary(){
        toggled = !toggled ? true : false;
        light.SetActive(toggled);
        SwitchSFX();
    }

    // SFX
    private void PickupSFX()
    {
        const string eventName = "event:/SFX/Items/Inventory/Bag Pickup";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
    private void SwitchSFX()
    {
        // flashlight switch sound
        const string eventName = "event:/SFX/Items/Flashlight/Switch-1";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
}
