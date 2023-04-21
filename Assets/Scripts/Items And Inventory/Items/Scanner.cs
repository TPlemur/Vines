using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Scanner : Item
{
    LayerMask layer;
    float timer = 0;
    float boopDelay = 2;
    int progress = -1;
    GameObject progressBar;
    //Animator anim;

    List<GameObject> trackerTargets;

    public Coroutine sfxUpdateCoroutine = null;
    private FMOD.Studio.EventInstance continuousSFXInstance;
    static public GameObject sfxUpdateTarget = null; // bad code to make this static, but trying to hack together a solution that works with lots of other bad code...

    //LEGACY DO NOT USE
    public Scanner(GameObject stateManager, GameObject UIElement) : base(stateManager, UIElement)
    {
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
    }

    public void setup(Camera pCam, LayerMask mask, GameObject stateManager, GameObject UIElement, GameObject progressUI)
    {
        playerCam = pCam;
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        layer = mask;
        progressBar = progressUI;
        LoadItem("ElectricalDevice");
    }

    public override void Primary(){

    }

    


}
