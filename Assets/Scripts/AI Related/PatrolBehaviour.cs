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
    public float MobAmbushDistance = 25.0f;
    Transform Player;
    GameObject PlayerObj;
    NavMeshAgent Mob;
    Brain mobBrain;

    Vector3 patrolPos;
    public float visionAngle = 1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        animator.gameObject.GetComponent<MonVineStateMachine>().currentState = MonVineStateMachine.state.walk;
        mobBrain.investigating = false;
        // Stops the monster from trying to pursue player into hiding holes
        Mob.Stop();
        Mob.ResetPath();

        // AUDIO
        // set FMOD ChaseState to patrolling
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ChaseState", (float)MixerController.CHASE_STATE.PATROLLING);
        // set footstep intensity
        Mob.GetComponentInChildren<SplitjawFootstepController>().SetIntensity(0.4f);

        //make vines chase player
        GameObject.FindGameObjectWithTag("Vine").GetComponent<SeekerTrigger>().setOnMonster();
    }



    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // If the monster doesn't have a path and isn't supposed to change to a new state, finds random spot within a radius to patrol to
       // otherwise, it changes states and starts a new behavior
       if(!Mob.hasPath){
            if (mobBrain.investigating){
                animator.SetBool("isInvestigating", true);
            }else if(mobBrain.timeForAmbush){
                checkAmbushValidity(animator, Mob, mobBrain, patrolRadius, patrolPos);
            }else{  // Patrol to random location on navmesh
                patrolToNewSpot(Mob, patrolRadius, patrolPos);
            }
        }
        // Finds a new spot to patrol too if mob gets stuck
        if(Mob.hasPath && Mob.velocity.magnitude == 0){
            patrolToNewSpot(Mob, patrolRadius, patrolPos);
        }
        // Enters charge state if the monster detects the player
        if(Brain.detectsPlayer && !Brain.isHiding){
            Mob.velocity = Vector3.zero;
            animator.SetBool("isCharging", true);
        }
    }

    

    // Finds a random location on the navmesh within a radius around the player, then returns it
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



    // Finds a new spot that the monster can patrol to, then tells it to go there
    void patrolToNewSpot(NavMeshAgent Mob, float patrolRadius, Vector3 patrolPos){
        patrolPos = RandomNavmeshLocation(patrolRadius);
        Mob.SetDestination(patrolPos);
        Mob.speed = 3; // For testing purposes
    }



    // Checks if the monster is able to ambush the player
    void checkAmbushValidity(Animator animator, NavMeshAgent Mob, Brain mobBrain, float patrolRadius, Vector3 patrolPos){
        // Initializes raycasting variables
        Ray monVis = new Ray(Mob.transform.position, (Player.transform.position-Mob.transform.position));
        RaycastHit hit;
        Vector3 playerDir = Player.transform.position - Mob.transform.position;
        playerDir.Normalize();
        float playerAngle = Mathf.Acos(Vector3.Dot(playerDir, Mob.transform.forward));
        float visionAngle = 1f;
        float distance = Vector3.Distance(Mob.transform.position, Player.position);

        // If able to raycast to player, do not change to ambush behaviour; keep patrolling instead
        if(playerAngle < visionAngle && Physics.Raycast(monVis, out hit, 25) && distance >= MobAmbushDistance){
            if(hit.collider.tag != "Player"){
                animator.SetBool("isAmbushing", true);
            }else{
                patrolToNewSpot(Mob, patrolRadius, patrolPos);
            }
        }else{
            patrolToNewSpot(Mob, patrolRadius, patrolPos);
        }
    }
}
