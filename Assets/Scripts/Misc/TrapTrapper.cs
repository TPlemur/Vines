using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrapTrapper : MonoBehaviour
{
    [SerializeField] TrapSetter setter;

    [SerializeField] private float MonTrapTime = 5;
    [SerializeField] private float PlayerTrapTime = 5;

    //prevent double trigger
    bool triggered = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (setter.currentState == TrapSetter.State.set)
        {
            //send collision to the approprate handeler
            if (!triggered && collision.transform.tag == "Player")
            {
                triggered = true;
                StartCoroutine(trapPlayer(collision.gameObject));
            }
            if (!triggered && collision.transform.tag == "Monster")
            {
                triggered = true;
                StartCoroutine(trapMonster(collision.gameObject));
            }
        }
    }

    //stop the monster for a bit
    IEnumerator trapMonster(GameObject mon)
    {
        //play shock noise and set monster speed to zero
        ShockSFX();
        NavMeshAgent monAgent = mon.GetComponent<NavMeshAgent>();
        float monSpeed = monAgent.speed;
        monAgent.speed = 0;

        //set up timers
        const float shockWaitTime = 0.25f;
        float duration = MonTrapTime;
        float shockDuration = shockWaitTime;

        //wait for approprate trap time, and refresh shock noise as needed
        while (duration > 0)
        {
            if (shockDuration <= 0)
            {
                ShockSFX();
                shockDuration = shockWaitTime;
            }
            shockDuration -= Time.deltaTime;

            duration -= Time.deltaTime;
            yield return null;
        }

        //reset and turn off
        monAgent.speed = monSpeed;
        setter.currentState = TrapSetter.State.off;
        triggered = false;
    }

    //stop the player for a bit
    IEnumerator trapPlayer(GameObject player)
    {
        //play shock noise and set player speed to zero
        ShockSFX();
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        float moveSpeed = pm.moveSpeed;
        pm.moveSpeed = 0;

        //set up timers
        const float shockWaitTime = 0.25f;
        float duration = PlayerTrapTime;
        float shockDuration = shockWaitTime;

        //wait for approprate trap time, and refresh shock noise as needed
        while (duration > 0)
        {
            if (shockDuration <= 0)
            {
                ShockSFX();
                shockDuration = shockWaitTime;
            }
            shockDuration -= Time.deltaTime;

            duration -= Time.deltaTime;
            yield return null;
        }


        //Reset and turn off
        pm.moveSpeed = moveSpeed;
        setter.currentState = TrapSetter.State.off;
        triggered = false;
    }

    //start the shock noise effect
    private void ShockSFX()
    {
        const string eventName = "event:/SFX/Items/Traps/Trap Shock";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        sound.start();
        sound.release();
    }
}
