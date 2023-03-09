using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVTM : Item
{
    // Right click positioning
    private Vector3 original;
    private bool toggled = false;

    // Camera cycling
    private List<GameObject> activeCams = new List<GameObject>();
    private GameObject currentCam;
    private int current = -1;

    // Camera connecting
    private LayerMask camLayer;
    private GameObject real;

    // Picture taking
    private LayerMask monsterPicLayer;
    private Material flash;
    private bool canTakePic = false;

    public PVTM(Camera cam, LayerMask camMask, GameObject realPVTMCam, LayerMask monsterMask, Material flashMat, GameObject stateManager) : base(cam, stateManager){
        LoadItem("PVTM_Prefab");
        camLayer = camMask;
        monsterPicLayer = monsterMask;
        real = realPVTMCam;
        flash = flashMat;
        gameState.PVTMObtained();
        //flash.a = 0.0f;

        PickupSFX();
    }

    public override void Primary(){
        if(gameState.IsPowerRestored()){
            if(toggled && gameState.IsFirstCameraLinked()){
                GameObject obj = ShootRaycast(real, 25.0f, monsterPicLayer);
                if(obj != null){
                    // take pic
                    gameState.SplitjawDocumented();
                }
                if(canTakePic){
                    gameState.GetComponent<GameStateManager>().StartCoroutine(Flash());
                    PicSFX();
                }
            }
            else{
                GameObject obj = ShootRaycast(playerCam, 10.0f, camLayer);
                if(obj != null){
                    // update game state to know what cams are linked
                    if(activeCams.Count == 0){
                        canTakePic = true;
                        gameState.FirstCameraLinked();
                    }
                    else{
                        gameState.AnotherCameraLinked();
                    }
                    // move real PVTM camera to new location and add the gameobject
                    // of the new location to the list activeCams
                    obj.GetComponent<Collider>().enabled = false;
                    activeCams.Add(obj);
                    currentCam = activeCams[activeCams.Count - 1];
                    real.transform.SetParent(obj.transform);
                    real.transform.localPosition = obj.transform.GetChild(0).gameObject.transform.localPosition;
                    real.transform.localRotation = obj.transform.GetChild(0).gameObject.transform.localRotation;
                    CameraWhirUpSFX(obj);
                }
            }
        }   
    }

    public override void Secondary(){
        toggled = !toggled ? true : false;
        itemObj.transform.localPosition = toggled ? new Vector3((float) 0.89, (float) 0.23, (float) -0.215) : original;
        // if bringing the camera up to face and at least one camera is linked, make the camera change sfx
        if (toggled && gameState.IsFirstCameraLinked())
            CameraChangeSFX();
    }

    // Cycle cams left and rigth
    public override void CycleRight(){
        if(activeCams.Count > 1){
            current += current == activeCams.Count - 1 ? -1 * (activeCams.Count - 1) : 1;
            currentCam = activeCams[current];
            real.transform.SetParent(currentCam.transform);
            real.transform.localPosition = currentCam.transform.GetChild(0).gameObject.transform.localPosition;
            real.transform.localRotation = currentCam.transform.GetChild(0).gameObject.transform.localRotation;
            CameraChangeSFX();
        }
    }

    public override void CycleLeft(){
        if(activeCams.Count > 1){
            current -= current == 0 ? -1 * (activeCams.Count - 1) : 1;
            currentCam = activeCams[current];
            real.transform.SetParent(currentCam.transform);
            real.transform.localPosition = currentCam.transform.GetChild(0).gameObject.transform.localPosition;
            real.transform.localRotation = currentCam.transform.GetChild(0).gameObject.transform.localRotation;
            CameraChangeSFX();
        }
    }

    public override bool IsToggled(){
        return toggled;
    }

    public IEnumerator Flash(){
        this.canTakePic = false;
        Color c = flash.color;
        float d = 1.25f;
        float remaining = d;
        while (remaining >= 0.0f) {
            c.a = remaining / d;
            flash.color = c;
            remaining -= Time.deltaTime;
            yield return null;
        }
        this.canTakePic = true;
        c.a = 0;
        flash.color = c;
    }

    // SFX
    private void PickupSFX()
    {
        const string eventName = "event:/SFX/Items/Inventory/Bag Pickup";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
    private void CameraWhirUpSFX(GameObject cam)
    {
        // camera whir up sound
        const string eventName = "event:/SFX/Items/Camera/WhirUp";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(cam));
        sound.start();
        sound.release();
    }
    private void PicSFX()
    {
        // pic sound
        const string eventName = "event:/SFX/Items/Camera/Shutter2D-2";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
    private void CameraChangeSFX()
    {
        // camera change noise sound
        const string eventName = "event:/SFX/Items/Camera/WhiteNoiseStatic-2";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.start();
        sound.release();
    }
}
