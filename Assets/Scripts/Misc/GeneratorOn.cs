using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorOn : MonoBehaviour
{
    [SerializeField] GameObject OffLight;
    [SerializeField] GameObject onLight;
    [SerializeField] GeneratorVibe vibe;
    [SerializeField] float StartTime = 2;
    [SerializeField] float lightFlashRate = 0.25f;
    [HideInInspector] public bool Starting = false;
    [HideInInspector] public bool Started = false;
    float timer = 0;
    float flashTimer = 0;
    bool Lstate = false;

    public delegate void StartingEvent(float timerRatio);
    public delegate void StartedEvent();

    public event StartingEvent startingEvent = null;
    public event StartedEvent startedEvent = null;

    public float GetStartTime() { return StartTime; }
    public float GetTimerRatio() { return (timer / StartTime); }

    public void turnOn()
    {
        OffLight.SetActive(false);
        onLight.SetActive(true);
        vibe.enabled = true;
    }
    void FlipLights()
    {
        flashTimer += Time.deltaTime;
        if (flashTimer > lightFlashRate)
        {
            OffLight.SetActive(Lstate);
            //onLight.SetActive(!Lstate);
            Lstate = !Lstate;
            flashTimer = 0;
        }
    }

    public void FixedUpdate()
    {
        Starting = false;
    }

    public void LateUpdate()
    {
        if (!Started)
        {
            if (Starting)
            {
                timer += Time.deltaTime;
                FlipLights();
                if (timer > StartTime)
                {
                    startedEvent?.Invoke();
                    Started = true;
                    turnOn();
                }
                else
                {
                    startingEvent?.Invoke(GetTimerRatio());
                }
            }
            else
            {
                timer = 0;
                OffLight.SetActive(true);
                onLight.SetActive(false);
            }
        }
    }

}
