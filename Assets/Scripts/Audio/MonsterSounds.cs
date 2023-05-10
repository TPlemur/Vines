using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;


public class MonsterSounds : MonoBehaviour
{
    public enum MONSTER_STATES { IDLE, CHASING, CLICK, SWALLOW, ROAR, GROWL, HOWL };

    public FMODUnity.StudioEventEmitter emitter = null;

    const float coroutineWaitTime = 0.2f;

    // Variables that are modified in the callback need to be part of a seperate class.
    // This class needs to be 'blittable' otherwise it can't be pinned in memory.
    [StructLayout(LayoutKind.Sequential)]
    class MarkerInfo
    {
        public bool newMarker = false;
        public FMOD.StringWrapper markerName = new FMOD.StringWrapper();
    }

    MarkerInfo markerInfo;
    GCHandle timelineHandle;

    FMOD.Studio.EVENT_CALLBACK markerCallback;

    Dictionary<string, MONSTER_STATES> transitionQueue = new Dictionary<string, MONSTER_STATES>();

    Dictionary<MONSTER_STATES, float> arrivalDistanceMultipliers = new Dictionary<MONSTER_STATES, float>();
    float defaultArrivalDistanceMultiplier = 1.0f;
    Dictionary<MONSTER_STATES, float> transitionDistanceMultipliers = new Dictionary<MONSTER_STATES, float>();
    float defaultTransitionDistanceMultiplier = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (emitter == null)
            emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        emitter.Play();

        markerCallback = new FMOD.Studio.EVENT_CALLBACK(MarkerEventCallback);
        markerInfo = new MarkerInfo();
        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(markerInfo, GCHandleType.Pinned);
        // Pass the object through the userdata of the instance
        emitter.EventInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        emitter.EventInstance.setCallback(MarkerEventCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        arrivalDistanceMultipliers[MONSTER_STATES.IDLE] = 0.5f;
        emitter.EventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, emitter.OverrideMaxDistance * arrivalDistanceMultipliers[MONSTER_STATES.IDLE]);
    }

    // Update is called once per frame
    void Update()
    {
        // updated queued transition if necessary
        if (markerInfo.newMarker)
        {
            markerInfo.newMarker = false;

            string trimmed = TrimMarkerName(markerInfo.markerName);
            MONSTER_STATES state = MarkerTrimmedToState(trimmed);

            // temp / experimental
            if (arrivalDistanceMultipliers.ContainsKey(state))
                emitter.EventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, emitter.OverrideMaxDistance * arrivalDistanceMultipliers[state]);
            else
                emitter.EventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, emitter.OverrideMaxDistance * defaultArrivalDistanceMultiplier);

            if (transitionQueue.ContainsKey(trimmed))
            {
                SetState(transitionQueue[trimmed]);
                transitionQueue.Remove(trimmed);
            }
        }
    }

    private string StateToMarkerTrimmed(MONSTER_STATES state)
    {
        switch (state)
        {
            case MONSTER_STATES.IDLE:
                return "Idle";
            case MONSTER_STATES.CHASING:
                return "Chasing";
            case MONSTER_STATES.CLICK:
                return "Click";
            case MONSTER_STATES.SWALLOW:
                return "";
            case MONSTER_STATES.ROAR:
                return "Roar";
            case MONSTER_STATES.GROWL:
                return "Growl";
            case MONSTER_STATES.HOWL:
                return "Howl";
        }

        return "";
    }
    private MONSTER_STATES MarkerTrimmedToState(string trimmed)
    {
        switch (trimmed)
        {
            case "Idle":
                return MONSTER_STATES.IDLE;
            case "Chasing":
                return MONSTER_STATES.CHASING;
            case "Click":
                return MONSTER_STATES.CLICK;
            case "Roar":
                return MONSTER_STATES.ROAR;
            case "Growl":
                return MONSTER_STATES.GROWL;
            case "Howl":
                return MONSTER_STATES.HOWL;
        }

        return MONSTER_STATES.IDLE;
    }
    private string TrimMarkerName(string name)
    {
        return name.Substring(0, name.LastIndexOf(" "));
    }

    private void QueueTransition(string trimmed, MONSTER_STATES dest)
    {
        transitionQueue[trimmed] = dest;
    }
    private void QueueTransition(MONSTER_STATES state, MONSTER_STATES dest)
    {
        transitionQueue[StateToMarkerTrimmed(state)] = dest;
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT MarkerEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, System.IntPtr _event, System.IntPtr parameterPtr)
    {
        // retrieve pointer to event instance
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(_event);
        // Retrieve the user data
        System.IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

        // checks
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        if (timelineInfoPtr == System.IntPtr.Zero)
            return FMOD.RESULT.OK;

        // Get the object to store beat and marker details
        GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
        MarkerInfo markerInfo = (MarkerInfo)timelineHandle.Target;
        
        switch (type)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
            {
                var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                markerInfo.markerName = parameter.name;
                markerInfo.newMarker = true;
                break;
            }
        }

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
        //StartCoroutine(WaitAndChangeState(0.11f, MONSTER_STATES.IDLE));
        SetState(MONSTER_STATES.CHASING);
    }

    public void EndChase()
    {
        Howl();
    }

    public void Howl()
    {
        SetState(MONSTER_STATES.HOWL);

        //StartCoroutine(WaitAndChangeState(coroutineWaitTime, MONSTER_STATES.IDLE));
        QueueTransition(MONSTER_STATES.HOWL, MONSTER_STATES.IDLE);
    }
    public void Roar()
    {
        SetDestinationIDValue(0);
        SetState(MONSTER_STATES.ROAR);

        //StartCoroutine(WaitAndChangeState(coroutineWaitTime, MONSTER_STATES.IDLE));
        QueueTransition(MONSTER_STATES.ROAR, MONSTER_STATES.IDLE);
    }
    public void RoarQuick()
    {
        SetDestinationIDValue(1);
        SetState(MONSTER_STATES.ROAR);

        //StartCoroutine(WaitAndChangeState(coroutineWaitTime, MONSTER_STATES.IDLE));
        QueueTransition(MONSTER_STATES.ROAR, MONSTER_STATES.IDLE);
    }
    public void Growl()
    {
        SetState(MONSTER_STATES.GROWL);

        //StartCoroutine(WaitAndChangeState(coroutineWaitTime, MONSTER_STATES.IDLE));
        QueueTransition(MONSTER_STATES.GROWL, MONSTER_STATES.IDLE);
    }
    public void Click()
    {
        SetState(MONSTER_STATES.CLICK);

        //StartCoroutine(WaitAndChangeState(coroutineWaitTime, MONSTER_STATES.IDLE));
        QueueTransition(MONSTER_STATES.CLICK, MONSTER_STATES.IDLE);
    }

    private void SetState(MONSTER_STATES state)
    {
        //StopAllCoroutines();

        SetRandomTransitionValue();
        emitter.EventInstance.setParameterByName("MonsterState", (float)state);

        if (transitionDistanceMultipliers.ContainsKey(state))
            emitter.EventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, emitter.OverrideMaxDistance * transitionDistanceMultipliers[state]);
        else
            emitter.EventInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, emitter.OverrideMaxDistance * defaultTransitionDistanceMultiplier);
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
