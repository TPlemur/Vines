using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlockedBehaviour : StateMachineBehaviour
{
    float timer = 0;
    NavMeshAgent Mob;
    Brain mobBrain;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
        // Stops the monster from trying to pursue player through shield
        Mob.Stop();
        Mob.isStopped = true;
        Mob.speed = 0;
        Mob.ResetPath();
        Mob.GetComponentInChildren<MonVineStateMachine>().currentState = MonVineStateMachine.state.walk;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(timer < mobBrain.shieldDelayTime){
            timer += Time.deltaTime;
        }else{
            mobBrain.isShielded = false;
            animator.SetBool("isBlocked", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mob.GetComponentInChildren<MonsterSounds>().Howl();
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
