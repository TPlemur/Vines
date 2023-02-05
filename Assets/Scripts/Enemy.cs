using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class Enemy : MonoBehaviour
{
    public NavMeshAgent Mob;

    public Transform Player;

    private MonsterSounds sounds = null;
    public MonsterMusic music;

    public float MobDetectionDistance = 100.0f;
    public float patrolRadius = 100.0f;

    public float visionAngle = 1f;
    public Animator animator;

    //public GameObject projectile;

    //public Transform projectilePoint;

    //public float timeBetweenAttacks;

    //bool alreadyAttacked;

    // Start is called before the first frame update
    void Start()
    {
        //Mob = GetComponent<NavMeshAgent>();
        if (sounds == null)
            sounds = GetComponent<MonsterSounds>();
    }

    // Update is called once per frame
    void Update()
    {
        //commented out to allow for vine based sensing
        /*
        Vector3 playerDir = Player.transform.position - transform.position;
        playerDir.Normalize();
        float playerAngle = Mathf.Acos(Vector3.Dot(playerDir, transform.forward));

        // Initializes raycasting variables
        Ray monVis = new Ray(transform.position, (Player.transform.position-transform.position));
        RaycastHit hit;

        // If able to raycast to player, move to player
        if(playerAngle < visionAngle && Physics.Raycast(monVis, out hit, MobDetectionDistance)){
            if(hit.collider.tag == "Player"){
                // entering CHASE state from PATROLLING state or continuing CHASE state
                Mob.SetDestination(Player.transform.position);
                if (sounds)
                    sounds.Roar();
                if (music)
                    music.Chase();
            }
        }
        */

        // Patrols randomly if it cant see player
        if(!Mob.hasPath){
            // entering PATROLLING state from CHASE state or continuing PATROLLING state
            Mob.SetDestination(RandomNavmeshLocation(patrolRadius));
            if (music)
                music.EndChase();
        }

        
            
                        //Shoot();
        //animating the bear

        // if (Mob.velocity.x > 0 || Mob.velocity.z > 0){
        //     animator.Play("RunForward");
        // }else{
        //     animator.Play("Idle");
        // }
    }

    // Finds random location within a radius to patrol in
    public Vector3 RandomNavmeshLocation(float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit NavMeshEnemy;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshEnemy, radius, 1)) {
            finalPosition = NavMeshEnemy.position;            
        }
        return finalPosition;
    }

    // public void Shoot()
    // {
    //     if (!alreadyAttacked)
    //     {
    //         Rigidbody rb = Instantiate(projectile, projectilePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
    //         rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
    //         rb.AddForce(transform.up * 7, ForceMode.Impulse);

    //         alreadyAttacked = true;
    //         Invoke(nameof(ResetAttack), timeBetweenAttacks);
    //     }
    // }

    // private void ResetAttack()
    // {
    //     alreadyAttacked = false;
    // }
    
    public void setTargetLocation(Vector3 TargetPosition)
    {
        Mob.SetDestination(TargetPosition);
    }

}
