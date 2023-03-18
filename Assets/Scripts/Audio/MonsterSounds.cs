using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;


public class MonsterSounds : MonoBehaviour
{
    public enum MONSTER_STATES { IDLE, CHASING, CLICK, SWALLOW, ROAR, GROWL, HOWL };

    public FMODUnity.StudioEventEmitter emitter = null;

    FMOD.Studio.EVENT_CALLBACK markerCallback;

    // Start is called before the first frame update
    void Start()
    {
        if (emitter == null)
            emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        emitter.Play();

        markerCallback = new FMOD.Studio.EVENT_CALLBACK(MarkerEventCallback);
        emitter.EventInstance.setCallback(MarkerEventCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    // Update is called once per frame
    void Update()
    {
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    FMOD.RESULT MarkerEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, System.IntPtr _event, System.IntPtr parameterPtr)
    {
        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
        string marker = parameter.name;

        Debug.Log(marker);

        return FMOD.RESULT.OK;
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
        StartCoroutine(WaitAndChangeState(0.01f, MONSTER_STATES.IDLE));
    }
    public void Roar()
    {
        SetDestinationIDValue(0);
        SetState(MONSTER_STATES.ROAR);
        StartCoroutine(WaitAndChangeState(0.01f, MONSTER_STATES.IDLE));
    }
    public void RoarQuick()
    {
        SetDestinationIDValue(1);
        SetState(MONSTER_STATES.ROAR);
        StartCoroutine(WaitAndChangeState(0.01f, MONSTER_STATES.IDLE));
    }
    public void Growl()
    {
        SetState(MONSTER_STATES.GROWL);
        StartCoroutine(WaitAndChangeState(0.01f, MONSTER_STATES.IDLE));
    }
    public void Click()
    {
        SetState(MONSTER_STATES.CLICK);
        StartCoroutine(WaitAndChangeState(0.01f, MONSTER_STATES.IDLE));
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
