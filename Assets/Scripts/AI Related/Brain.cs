using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Brain : MonoBehaviour
{
    // Hint system for investigating player locations
    public float mobHintTimer;
    [HideInInspector]
    public float hintTimer = 0;
    [HideInInspector]
    public bool timeForHint = false;
    [HideInInspector]
    public bool investigating = false;

    // System that determines when the monster hunts and for how long
    public float mobPursuitTimer;
    [HideInInspector]
    public float tempPursuitTimer;
    public float maxChaseTimer;
    [HideInInspector]
    public bool detectsPlayer = false;
    [HideInInspector]
    public bool huntedGeneratorEvent = false;
    
    public float patrolDelay;

    // System for monster to ambush from hiding spots
    public float ambushTime;
    public float maxAmbushTime = 10;
    [HideInInspector]
    public float timeHidden = 0;
    [HideInInspector]
    public float timeSpentAmbushing = 0;
    [HideInInspector]
    public bool isHiding = false;
    [HideInInspector]
    public bool monsterIsHiding = false;
    [HideInInspector]
    public bool timeForAmbush = false;
    [HideInInspector]
    public bool justAmbushed = false;
    [HideInInspector]
    public Vector3 oldPosition = new Vector3 (0,0,0);


    // System for player deploying shield
    public float shieldDelayTime = 6;
    public int shieldBumpDist = 1;
    [HideInInspector]
    public Vector3 shieldDir = new Vector3 (0,0,0);
    [HideInInspector]
    public bool isShielded = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug statements for testing timers
        // Debug.Log("Time Hidden: " + timeHidden);
        // Debug.Log("Detected: "+ detectsPlayer);
        // Debug.Log("Time spent ambushing" + timeSpentAmbushing);
        // Debug.Log("Hint Timer: " + hintTimer);


        // Timer to send hint
        if(hintTimer < mobHintTimer && !detectsPlayer){
            hintTimer += Time.deltaTime;
        }else if(detectsPlayer){
            hintTimer = 0;
        }
        else if(!isHiding){
            timeForHint = true;
            investigating = true;
        }

        // Activates ambush state after player hides for [ambushTime] seconds
        if(timeHidden >= ambushTime){
            timeForAmbush = true;
        }

        // Ticks timer for ambush state, 
        if(timeForAmbush && monsterIsHiding && timeSpentAmbushing < maxAmbushTime){
        // if(timeForAmbush && timeSpentAmbushing < maxAmbushTime){
            timeSpentAmbushing += Time.deltaTime;
            Debug.Log("Time spent ambushing" + timeSpentAmbushing);
        }else if(timeForAmbush && timeSpentAmbushing > maxAmbushTime){
            timeHidden = 0;
            timeSpentAmbushing = 0;
            timeForAmbush = false;
        }

        if(justAmbushed){
            justAmbushed = false;
            timeHidden = 0;
            timeSpentAmbushing = 0;
            timeForAmbush = false;
        }
    }
}
