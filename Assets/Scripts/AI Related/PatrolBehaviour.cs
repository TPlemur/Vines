using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PatrolBehaviour : StateMachineBehaviour
{
    float timer;
    public float MobDetectionDistance = 1000.0f;
    public float patrolRadius = 100.0f;
    Transform Player;
    GameObject PlayerObj;
    NavMeshAgent Mob;
    Brain mobBrain;
    bool playerCanSeeMob = false;

    private MonsterSounds sounds = null;
    public MonsterMusic music;
    public PlayerSounds playerSounds;

    Vector3 patrolPos;

    public float visionAngle = 1f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        /*if (sounds == null)
            sounds = GetComponent<MonsterSounds>();
        */
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        animator.gameObject.GetComponent<MonVineStateMachine>().currentState = MonVineStateMachine.state.walk;
        mobBrain.investigating = false;
        // Stops the monster from trying to pursue player into hiding holes
        Mob.Stop();
        Mob.ResetPath();

        // set footstep intensity
        Mob.GetComponentInChildren<SplitjawFootstepController>().SetIntensity(0.4f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // If the monster doesn't have a path, finds random spot within a radius to patrol to
       if(!Mob.hasPath){
            if (mobBrain.investigating){
                animator.SetBool("isInvestigating", true);
            }else if (GameStateManager.GeneratorOn && !mobBrain.huntedGeneratorEvent){ //One time hunt event as a result of the generator turning on
                animator.SetBool("isInvestigating", true);
                mobBrain.huntedGeneratorEvent = true;
                mobBrain.investigating = true;
            }else{
                patrolPos = RandomNavmeshLocation(patrolRadius);
                Mob.SetDestination(patrolPos);
                Mob.speed = 3; //Only for testing purposes
                //Debug.Log("RANDOM");
            }
       }

       // Supposed to end patrolling after reaching spot, so it can return to idle, then patrol again
    //    if(Mob.velocity.sqrMagnitude == 0f){
    //         Debug.Log("reached patrol spot");
    //         animator.SetBool("isPatrolling", false);
    //    }

       /*Mob.SetDestination(RandomNavmeshLocation(patrolRadius));
            if (music)
                music.EndChase();
            if (playerSounds)
                playerSounds.EndChase();
        */
        // float distance = Vector3.Distance(animator.transform.position, Player.position);
        // if (distance < MobDetectionDistance)
        if(mobBrain.detectsPlayer && !mobBrain.isHiding){
            Mob.velocity = Vector3.zero;
            animator.SetBool("isCharging", true);
        }

        if(mobBrain.timeForAmbush){
            Ray monVis = new Ray(Mob.transform.position, (Player.transform.position-Mob.transform.position));
            RaycastHit hit;
            if(Physics.Raycast(monVis, out hit, 100)){
                if(hit.collider.tag == "Player"){
                    playerCanSeeMob = true;
                }else{
                    playerCanSeeMob = false;
                }
            }
            if(!playerCanSeeMob){
                animator.SetBool("isAmbushing", true);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    public Vector3 RandomNavmeshLocation(float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += Player.transform.position;
        NavMeshHit NavMeshEnemy;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshEnemy, radius, 1)) {
            finalPosition = NavMeshEnemy.position;            
        }
        return finalPosition;
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
