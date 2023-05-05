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
        Mob.speed = 3;
        animator.SetBool("isChasing", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isPatrolling", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Walks towards player if they're not in a hiding spot
        if(!Mob.hasPath &! mobBrain.isHiding){
            Mob.SetDestination(Player.position);
        }
        // Enters charge state when player detected
        if(mobBrain.detectsPlayer){
            animator.SetBool("isCharging", true);
        }
        // Returns to patrol state when player hides
        if(mobBrain.isHiding){
            mobBrain.investigating = false;
            animator.SetBool("isInvestigating", false);
        }
        // Enters charge state if the player takes a picture of the monster
        if(!mobBrain.picTakenEvent && GameStateManager.ValidSplitjawPic){
            Mob.velocity = Vector3.zero;
            animator.SetBool("isCharging", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       mobBrain.investigating = false;
    }
}
