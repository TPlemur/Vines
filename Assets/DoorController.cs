using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : StateMachineBehaviour
{
    public static bool doorLinkTrigger;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Heehoo");
        animator.SetBool("SecurityDoorToggle", false);
        if(GameStateManager.GeneratorOn){
            animator.SetBool("isLocked", false);
        }else{
            animator.SetBool("isLocked", true);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(GameStateManager.GeneratorOn){
            animator.SetBool("isLocked", false);
        }
        if(doorLinkTrigger){
            Debug.Log("TriggerSet");
            animator.SetBool("SecurityDoorToggle", true);
            doorLinkTrigger = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
