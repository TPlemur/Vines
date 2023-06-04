using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSetter : MonoBehaviour
{
    [SerializeField] TrapTrapper trapper;

    [SerializeField] private float setTime = 2;
    [SerializeField] private float DelayTime = 2;

    [SerializeField] private float sparkTime = 5;
    private float sparkTimer = 0;
    [SerializeField] private GameObject sparkObject;

    //materials for different states
    [SerializeField] private Material InteractableOutline;
    [SerializeField] private GameObject electricityArcHolder;
    [SerializeField] private ParticleSystem electricArc1;
    [SerializeField] private ParticleSystem electricArc2;
    [SerializeField] private float electricArcLength;
    [SerializeField] private Color electricArcPowerUpColor;
    [SerializeField] private Color electricArcReadyColor;
    [SerializeField] private Color electricArcActiveColor;

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

    public static float chargingTime;

    private ParticleSystem.MainModule EA1;
    private ParticleSystem.MainModule EA2;
    private void Start()
    {
        EA1 = electricArc1.main;
        EA2 = electricArc2.main;
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
            if (PlayerContact && LookAtTrap() && Input.GetMouseButton(0) && inventory.EquippedIsElectricalEquipment() && GameStateManager.GeneratorOn && timer <= setTime)
            {
                ElectricalEquipment.sfxUpdateTarget = gameObject;
                //accumulate charge
                if (timer == 0) { electricityArcHolder.SetActive(true); }
                timer += Time.deltaTime;
                EA1.startLifetime = (timer / setTime) * electricArcLength;
                EA2.startLifetime = (timer / setTime) * electricArcLength;
            }
            else if (timer > setTime)
            {
                //if sufficanly charged, and no longer being charged
                StartCoroutine(powerUp());
                timer = 0;
            }
            else
            {
                //reset if player stops
                rend.enabled = false;
                electricityArcHolder.SetActive(false);
                EA1.startColor = electricArcPowerUpColor;
                EA2.startColor = electricArcPowerUpColor;
                timer = 0;
                emitter.Stop();
            }
            //vissualy indicate that trap is ready
            if (timer > setTime) { EA1.startColor = electricArcReadyColor; EA2.startColor = electricArcReadyColor; }

            if (GameStateManager.GeneratorOn)
            {
                sparkTimer += Time.deltaTime;
                if (sparkTimer >= sparkTime)
                {
                    // spark
                    if (!sparkObject.activeSelf){
                        sparkObject.SetActive(true);
                    }
                    SparkSFX();
                    sparkTimer = 0;
                    StartCoroutine(SparkParticleDisable());
                }
            }
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
            chargingTime = setTime;
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
        ObjectiveScript.playerIsTrap = false;
    }

    private bool LookAtTrap()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit);
        if (hit.transform.gameObject.tag == "Electrical Equipment" || hit.transform.gameObject.tag == "Player")
        {
            ObjectiveScript.playerIsTrap = true;
            return true;
        }
        ObjectiveScript.playerIsTrap = false;
        return false;
    }

    //wait two seconds then activate
    IEnumerator powerUp()
    {
        currentState = State.charged;
        yield return new WaitForSeconds(DelayTime);
        EA1.startColor = electricArcActiveColor; 
        EA2.startColor = electricArcActiveColor;
        currentState = State.set;
        trapper.enabled = true;

        emitter.Play();
    }

    private void SparkSFX()
    {
        const string eventName = "event:/SFX/Items/Traps/Trap Shock";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        sound.setVolume(0.5f);
        sound.setPitch(2f);
        sound.start();
        sound.release();
    }

    private IEnumerator SparkParticleDisable()
    {
        yield return new WaitForSeconds(0.2f);
        sparkObject.SetActive(false);
    }
}
