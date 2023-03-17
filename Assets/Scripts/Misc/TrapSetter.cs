using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSetter : MonoBehaviour
{
    [SerializeField] TrapTrapper trapper;

    [SerializeField] private float setTime = 2;
    [SerializeField] private float DelayTime = 2;

    //materials for different states
    [SerializeField] private Material powering;
    [SerializeField] private Material ready;
    [SerializeField] private Material active;
    [SerializeField] private Material InteractableOutline;

    [SerializeField] private MeshRenderer[] blocks;

    //track what state the trap is in
    public enum State
    {
        off,
        charged,
        set,
        triggered
    }
    public State currentState = State.off;

    MeshRenderer rend;
    bool PlayerContact = false;
    Inventory inventory;
    float timer = 0;

    Material[] outOnArr;
    Material[] outOffArr;

    private FMODUnity.StudioEventEmitter emitter;

    public float GetStartTime() { return setTime; }
    public float GetTimerRatio() { return (timer / setTime); }

    private void Start()
    {
        //setup the outline system
        rend = gameObject.GetComponent<MeshRenderer>();
        rend.enabled = false;
        outOnArr = blocks[0].materials;
        outOffArr = blocks[0].materials;
        outOnArr[1] = InteractableOutline;
        foreach (MeshRenderer b in blocks)
        {
            b.materials = outOffArr;
        }

        // setup sound emmiter
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    private void Update()
    {
        //check if trap can be charged
        if (currentState == State.off)
        {
            if (PlayerContact && Input.GetMouseButton(0) && inventory.EquippedIsElectricalEquipment() && GameStateManager.GeneratorOn && timer <= setTime)
            {
                ElectricalEquipment.sfxUpdateTarget = gameObject;
                //accumulate charge
                if (timer == 0) { rend.enabled = true; rend.material = powering; }
                timer += Time.deltaTime;
            }
            else if (timer > setTime)
            {
                //if sufficanly charged, and no longer being charged
                StartCoroutine(powerUp());
            }
            else
            {
                //reset if player stops
                rend.enabled = false;
                timer = 0;
                emitter.Stop();
            }
            //vissualy indicate that trap is ready
            if (timer > setTime) { rend.material = ready; }
        }

    }

    //highlight trap blocks, and maintain contact for charging
    private void OnTriggerEnter(Collider collision)
    {
        if (GameStateManager.GeneratorOn && collision.transform.tag == "Player")
        {
            foreach (MeshRenderer b in blocks)
            {
                b.materials = outOnArr;
            }
            PlayerContact = true;
            inventory = collision.transform.gameObject.GetComponent<InventoryManager>().inventory;
        }
    }

    //break contact and unhighlight
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
        trapper.enabled = true;

        emitter.Play();
    }
}
