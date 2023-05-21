using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmbushBehaviour : StateMachineBehaviour
{
    Transform Player;
    UnityEngine.AI.NavMeshAgent Mob;
    Brain mobBrain;
    GameObject[] hidingHoles;
    GameObject hidingSpotToAmbush;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entered Ambush State");
        hidingHoles = GameObject.FindGameObjectsWithTag("HidingHole");
        Mob = animator.gameObject.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
        mobBrain.currentMonState = Brain.monState.ambush;
        mobBrain.oldPosition = Mob.transform.position;
        mobBrain.monsterIsHiding = true;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Brain.investigating = false;
        animator.gameObject.GetComponent<MonVineStateMachine>().currentState = MonVineStateMachine.state.walk;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hidingSpotToAmbush = hidingHoles[0];
        foreach(GameObject i in hidingHoles){   // For every hiding hole
            // Checks if the next hiding hole is closer than the next closest, and if so, sets it as the ambush target
            float distanceOld = Vector3.Distance(hidingSpotToAmbush.transform.position, Player.position);
            float distanceNew = Vector3.Distance(i.transform.position, Player.position);
            if(distanceNew < distanceOld){
                hidingSpotToAmbush = i;
            }
        }
        // Moves monster to hiding spot
        float distanceFromPlayer = Vector3.Distance(hidingSpotToAmbush.transform.position, Player.position);
        if(!Brain.isHiding && distanceFromPlayer >= 10){
            Mob.Warp(hidingSpotToAmbush.transform.position);
            Mob.isStopped = true;
            Mob.Stop();
            Mob.ResetPath();
            Mob.transform.LookAt(Player);
            hidingSpotToAmbush.transform.parent.parent.GetComponentInChildren<HideVineMoveEffects>().growVine();
        }
        // If the player gets too close, charge them
        float distanceFromHidingSpot = Vector3.Distance(hidingSpotToAmbush.transform.position, Mob.transform.position);
        if(distanceFromPlayer <= 7 && distanceFromHidingSpot <= 2){ // Ensures monster is in hiding spot before charging
            animator.SetBool("isCharging", true);
            mobBrain.justAmbushed = true;
            animator.SetBool("isAmbushing", false);
        }
        // Once timer is up, leaves ambush state and returns to patrolling state
        if(!mobBrain.timeForAmbush){
            mobBrain.justAmbushed = true;
            animator.SetBool("isAmbushing", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       mobBrain.monsterIsHiding = false;
    }
}
