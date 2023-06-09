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
    GameObject[] hidingHoles;
    public static GameObject Lure;
    bool tooCloseToPlayer = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Investigating");
        hidingHoles = GameObject.FindGameObjectsWithTag("HidingHole");
        Debug.Log(hidingHoles.Length);
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
                tooCloseToPlayer = false;
                foreach(GameObject i in hidingHoles){   // For every hiding hole
                    float distance = Vector3.Distance(i.transform.position, Player.position);
                    Debug.Log("Distance: " + distance);
                    if(distance <= 5){
                        tooCloseToPlayer = true;
                    }
                }
                if(!tooCloseToPlayer){
                    Mob.SetDestination(Player.position);
                }else{
                    Mob.SetDestination(RandomNavmeshLocation(25));
                }
                Debug.Log("Too Close?: " + tooCloseToPlayer);
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

    // Finds a random location on the navmesh within a radius around the player, then returns it
    public Vector3 RandomNavmeshLocation(float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += Player.transform.position;
        NavMeshHit NavMeshEnemy;
        Vector3 finalPosition = Vector3.zero;
        tooCloseToPlayer = false;
        foreach(GameObject i in hidingHoles){   // For every hiding hole
            float distance = Vector3.Distance(i.transform.position, Player.position);
            Debug.Log("Distance: " + distance);
            if(distance <= 5){
                tooCloseToPlayer = true;
            }
        }
        if (NavMesh.SamplePosition(randomDirection, out NavMeshEnemy, radius, 1) && !tooCloseToPlayer) {
            finalPosition = NavMeshEnemy.position;            
        }
        return finalPosition;
    }
}
