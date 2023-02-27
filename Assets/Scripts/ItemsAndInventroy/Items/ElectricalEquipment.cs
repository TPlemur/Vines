using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElectricalEquipment : Item
{
    LayerMask layer;

    public ElectricalEquipment(Camera cam, LayerMask mask) : base(cam){
        layer = mask;
        LoadItem("ElectricalDevice");
    }

    public override void Primary(){
        GameObject obj = ShootRaycast(playerCam, 2.5f, layer);
        if(obj != null){
            if(obj.tag == "ElectricalPanel"){
                SparkMediumSFX();
                Debug.Log("PANEL CHECK");
                // call TurnOnLights() inside of warehouse
                // PlayerItemsAndInventory.generatorOn = true;
                // warehouseObj.gameObject.GetComponent<WarehouseMaker>().warehouse.TurnOnLights();
                //Destroy(panel.gameObject); -- don't destroy just disabled collider because the object holds some audio components
                obj.GetComponent<Collider>().enabled = false;
                TurnOnGeneratorSFX(obj);
                // ObjectiveScript.PowerObjBool = false;
                // ObjectiveScript.PVTMObjBool = true;
            }
            if(obj.tag == "ContainmentButton"){
                Debug.Log("MONSTER CHECK");
                if(MonsterCheck.isMonsterInside){
                    SceneManager.LoadScene(2);
                }
            }
        }
        else{
            SparkShortSFX();
        }
    }

    public override void Secondary(){
        return;
    }

    // SFX
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

    private void TurnOnGeneratorSFX(GameObject panel)
    {
        foreach (var emitter in panel.GetComponents<FMODUnity.StudioEventEmitter>())
        {
            emitter.Play();
        }
    }
}
