using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MixerEventTrigger : MonoBehaviour
{
    [Serializable]
    public class Event : UnityEvent<float> { }

    [SerializeField]
    public Event callEvent = new Event();

    //public Event callEvent { get { return _callEvent; } set { _callEvent = value; } }

    // have enum for more params than just bus volume if ever needed
    //enum MIXER_PARAM_TYPE { };

    [SerializeField]
    private MixerController.MIXER_BUS mixerBus;

    enum TRIGGER { NONE, START, LATE_START };

    [SerializeField]
    private TRIGGER trigger = TRIGGER.NONE;

    // Start is called before the first frame update
    void Start()
    {
        if (trigger == TRIGGER.START)
            Execute();

        if (trigger == TRIGGER.LATE_START)
            StartCoroutine(LaunchLateStart());
    }

    private IEnumerator LaunchLateStart()
    {
        const float seconds = 0.001f;
        yield return new WaitForSeconds(seconds);

        LateStart();
    }

    private void LateStart()
    {
        Execute();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Execute()
    {
        float val = MixerController.GetBusVolume(mixerBus);
        callEvent.Invoke(val);
    }
}
