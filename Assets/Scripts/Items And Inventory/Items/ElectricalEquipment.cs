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

    public Coroutine sfxUpdateCoroutine = null;
    private FMOD.Studio.EventInstance continuousSFXInstance;
    static public GameObject sfxUpdateTarget = null; // bad code to make this static, but trying to hack together a solution that works with lots of other bad code...

    public ElectricalEquipment(Camera cam, LayerMask mask, GameObject stateManager, GameObject UIElement) : base(cam, stateManager, UIElement){
        layer = mask;
        LoadItem("ElectricalDevice");
        InitContinuousSFX(cam.gameObject);
        //anim = itemObj.transform.GetChild(0).GetComponent<Animator>();
        //PickupSFX(); you start with the item
    }

    public override void Primary(){
        if (sfxUpdateCoroutine == null)
            sfxUpdateCoroutine = gameState.GetComponent<GameStateManager>().StartCoroutine(SFXUpdate());

        GameObject obj = ShootRaycast(playerCam, 2.5f, layer);
        //anim.SetTrigger("EEAnimPlay");
        if(obj != null){
            if(!GameStateManager.GeneratorOn && obj.tag == "ElectricalPanel"){
                sfxUpdateTarget = obj.transform.parent.gameObject;
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
            //timer += Time.deltaTime;
            //if (timer > boopDelay)
            //{
            //    SocketTesterSFX();
            //    timer = 0;
            //}

        }
    }

    public IEnumerator SFXUpdate()
    {
        StartContinuousSFX();

        SocketTesterSFX();

        // keep updating while held down
        while (Input.GetMouseButton(0))
        {
            if (sfxUpdateTarget != null)
            {
                GeneratorOn genTarget = sfxUpdateTarget.GetComponent<GeneratorOn>();
                TrapSetter trapTarget = sfxUpdateTarget.GetComponent<TrapSetter>();
                if (genTarget != null)
                {
                    float pitch = genTarget.GetTimerRatio() * 0.5f;
                    SetContinuousSFXPitch(pitch);

                    if (genTarget.Started)
                    {
                        sfxUpdateTarget = null;
                        StopContinuousSFX();
                        SetContinuousSFXPitch(0);
                    }
                }
                if (trapTarget != null)
                {
                    float pitch = trapTarget.GetTimerRatio() * 0.5f;
                    SetContinuousSFXPitch(pitch);

                    if (trapTarget.currentState != TrapSetter.State.off)
                    {
                        sfxUpdateTarget = null;
                        StopContinuousSFX();
                        SetContinuousSFXPitch(0);
                        SocketTesterSFX();
                    }
                }
            }
            yield return null;
        }

        yield return null;

        // not holding down anymore
        gameState.GetComponent<GameStateManager>().StopCoroutine(sfxUpdateCoroutine);
        StopContinuousSFX();
        sfxUpdateCoroutine = null;
    }

    // SFX
    private void InitContinuousSFX(GameObject obj)
    {
        const string eventName = "event:/SFX/Items/Electrical/Socket Tester Continuous";
        continuousSFXInstance = FMODUnity.RuntimeManager.CreateInstance(eventName);
        continuousSFXInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(obj));
    }
    private void StartContinuousSFX()
    {
        continuousSFXInstance.start();
    }
    private void StopContinuousSFX()
    {
        continuousSFXInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    private void SetContinuousSFXPitch(float pitch)
    {
        continuousSFXInstance.setParameterByName("Pitch", pitch);
    }
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
