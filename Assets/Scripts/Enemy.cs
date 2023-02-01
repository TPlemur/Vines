using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent Mob;

    public Transform Player;

    public float MobDetectionDistance = 100.0f;
    public Animator animator;

    //public GameObject projectile;

    //public Transform projectilePoint;

    //public float timeBetweenAttacks;

    //bool alreadyAttacked;

    // Start is called before the first frame update
    void Start()
    {
        //Mob = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Player.transform.position);

        // Initializes raycasting variables
        Ray monVis = new Ray(transform.position, (Player.transform.position-transform.position));
        RaycastHit hit;

        // If able to raycast to player, move to player
        if(Physics.Raycast(monVis, out hit, MobDetectionDistance)){
            if(hit.collider.tag == "Player"){
                Debug.Log("I see you");
                Mob.SetDestination(Player.transform.position);
            }
        }
        if(!Mob.hasPath){
            Mob.SetDestination(RandomNavmeshLocation(100));
        }

        
            
                        //Shoot();
        //animating the bear

        // if (Mob.velocity.x > 0 || Mob.velocity.z > 0){
        //     animator.Play("RunForward");
        // }else{
        //     animator.Play("Idle");
        // }
    }

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
    

}
