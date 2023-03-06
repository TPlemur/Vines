using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushBehaviour : StateMachineBehaviour
{
    Transform Player;
    UnityEngine.AI.NavMeshAgent Mob;
    Brain mobBrain;
    GameObject[] hidingHoles;// = GameObject.FindGameObjectsWithTag("HidingHole");
    GameObject hidingSpotToAmbush;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entered Ambush State");
        hidingHoles = GameObject.FindGameObjectsWithTag("HidingHole");
        Mob = animator.gameObject.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        mobBrain.investigating = false;
        // Stops the monster from trying to move and stops vines from spawning
        Mob.Stop();
        Mob.isStopped = true;
        animator.gameObject.GetComponent<MonVineStateMachine>().currentState = MonVineStateMachine.state.none;
        Mob.ResetPath();
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
        if(!mobBrain.isHiding){
            Mob.Warp(hidingSpotToAmbush.transform.position);
        }

        //If within certain distance of player, kill them
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
