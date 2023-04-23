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
    float scanWidth = 0.25f;
    List<GameObject> trackerTargets;

    public Coroutine sfxUpdateCoroutine = null;
    private FMOD.Studio.EventInstance continuousSFXInstance;
    static public GameObject sfxUpdateTarget = null; // bad code to make this static, but trying to hack together a solution that works with lots of other bad code...


    public void setup(Camera pCam, LayerMask mask, GameObject stateManager, GameObject UIElement, List<GameObject> targets)
    {
        playerCam = pCam;
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        layer = mask;
        LoadItem("ElectricalDevice");
        trackerTargets = targets;
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
    }

    private void Update()
    {
        if (scannerOn)
        {
            Debug.Log(scan());
        }
    }

    //returns the closest object within the angle threshold, or zero if nothing found
    float scan()
    {
        List<float> dists = new List<float>();
        foreach (GameObject target in trackerTargets)
        {
            Vector3 targetDir = target.transform.position - playerCam.transform.position;
            float angle = Vector3.Angle(targetDir.normalized, playerCam.transform.forward);
            if (angle < scanWidth) { dists.Add(Mathf.Abs(targetDir.magnitude)); }
        }
        if (dists.Count == 0) { return 0; }
        else
        {
            //find min in list
            float detectedDist = dists[0];
            for (int i = 0;i < dists.Count ;i++)
            {
                if (dists[i] > detectedDist)
                {
                    detectedDist = dists[i];
                }
            }
            return detectedDist;
        }
    }

}
