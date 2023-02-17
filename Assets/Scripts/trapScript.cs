using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class trapScript : MonoBehaviour
{
    [SerializeField] private float setTime = 2;
    [SerializeField] private float DelayTime = 2;
    [SerializeField] private float MonTrapTime = 5;
    [SerializeField] private float PlayerTrapTime = 5;

    enum State
    {
        off,
        isPowering,
        charged,
        set,
        triggered
    }
    bool set = false;
    State currentState = State.off;

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
        if (currentState == State.off) {
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
            if (currentState == State.set)
            {
                currentState = State.triggered;
                StartCoroutine(trapPlayer(collision.gameObject));
            }
        }
        if(currentState == State.set && collision.transform.tag == "Monster" )
        {
            currentState = State.triggered;
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
        currentState = State.charged;
        yield return new WaitForSeconds(DelayTime);
        rend.material = active;
        currentState = State.set;
    }

    //stop the monster for a bit
    IEnumerator trapMonster(GameObject mon)
    {
        NavMeshAgent monAgent = mon.GetComponent<NavMeshAgent>();
        float monSpeed = monAgent.speed;
        monAgent.speed = 0;
        yield return new WaitForSeconds(MonTrapTime);
        monAgent.speed = monSpeed;
        timer = 0;
        currentState = State.off;
    }

    //stop the player for a bit
    IEnumerator trapPlayer(GameObject player)
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        float moveSpeed = pm.moveSpeed;
        pm.moveSpeed = 0;
        yield return new WaitForSeconds(PlayerTrapTime);
        pm.moveSpeed = moveSpeed;
        timer = 0;
        currentState = State.off;
    }
}
