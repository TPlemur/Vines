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
    private bool isFlashing = false;
    private float flashTimer = 0;

    public PVTM(Camera cam, LayerMask camMask, GameObject realPVTMCam, LayerMask monsterMask, Material flashMat, GameObject stateManager) : base(cam, stateManager){
        LoadItem("PVTM_Prefab");
        camLayer = camMask;
        monsterPicLayer = monsterMask;
        real = realPVTMCam;
        flash = flashMat;
        gameState.PVTMObtained();
    }

    public override void Primary(){
        if(gameState.IsPowerRestored()){
            if(toggled){
                GameObject obj = ShootRaycast(real, 25.0f, monsterPicLayer);
                // StartCoroutine(Flash());
                if(obj != null){
                    // take pic
                    gameState.SplitjawDocumented();
                }
            }
            else{
                GameObject obj = ShootRaycast(playerCam, 10.0f, camLayer);
                if(obj != null){
                    // update game state to know what cams are linked
                    if(activeCams.Count == 0){
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

    IEnumerator Flash(){
        Color c = flash.color;
        for(float alpha = 1f; alpha >= 0f; alpha -= 0.1f){
            c.a = alpha;
            flash.color = c;
            yield return null;
        }
    }

    // SFX
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
