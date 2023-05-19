using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBehaviour : StateMachineBehaviour
{
    UnityEngine.AI.NavMeshAgent Mob;
    Brain mobBrain;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mob = animator.gameObject.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //One time patrol start event as a result of the generator turning on
        if(GameStateManager.GeneratorOn && !mobBrain.huntedGeneratorEvent){ 
            animator.SetBool("isSleeping", false);
            mobBrain.huntedGeneratorEvent = true;
            SmartSeeker.setSurround = true;
        }
    }
}
