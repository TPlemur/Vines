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
    public float maxAmbushTime;
    [HideInInspector]
    public float timeHidden = 0;
    [HideInInspector]
    public bool isHiding = false;
    [HideInInspector]
    public bool timeForAmbush = false;
    [HideInInspector]
    public float timeSpentAmbushing = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Time Hidden: " + timeHidden);
        // Debug.Log("Detected: "+ detectsPlayer);
        

        // Timer to send hint
        if(hintTimer < mobHintTimer && !detectsPlayer){
            hintTimer += Time.deltaTime;
            //Debug.Log(hintTimer);
        }else if(detectsPlayer){
            hintTimer = 0;
        }
        else if(!isHiding){
            timeForHint = true;
            Debug.Log("HINT");
            investigating = true;
        }

        // Activates ambush state after player hides for [ambushTime] seconds
        if(timeHidden >= ambushTime){
            timeForAmbush = true;
        }

        // Ticks timer for ambush state, 
        if(timeForAmbush && timeSpentAmbushing < maxAmbushTime){
            timeSpentAmbushing += Time.deltaTime;
            // Debug.Log("Time spent ambushing" + timeSpentAmbushing);
        }else if(timeForAmbush && timeSpentAmbushing > maxAmbushTime){
            timeHidden = 0;
            timeSpentAmbushing = 0;
            timeForAmbush = false;
        }
    }
}
