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

    Vector3 patrolPos;

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
        Mob.GetComponentInChildren<MonVineStateMachine>().currentState = MonVineStateMachine.state.walk;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // If the monster doesn't have a path, finds random spot within a radius to patrol to
       if(!Mob.hasPath){
            patrolPos = RandomNavmeshLocation(patrolRadius);
            Mob.SetDestination(patrolPos);
            Mob.speed = 3; //Only for testing purposes
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
        if(Mob.GetComponent<Brain>().detectsPlayer){
            Debug.Log(Mob.GetComponent<Brain>().detectsPlayer);
            animator.SetBool("isChasing", true);
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
