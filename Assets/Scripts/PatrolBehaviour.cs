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
    NavMeshAgent Mob;

    private MonsterSounds sounds = null;
    public MonsterMusic music;
    public PlayerSounds playerSounds;

    public float visionAngle = 1f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        /*if (sounds == null)
            sounds = GetComponent<MonsterSounds>();
        */
        Mob = animator.GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       timer += Time.deltaTime;

       /*Mob.SetDestination(RandomNavmeshLocation(patrolRadius));
            if (music)
                music.EndChase();
            if (playerSounds)
                playerSounds.EndChase();
        */
        float distance = Vector3.Distance(animator.transform.position, Player.position);
        if (distance < MobDetectionDistance)
            animator.SetBool("isChasing", true);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
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
