using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PVTM : Item
{
    // Right click positioning
    private Vector3 original;
    private bool toggled = false;

    // Camera cycling
    private List<GameObject> activeCams;
    private GameObject currentCam;
    private int current = -1;

    // Camera connecting
    private LayerMask camLayer;
    private GameObject real;

    // Picture taking
    private LayerMask monsterPicLayer;
    private Material flash;
    private bool canTakePic = false;

    // Laser VFX
    private LineRenderer lr;
    private Transform laserStart;

    // Timers for vines covering cameras
    public float camVineTime = 90;

    private void Start()
    {
        activeCams = new List<GameObject>();
    }

    IEnumerator cameraDeactivationTimer(int index)
    {
        GameObject camToRemove = activeCams[index];
        yield return new WaitForSeconds(camVineTime-10);
        camToRemove.GetComponentInChildren<ProceduralIvy>().GenIvy();
        yield return new WaitForSeconds(10);
        camToRemove.GetComponent<Collider>().enabled = true;
        // If there is more than one camera linked, cycles cameras after removing one
        if(activeCams.Count > 1){
            if(current > index){
                current--;
            }
        }else{ 
            // If only camera in the list, screen goes black
            real.transform.SetParent(null);
            real.transform.localPosition = new Vector3(0, 1000, 0);
            canTakePic = false;
        }
        activeCams.RemoveAt(0);
        // Reloads camera
        if(activeCams.Count > 0){
            ConnectCurrent();
        }
    }

    public void setup(Camera cam, LayerMask camMask, GameObject realPVTMCam, LayerMask monsterMask,
            Material flashMat, GameObject stateManager, GameObject UIElement)
    {
        playerCam = cam;
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        LoadItem("PVTM_Prefab");
        camLayer = camMask;
        monsterPicLayer = monsterMask;
        real = realPVTMCam;
        flash = flashMat;
        ItemUI.transform.GetChild(0).gameObject.SetActive(true);
        laserStart = itemObj.transform.GetChild(0).GetChild(4);
        lr = laserStart.GetComponent<LineRenderer>();
        gameState.PVTMObtained();
        PickupSFX();

    }

    public override void Primary(){
        if(gameState.IsPowerRestored()){
            if(toggled){
                if(gameState.IsFirstCameraLinked()){
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
            }
            else{
                ShootLaser();
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
                    current = activeCams.Count - 1;
                    //currentCam = activeCams[activeCams.Count - 1];
                    ConnectCurrent();
                    //real.transform.SetParent(obj.transform);
                    //real.transform.localPosition = obj.transform.GetChild(0).gameObject.transform.localPosition;
                    //real.transform.localRotation = obj.transform.GetChild(0).gameObject.transform.localRotation;
                    //real.GetComponent<Camera>().fieldOfView = currentCam.GetComponent<Camera>().fieldOfView;
                    CameraWhirUpSFX(obj);
                    StartCoroutine(cameraDeactivationTimer(activeCams.Count - 1));
                }
            }
        }
        else{
            ShootLaser();
        }
    }

    public override void Secondary(){
        toggled = !toggled ? true : false;
        // figure out which UI element is active and inactive
        int active = toggled ? 0 : 1;
        int inactive = toggled ? 1 : 0;
        // set active UI to be inactive and inactive to be active
        ItemUI.transform.GetChild(active).gameObject.SetActive(false);
        ItemUI.transform.GetChild(inactive).gameObject.SetActive(true);
        itemObj.transform.localPosition = toggled ? new Vector3((float) 0.89, (float) 0.23, (float) -0.215) : original;
        // if bringing the camera up to face and at least one camera is linked, make the camera change sfx
        if (toggled && gameState.IsFirstCameraLinked()){
            CameraChangeSFX();
        }
    }

    // Cycle cams left and rigth
    public override void CycleRight(){
        if(activeCams.Count > 1){
            current += current == activeCams.Count - 1 ? -1 * (activeCams.Count - 1) : 1;
            //currentCam = activeCams[current];
            ConnectCurrent();
            //real.transform.SetParent(currentCam.transform);
            //real.transform.localPosition = currentCam.transform.GetChild(0).gameObject.transform.localPosition;
            //real.transform.localRotation = currentCam.transform.GetChild(0).gameObject.transform.localRotation;
            //real.GetComponent<Camera>().fieldOfView = currentCam.GetComponent<Camera>().fieldOfView;
            CameraChangeSFX();
        }
    }

    

    public override void CycleLeft(){
        if(activeCams.Count > 1){
            current -= current == 0 ? -1 * (activeCams.Count - 1) : 1;
            //currentCam = activeCams[current];
            ConnectCurrent();
            //real.transform.SetParent(currentCam.transform);
            //real.transform.localPosition = currentCam.transform.GetChild(0).gameObject.transform.localPosition;
            //real.transform.localRotation = currentCam.transform.GetChild(0).gameObject.transform.localRotation;
            //real.GetComponent<Camera>().fieldOfView = currentCam.GetComponent<Camera>().fieldOfView;
            CameraChangeSFX();
        }
    }

    public void ConnectCurrent(){
        currentCam = activeCams[current];
        real.transform.SetParent(currentCam.transform);
        real.transform.localPosition = currentCam.transform.GetChild(0).gameObject.transform.localPosition;
        real.transform.localRotation = currentCam.transform.GetChild(0).gameObject.transform.localRotation;
        real.GetComponentInChildren<Camera>().fieldOfView = currentCam.GetComponentInChildren<Camera>().fieldOfView;
        CameraChangeSFX();
    }


    public override bool IsToggled(){
        return toggled;
    }

    public void ShootLaser(){
        Debug.Log("laser shot");
        // shoot raycast to get distance so the laser doesn't clip through walls
        RaycastHit hit;
        Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit);
        // if hit distance is greater then 10 then set to 10
        // if player shoots into the sky make the distance 10
        float dist = hit.distance > 10f ? 10f : hit.distance == 0 ? 10f : hit.distance;
        // set new points of laser
        List<Vector3> points = new List<Vector3>(){laserStart.position, playerCam.transform.position + playerCam.transform.TransformDirection(Vector3.forward) * dist};
        lr.positionCount = points.Count;
        for(int i = 0; i < points.Count; i++){
            lr.SetPosition(i, points[i]);
        }
        gameState.GetComponent<GameStateManager>().StartCoroutine(LaserFade());
    }

    public IEnumerator LaserFade(){
        Color c = lr.startColor;
        float d = 0.03f;
        float remaining = d;
        while (remaining >= 0.0f) {
            c.a = remaining / d;
            lr.startColor = c;
            lr.endColor   = c;
            remaining -= Time.deltaTime;
            yield return null;
        }
        c.a = 0;
        lr.startColor = c;
        lr.endColor   = c;
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

    public static bool resupdate = false;
    private void Update()
    {
        if (resupdate)
        {
            resupdate = false;
            if (activeCams.Count >= 1)
            {
                ConnectCurrent();
            }
        }
    }
}
