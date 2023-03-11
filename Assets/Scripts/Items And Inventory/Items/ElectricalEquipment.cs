using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElectricalEquipment : Item
{
    LayerMask layer;

    public ElectricalEquipment(Camera cam, LayerMask mask, GameObject stateManager, GameObject UIElement) : base(cam, stateManager, UIElement){
        layer = mask;
        LoadItem("ElectricalDevice");
        //PickupSFX(); you start with the item
    }

    public override void Primary(){
        GameObject obj = ShootRaycast(playerCam, 2.5f, layer);
        if(obj != null){
            if(obj.tag == "ElectricalPanel"){
                SocketTesterSFX();
                gameState.PowerRestored();
                //Destroy(panel.gameObject); -- don't destroy just disabled collider because the object holds some audio components
                obj.transform.parent.GetComponent<GeneratorVibe>().enabled = true;
                obj.GetComponent<Collider>().enabled = false;
                TurnOnGeneratorSFX(obj);

            }
            if(obj.tag == "ContainmentButton" && gameState.IsPowerRestored()){
                if(MonsterCheck.isMonsterInside){
                    gameState.SplitjawContained();
                    SceneManager.LoadScene(2);
                }
            }
        }
        else{
            SocketTesterSFX();
        }
    }

    // SFX
    private void PickupSFX()
    {
        const string eventName = "event:/SFX/Items/Inventory/Bag Pickup";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
    private void SparkShortSFX()
    {
        const string eventName = "event:/SFX/Items/Electrical/Spark Short";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
    private void SparkMediumSFX()
    {
        const string eventName = "event:/SFX/Items/Electrical/Spark Medium";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
    private void SocketTesterSFX()
    {
        const string eventName = "event:/SFX/Items/Electrical/Socket Tester";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }

    private void TurnOnGeneratorSFX(GameObject panel)
    {
        SparkMediumSFX();
        foreach (var emitter in panel.GetComponents<FMODUnity.StudioEventEmitter>())
        {
            emitter.Play();
        }
    }
}
