using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnityEventTrigger : MonoBehaviour
{
    public enum TRIGGER { NONE, START, LATE_START, TRIGGER_ENTER, TRIGGER_EXIT };

    [Serializable]
    public class Event : UnityEvent { }

    public TRIGGER trigger = TRIGGER.NONE;

    public Event callEvent;

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

    public void Execute()
    {
        callEvent.Invoke();
    }
}
