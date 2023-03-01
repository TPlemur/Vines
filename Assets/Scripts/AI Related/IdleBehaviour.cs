using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    float timer;
    float patrolDelay;
    public float MobDetectionDistance = 1000.0f;
    Transform Player;
    NavMeshAgent Mob;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        patrolDelay = Mob.GetComponentInChildren<Brain>().patrolDelay;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > patrolDelay)
            animator.SetBool("isPatrolling", true);

        // float distance = Vector3.Distance(animator.transform.position, Player.position);
        // if (distance < MobDetectionDistance)
        //     animator.SetBool("isChasing", true);
        //Debug.Log(Mob.GetComponent<Brain>().detectsPlayer);
        if(Mob.GetComponentInChildren<Brain>().detectsPlayer && !Mob.GetComponentInChildren<Brain>().isHiding)
            animator.SetBool("isChasing", true);

        //Debug.Log(Mob.GetComponent<Brain>().investigating);
        if (Mob.GetComponentInChildren<Brain>().investigating){
            animator.SetBool("isInvestigating", true);
            //Debug.Log("Investigating");
        }
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