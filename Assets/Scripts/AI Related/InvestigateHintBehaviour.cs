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
    public static GameObject Lure;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Investigating");
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        playerPos = Player.position;//getClosestNavPointToPlayer(Player);
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
        mobBrain.currentMonState = Brain.monState.investigate;
        Mob.speed = 3;
        animator.SetBool("isChasing", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isPatrolling", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Walks towards player if they're not in a hiding spot
        if(Brain.currentTarget == Brain.target.player){
            if(!Mob.hasPath &! Brain.isHiding){
                Mob.SetDestination(Player.position);
            }
        }else if(Brain.currentTarget == Brain.target.lure){
            if(!Mob.hasPath){
                Mob.SetDestination(Lure.transform.position);
            }
        }
        // Enters charge state when player detected
        if(Brain.detectsPlayer){
            animator.SetBool("isCharging", true);
        }
        // Returns to patrol state when player hides
        if(Brain.isHiding){
            Brain.investigating = false;
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
       Brain.investigating = false;
       mobBrain.timeForHint = false;
    }
}
