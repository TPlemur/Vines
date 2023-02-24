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
    [HideInInspector]
    public bool detectsPlayer = false;
    
    public float patrolDelay;

    // System for monster to ambush from hiding spots
    public float ambushTime;
    [HideInInspector]
    public float timeHidden = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(detectsPlayer);
        // Timer to send hint
        if(hintTimer < mobHintTimer && !detectsPlayer){
            hintTimer += Time.deltaTime;
            //Debug.Log(hintTimer);
        }else if(detectsPlayer){
            hintTimer = 0;
        }
        else{
            timeForHint = true;
            investigating = true;
        }
    }
}
