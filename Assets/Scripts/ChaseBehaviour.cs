using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    Transform Player;
    NavMeshAgent Mob;
    public float attackRange = 20;
    public float enemyspeed = 5;
    float timer;
    bool chargeCD = false;
    bool charging = false;
    Vector3 playerPos;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       timer = 0;
       Mob = animator.GetComponent<NavMeshAgent>();
       Mob.speed = enemyspeed;

       Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distance = Vector3.Distance(animator.transform.position, Player.position);
        //Debug.Log(playerPos);
        if (distance < attackRange && !chargeCD && !charging){
            //Debug.Log("charging");
            charging = true;
            playerPos = Player.position;
            Mob.SetDestination(playerPos);
            Mob.speed = 20;
        }else if (charging){
            if (Vector3.Distance(animator.transform.position, playerPos) < 1){
                Mob.speed = 5;
                charging = false;
                chargeCD = true;
                Debug.Log("finished charge");
            }
        }else{
            Mob.SetDestination(Player.position);
        }
        if (chargeCD){
            timer += Time.deltaTime;
        }
        if (timer > 3)
            Debug.Log("charge again");
            chargeCD = false;
            timer = 0;

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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