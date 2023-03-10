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

    [SerializeField] private Material powering;
    [SerializeField] private Material ready;
    [SerializeField] private Material active;
    [SerializeField] private Material InteractableOutline;

    [SerializeField] private MeshRenderer[] blocks;
    [SerializeField] private ColliderNotifyer playerSensor;

    enum State
    {
        off,
        charged,
        set,
        triggered
    }
    State currentState = State.off;

    MeshRenderer rend;
    bool PlayerContact = false;
    Inventory inventory;
    float timer = 0;

    Material[] outOnArr;
    Material[] outOffArr;

    private FMODUnity.StudioEventEmitter emitter;

    private void Update()
    {
        //check if trap can be charged
        if (currentState == State.off) {
            if (PlayerContact && Input.GetMouseButton(0) && inventory.EquippedIsElectricalEquipment() && GameStateManager.GeneratorOn)
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
        outOnArr = blocks[0].materials;
        outOffArr = blocks[0].materials;
        outOnArr[1] = InteractableOutline;
        foreach (MeshRenderer b in blocks)
        {
            b.materials = outOffArr;
        }

        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }


    private void OnTriggerEnter(Collider collision)
    {
        //send collision to the approprate handeler
        if(collision.transform.tag == "Player")
        {
            foreach (MeshRenderer b in blocks)
            {
                b.materials = outOnArr;
            }
            PlayerContact = true;
            inventory = collision.transform.gameObject.GetComponent<InventoryManager>().inventory;
            if (currentState == State.set && !Input.GetKey(KeyCode.LeftControl))
            {
                currentState = State.triggered;
                StartCoroutine(trapPlayer(collision.gameObject));
            }
        }
        if (currentState == State.set && collision.transform.tag == "Monster" )
        {
            currentState = State.triggered;
            StartCoroutine(trapMonster(collision.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerContact = false;
        foreach (MeshRenderer b in blocks)
        {
            b.materials = outOffArr;
        }
    }

    //wait two seconds then activate
    IEnumerator powerUp()
    {
        currentState = State.charged;
        yield return new WaitForSeconds(DelayTime);
        rend.material = active;
        currentState = State.set;

        emitter.Play();
    }

    //stop the monster for a bit
    IEnumerator trapMonster(GameObject mon)
    {
        ShockSFX();
        NavMeshAgent monAgent = mon.GetComponent<NavMeshAgent>();
        float monSpeed = monAgent.speed;
        monAgent.speed = 0;

        const float shockWaitTime = 0.25f;
        float duration = MonTrapTime;
        float shockDuration = shockWaitTime;
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

        monAgent.speed = monSpeed;
        timer = 0;
        currentState = State.off;

        emitter.Stop();
    }

    //stop the player for a bit
    IEnumerator trapPlayer(GameObject player)
    {
        ShockSFX();
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        float moveSpeed = pm.moveSpeed;
        pm.moveSpeed = 0;

        const float shockWaitTime = 0.25f;
        float duration = PlayerTrapTime;
        float shockDuration = shockWaitTime;
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

        pm.moveSpeed = moveSpeed;
        timer = 0;
        currentState = State.off;

        emitter.Stop();
    }

    private void ShockSFX()
    {
        const string eventName = "event:/SFX/Items/Traps/Trap Shock";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        sound.start();
        sound.release();
    }
}
