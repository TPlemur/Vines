using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlinker : MonoBehaviour
{

    public float onTime = 4;
    public float onTimeFuzz = 2;
    public float offTime = 2;
    public float offTimeFuzz = 1;
    Light lightObj;
    float timer = 0;
    float timerTarget;
    // Start is called before the first frame update
    void Start()
    {
        lightObj = gameObject.GetComponent<Light>();
        lightObj.enabled = true;
        timerTarget = onTime + Random.value*onTimeFuzz - onTimeFuzz/2;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timerTarget)
        {
            if (lightObj.enabled)
            {
                lightObj.enabled = false;
                timerTarget = offTime + Random.value * offTimeFuzz - offTimeFuzz / 2;
                timer = 0;
            }
            else
            {
                lightObj.enabled = true;
                timerTarget = onTime + Random.value * onTimeFuzz - onTimeFuzz / 2;
                timer = 0;
            }
        }
    }
}
