using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class trapScript : MonoBehaviour
{
    public float setTime = 2;
    public bool set = false;
     

    [SerializeField] private Material powering;
    [SerializeField] private Material ready;
    [SerializeField] private Material active;

    MeshRenderer rend;
    bool PlayerContact = false;
    bool ispowering = false;
    bool charged = false;
    bool triggered = false;
    float timer = 0;

    private void Update()
    {
        //check if trap can be charged
        if (!charged && !set) {
            if (PlayerContact && Input.GetKey(KeyCode.F))
            {
                //accumulate charge
                if (timer == 0) { rend.enabled = true; rend.material = powering; }
                timer += Time.deltaTime;
            }
            else if(timer > setTime)
            {
                //if sufficanly charged, and no longer being charged
                StartCoroutine(powerUp());
            }
            else
            {
                //reset if player stops
                rend.enabled = false;
                timer = 0;
            }
            //vissualy indicate that trap is ready
            if (timer > setTime) { rend.material = ready; }
        }

    }

    private void Start()
    {
        rend = gameObject.GetComponent<MeshRenderer>();
        rend.enabled = false;
    }


    private void OnTriggerEnter(Collider collision)
    {
        //send collision to the approprate handeler
        if(collision.transform.tag == "Player")
        {
            PlayerContact = true;
            if (set && !triggered)
            {
                triggered = true;
                StartCoroutine(trapPlayer(collision.gameObject));
            }
        }
        if(set && !triggered && collision.transform.tag == "Monster" )
        {
            triggered = true;
            StartCoroutine(trapMonster(collision.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerContact = false;
    }

    //wait two seconds then activate
    IEnumerator powerUp()
    {
        charged = true;
        yield return new WaitForSeconds(2);
        rend.material = active;
        set = true;
        charged = false;
    }

    //stop the monster for a bit
    IEnumerator trapMonster(GameObject mon)
    {
        NavMeshAgent monAgent = mon.GetComponent<NavMeshAgent>();
        float monSpeed = monAgent.speed;
        monAgent.speed = 0;
        yield return new WaitForSeconds(5);
        monAgent.speed = monSpeed;
        set = false;
        rend.enabled = false;
        timer = 0;
        triggered = false;
    }

    //stop the player for a bit
    IEnumerator trapPlayer(GameObject player)
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        float moveSpeed = pm.moveSpeed;
        pm.moveSpeed = 0;
        yield return new WaitForSeconds(5);
        pm.moveSpeed = moveSpeed;
        set = false;
        rend.enabled = false;
        timer = 0;
        triggered = false;
    }
}
