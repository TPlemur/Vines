using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElectricalEquipment : Item
{
    LayerMask layer;

    public ElectricalEquipment(Camera cam, LayerMask mask, GameObject stateManager) : base(cam, stateManager){
        layer = mask;
        LoadItem("ElectricalDevice");
    }

    public override void Primary(){
        GameObject obj = ShootRaycast(playerCam, 2.5f, layer);
        if(obj != null){
            if(obj.tag == "ElectricalPanel"){
                SparkMediumSFX();
                gameState.PowerRestored();
                //Destroy(panel.gameObject); -- don't destroy just disabled collider because the object holds some audio components
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
            SparkShortSFX();
        }
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
