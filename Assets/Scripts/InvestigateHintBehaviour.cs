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

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        playerPos = Player.position;//getClosestNavPointToPlayer(Player);
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        //Debug.Log("Investigate state");
        Mob.speed = 3;
        animator.SetBool("isChasing", false);
        animator.SetBool("isPatrolling", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!Mob.hasPath){
            Mob.SetDestination(Player.position);
        }
        // Mob.GetComponent<Brain>().investigating = true;
        if(Mob.GetComponentInChildren<Brain>().detectsPlayer){
            Debug.Log(Mob.GetComponentInChildren<Brain>().detectsPlayer);
            animator.SetBool("isChasing", true);
        }
        if(Mob.GetComponentInChildren<Brain>().isHiding){
            animator.SetBool("isPatrolling", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       Mob.GetComponentInChildren<Brain>().investigating = false;
    }

    public Vector3 getClosestNavPointToPlayer(Transform target){
        NavMeshHit navHit;
        NavMesh.FindClosestEdge(target.position, out navHit, NavMesh.AllAreas);
        Debug.Log("finding closest position");
        return navHit.position;
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
