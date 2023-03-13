using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElectricalEquipment : Item
{
    LayerMask layer;
    float timer = 0;
    float boopDelay = 2;
    //Animator anim;

    public ElectricalEquipment(Camera cam, LayerMask mask, GameObject stateManager, GameObject UIElement) : base(cam, stateManager, UIElement){
        layer = mask;
        LoadItem("ElectricalDevice");
        //anim = itemObj.transform.GetChild(0).GetComponent<Animator>();
        //PickupSFX(); you start with the item
    }

    public override void Primary(){
        GameObject obj = ShootRaycast(playerCam, 2.5f, layer);
        //anim.SetTrigger("EEAnimPlay");
        if(obj != null){
            if(!GameStateManager.GeneratorOn && obj.tag == "ElectricalPanel"){
                obj.transform.parent.GetComponent<GeneratorOn>().Starting = true;
                if (obj.transform.parent.GetComponent<GeneratorOn>().Started == true)
                {
                    SocketTesterSFX();
                    gameState.PowerRestored();
                    //Destroy(panel.gameObject); -- don't destroy just disabled collider because the object holds some audio components
                    obj.transform.parent.GetComponent<GeneratorVibe>().enabled = true;
                    obj.GetComponent<Collider>().enabled = false;
                    TurnOnGeneratorSFX(obj);
                }
            }
            if(obj.tag == "ContainmentButton" && gameState.IsPowerRestored()){
                if(MonsterCheck.isMonsterInside){
                    gameState.SplitjawContained();
                    SceneManager.LoadScene(2);
                }
            }
        }
        else{
            //limit boop to every n Seconds
            timer += Time.deltaTime;
            if (timer > boopDelay)
            {
                SocketTesterSFX();
                timer = 0;
            }

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
