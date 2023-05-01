using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Scanner : Item
{
    LayerMask layer;
    bool scannerOn = false;
    float scanWidth = 15f;
    List<GameObject> trackerTargets;

    ScannerDisplay display;
    float scannerScale = 80;


    public Coroutine sfxUpdateCoroutine = null;
    private FMOD.Studio.EventInstance continuousSFXInstance;
    static public GameObject sfxUpdateTarget = null; // bad code to make this static, but trying to hack together a solution that works with lots of other bad code...

    void findItems()
    {
        trackerTargets.Add(GameObject.Find("Flashlight_I"));
        trackerTargets.Add(GameObject.Find("PVTM_Prefab_I"));
        trackerTargets.Add(GameObject.Find("Shield_I"));
        trackerTargets.Add(GameObject.Find("GenOffLight"));
        trackerTargets.Add(GameObject.Find("Monster"));
    }

    private void Start()
    {
        findItems();   
    }

    public void setup(Camera pCam, LayerMask mask, GameObject stateManager, GameObject UIElement, List<GameObject> targets)
    {
        playerCam = pCam;
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        layer = mask;
        LoadItem("Scanner");
        trackerTargets = targets;
        display = itemObj.GetComponent<ScannerDisplay>();
    }

    public void addTarget(GameObject newTar)
    {
        trackerTargets.Add(newTar);
    }
    public bool clearTarget(GameObject tar)
    {
        return trackerTargets.Remove(tar);
    }

    public override void Primary(){
        scannerOn = !scannerOn ? true : false;
        if (!scannerOn)
        {
            display.BlankDisplay();
        }
    }

    private void Update()
    {
        if (scannerOn)
        {
            GameObject tar = scan();
            if(tar == null) { display.BlankDisplay(); }
            else { objToDisplay(tar); }
        }
    }

    //
    void objToDisplay(GameObject obj)
    {

        //get a bunch of info
        Vector3 targetDir = obj.transform.position - playerCam.transform.position;
        float angleFront = Vector3.Angle(targetDir.normalized, playerCam.transform.forward);
        float angleRight = Vector3.Angle(targetDir.normalized, playerCam.transform.right);
        float targetDist = targetDir.magnitude;

        //find col
        int col;
        if (angleFront < (2 * scanWidth) / 3) { col = 1; }
        else if (angleRight < 90) { col = 2; }
        else { col = 0; }

        //find row
        float row = 1 + 4 * targetDist / scannerScale;
        row = Mathf.Clamp(row, 1, 4);

        display.SetDisplay(col, (int)row);
    }

    //returns the closest object within the angle threshold, or null if nothing found
    GameObject scan()
    {
        //Remove any destroyed objects
        trackerTargets.RemoveAll(t => t == null);

        List<float> dists = new List<float>();
        List<GameObject> objs = new List<GameObject>();
        foreach (GameObject target in trackerTargets)
        {
            Vector3 targetDir = target.transform.position - playerCam.transform.position;
            float angle = Vector3.Angle(targetDir.normalized, playerCam.transform.forward);
            if (angle < scanWidth) {
                dists.Add(Mathf.Abs(targetDir.magnitude));
                objs.Add(target);
            }
        }
        if (dists.Count == 0) { return null; }
        else
        {
            //find min in list
            GameObject finalTarget = objs[0];
            float detectedDist = dists[0];
            for (int i = 0;i < dists.Count ;i++)
            {
                if (dists[i] < detectedDist)
                {
                    detectedDist = dists[i];
                    finalTarget = objs[1];
                }
            }
            return finalTarget;
        }
    }

}
