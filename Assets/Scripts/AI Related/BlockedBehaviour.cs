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
        animator.SetBool("isChasing", false);
        animator.SetBool("isCharging", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stops the monster for [shieldDelayTime] seconds, until the shield deflates, then resumes charging
        if(timer < mobBrain.shieldDelayTime){
            timer += Time.deltaTime;
        }else{
            mobBrain.isShielded = false;
            animator.SetBool("isBlocked", false);
            Brain.detectsPlayer = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mob.GetComponentInChildren<MonsterSounds>().Howl();
        Brain.detectsPlayer = false;
    }
}
