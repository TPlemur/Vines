using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSounds : MonoBehaviour
{
    public enum MONSTER_STATES { IDLE, CHASING, CLICK, SWALLOW, ROAR, GROWL, HOWL };

    public FMODUnity.StudioEventEmitter emitter = null;

    // Start is called before the first frame update
    void Start()
    {
        if (emitter == null)
            emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        emitter.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPreCharge()
    {
        // Handled by anim events
        Click(); // still trigger click here anyways though
        //StartCoroutine(WaitAndChangeState(1.0f, MONSTER_STATES.ROAR));
        //StartCoroutine(WaitAndChangeState(1.5f, MONSTER_STATES.CHASING));
    }

    public void StartChase()
    {
        //SetState(MONSTER_STATES.ROAR);
        //StartCoroutine(WaitAndChangeState(0.001f, MONSTER_STATES.IDLE));
        SetState(MONSTER_STATES.CHASING);
    }

    public void EndChase()
    {
        Howl();
    }

    public void Howl()
    {
        SetState(MONSTER_STATES.HOWL);
        StartCoroutine(WaitAndChangeState(0.1f, MONSTER_STATES.IDLE));
    }
    public void Roar()
    {
        SetDestinationIDValue(0);
        SetState(MONSTER_STATES.ROAR);
        StartCoroutine(WaitAndChangeState(0.1f, MONSTER_STATES.IDLE));
    }
    public void RoarQuick()
    {
        SetDestinationIDValue(1);
        SetState(MONSTER_STATES.ROAR);
        StartCoroutine(WaitAndChangeState(0.1f, MONSTER_STATES.IDLE));
    }
    public void Growl()
    {
        SetState(MONSTER_STATES.GROWL);
        StartCoroutine(WaitAndChangeState(0.1f, MONSTER_STATES.IDLE));
    }
    public void Click()
    {
        SetState(MONSTER_STATES.CLICK);
        StartCoroutine(WaitAndChangeState(0.1f, MONSTER_STATES.IDLE));
    }

    private void SetState(MONSTER_STATES state)
    {
        SetRandomTransitionValue();
        emitter.EventInstance.setParameterByName("MonsterState", (float)state);
    }
    private MONSTER_STATES GetState()
    {
        float state;
        emitter.EventInstance.getParameterByName("MonsterState", out state);
        return (MONSTER_STATES)state;
    }

    private IEnumerator WaitAndChangeState(float seconds, MONSTER_STATES state)
    {
        yield return new WaitForSeconds(seconds);
        SetState(state);
    }

    private void SetRandomTransitionValue()
    {
        emitter.EventInstance.setParameterByName("DestinationProbability", Random.value);
    }

    private void SetDestinationIDValue(int ID)
    {
        emitter.EventInstance.setParameterByName("DestinationID", (float)ID);
    }
}
