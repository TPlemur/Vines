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

        if (distance < MobDetectionDistance)
        {
            Vector3 dirToPlayer = transform.position - Player.transform.position;

            Vector3 newPos = transform.position - dirToPlayer;

            Mob.SetDestination(newPos);
            

            //Shoot();
        }
        //animating the bear

        if (Mob.velocity.x > 0 || Mob.velocity.z > 0){
            animator.Play("RunForward");
        }else{
            animator.Play("Idle");
        }
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
