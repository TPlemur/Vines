using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    Transform Player;
    NavMeshAgent Mob;
    Brain mobBrain;
    public float attackRange = 20;
    public float enemyspeed = 5;
    float timeSpentCharging;
    bool canStillSeePlayer = false;
    LayerMask mask;

    public float chargeSpeed;
    public float moveSpeed;
    Vector3 playerPos;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        //Mob.speed = enemyspeed;
        Mob.speed = chargeSpeed;
        mobBrain = Mob.GetComponentInChildren<Brain>();
        timeSpentCharging = 0;
        Debug.Log("IN CHASE STATE");
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        mask = LayerMask.GetMask("Player");

        // AUDIO
        // set FMOD ChaseState to chasing
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ChaseState", (float)MixerController.CHASE_STATE.CHASING);
        Mob.GetComponentInChildren<MonsterSounds>().StartChase();
        // set footstep intesity
        Mob.GetComponentInChildren<SplitjawFootstepController>().SetIntensity(0.8f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Initializes raycasting variables
        Ray monVis = new Ray(Mob.transform.position, (Player.transform.position-Mob.transform.position));
        RaycastHit hit;
        Vector3 playerDir = Player.transform.position - Mob.transform.position;
        playerDir.Normalize();
        float playerAngle = Mathf.Acos(Vector3.Dot(playerDir, Mob.transform.forward));
        float visionAngle = 1f;

        // Checks to see if the monster can still see the player and they're close
        if(playerAngle < visionAngle && Physics.Raycast(monVis, out hit, 10, mask)){
            canStillSeePlayer = true;
        }else{
            canStillSeePlayer = false;
        }
        // Exits chase state if the player is in a hiding hole (unless it can still see the player), otherwise charges them
        if(Brain.isHiding && !canStillSeePlayer){
            Brain.detectsPlayer = false;
            Mob.Stop();
            Mob.ResetPath();
            animator.SetBool("isChasing", false);
        }else{
            playerPos = Player.position;
            Mob.SetDestination(playerPos);
            Mob.GetComponentInChildren<MonVineStateMachine>().currentState = MonVineStateMachine.state.charge;
        }
        // Stops chasing the player after a certain amount of time
        if(timeSpentCharging >= mobBrain.maxChaseTimer){
            Brain.detectsPlayer = false;
            animator.SetBool("isChasing", false);
        }
        // Stops the monster from killing the player when the shield is deployed
        if(mobBrain.isShielded){
            animator.SetBool("isBlocked", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // set FMOD ChaseState to patrolling
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ChaseState", (float)MixerController.CHASE_STATE.PATROLLING);
        Mob.GetComponentInChildren<MonsterSounds>().EndChase();
        // set footstep intesity (safety for now at least)
        Mob.GetComponentInChildren<SplitjawFootstepController>().SetIntensity(0.4f);
    }
}
