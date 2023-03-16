using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class InvestigateHintBehaviour : StateMachineBehaviour
{
    public float MobDetectionDistance = 1000.0f;
    Transform Player;
    NavMeshAgent Mob;
    Vector3 playerPos;
    Brain mobBrain;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Investigating");
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        playerPos = Player.position;//getClosestNavPointToPlayer(Player);
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
        //Debug.Log("Investigate state");
        Mob.speed = 3;
        animator.SetBool("isChasing", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isPatrolling", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!Mob.hasPath &! mobBrain.isHiding){
            Mob.SetDestination(Player.position);
        }
        // Mob.GetComponent<Brain>().investigating = true;
        if(mobBrain.detectsPlayer){
            animator.SetBool("isCharging", true);
        }
        if(mobBrain.isHiding){
            mobBrain.investigating = false;
            //animator.SetBool("isPatrolling", true);
            animator.SetBool("isInvestigating", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       mobBrain.investigating = false;
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
