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
        // Exits chase state if the player is in a hiding hole, otherwise charges them
        if(mobBrain.isHiding){
            mobBrain.detectsPlayer = false;
            Mob.Stop();
            Mob.ResetPath();
            animator.SetBool("isChasing", false);
        }else{
            Debug.Log("charging");
            playerPos = Player.position;
            Mob.SetDestination(playerPos);
            Mob.GetComponentInChildren<MonVineStateMachine>().currentState = MonVineStateMachine.state.charge;
        }
        // Stops chasing the player after a certain amount of time
        if(timeSpentCharging >= mobBrain.maxChaseTimer){
            mobBrain.detectsPlayer = false;
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
