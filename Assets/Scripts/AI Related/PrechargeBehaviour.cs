using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PrechargeBehaviour : StateMachineBehaviour
{
    float timer;
    Transform Player;
    GameObject PlayerObj;
    NavMeshAgent Mob;
    Brain mobBrain;
    float animationTime = 5.30f;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mob = animator.gameObject.GetComponentInParent<NavMeshAgent>();
        mobBrain = Mob.GetComponentInChildren<Brain>();
        timer = 0;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        animator.gameObject.GetComponent<MonVineStateMachine>().currentState = MonVineStateMachine.state.roar;
        mobBrain.investigating = false;
        // Stops the monster from trying to pursue player into hiding holes
        Mob.Stop();
        Mob.isStopped = true;
        Mob.speed = 0;
        Mob.ResetPath();

        // AUDIO
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ChaseState", (float)MixerController.CHASE_STATE.CHASING);
        Mob.GetComponentInChildren<MonsterSounds>().StartPreCharge();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        Mob.velocity = Vector3.zero;
        if(timer >= animationTime){
            Debug.Log("ROAR");
            animator.SetBool("isCharging", false);
            animator.SetBool("isChasing", true);
        }
        Mob.transform.LookAt(Player);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
