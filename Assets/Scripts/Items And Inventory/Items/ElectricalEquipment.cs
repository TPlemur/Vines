using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ElectricalEquipment : Item
{
    LayerMask layer;
    float timer = 0;
    float boopDelay = 2;
    int progress = -1;
    GameObject progressBar;
    //Animator anim;

    public Coroutine sfxUpdateCoroutine = null;
    private FMOD.Studio.EventInstance continuousSFXInstance;
    static public GameObject sfxUpdateTarget = null; // bad code to make this static, but trying to hack together a solution that works with lots of other bad code...

    public ElectricalEquipment(Camera cam, LayerMask mask, GameObject stateManager, GameObject UIElement, GameObject progressUI) : base(cam, stateManager, UIElement){
        layer = mask;
        progressBar = progressUI;
        LoadItem("ElectricalDevice");
        InitContinuousSFX(cam.gameObject);
        //anim = itemObj.transform.GetChild(0).GetComponent<Animator>();
        //PickupSFX(); you start with the item
    }

    public void setup(Camera pCam, LayerMask mask, GameObject stateManager, GameObject UIElement, GameObject progressUI)
    {
        playerCam = pCam;
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        layer = mask;
        progressBar = progressUI;
        LoadItem("ElectricalDevice");
        InitContinuousSFX(pCam.gameObject);

        //anim = itemObj.transform.GetChild(0).GetComponent<Animator>();
        //PickupSFX(); you start with the item
    }

    ~ElectricalEquipment()
    {
        StopContinuousSFX();
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

        }
    }

    public IEnumerator SFXUpdate()
    {
        StartContinuousSFX();

        SocketTesterSFX();

        float timer = 0f;

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

                        progressBar.transform.GetComponent<UnityEngine.UI.Slider>().value = 0;
                        progressBar.SetActive(false);
                    }
                    else{
                        progressBar.SetActive(true);
                        progressBar.transform.GetComponent<UnityEngine.UI.Slider>().value = genTarget.GetTimerRatio() + 0.01f;
                    }
                    if(genTarget.GetTimerRatio() < 0.001f){
                        if(timer > 0.001f){
                            progressBar.transform.GetComponent<UnityEngine.UI.Slider>().value = 0;
                            progressBar.SetActive(false);
                        }
                        timer += Time.deltaTime;
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
                        progressBar.transform.GetComponent<UnityEngine.UI.Slider>().value = 0;
                        progressBar.SetActive(false);
                    }
                    else{
                        progressBar.SetActive(true);
                        progressBar.transform.GetComponent<UnityEngine.UI.Slider>().value = trapTarget.GetTimerRatio() + 0.01f;
                    }
                    if(trapTarget.GetTimerRatio() < 0.001f){
                        if(timer > 0.001f){
                            progressBar.SetActive(false);
                            progressBar.transform.GetComponent<UnityEngine.UI.Slider>().value = 0;
                        }
                        timer += Time.deltaTime;
                    }
                }
            }
            yield return null;
        }

        // not holding down anymore
        StopContinuousSFX();
        sfxUpdateCoroutine = null;
        // reset target
        sfxUpdateTarget = null;
        // reset pitch
        SetContinuousSFXPitch(0);
        // turn off all children of the progress bar
        progressBar.SetActive(false);
        progressBar.transform.GetComponent<UnityEngine.UI.Slider>().value = 0;
    }

    // SFX
    private void InitContinuousSFX(GameObject obj)
    {
        const string eventName = "event:/SFX/Items/Electrical/Socket Tester Continuous";
        continuousSFXInstance = FMODUnity.RuntimeManager.CreateInstance(eventName);
        continuousSFXInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(obj));
        continuousSFXInstance.setVolume(0.8f);
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
