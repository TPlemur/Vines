using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSounds : MonoBehaviour
{
    public enum MONSTER_STATES { IDLE, SWALLOWING, ROAR };

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

    public void Roar()
    {
        SetState(MONSTER_STATES.ROAR);
        StartCoroutine(WaitAndChangeState(0.001f, MONSTER_STATES.IDLE));
    }

    private void SetState(MONSTER_STATES state)
    {
        SetRandomTransitionValue();
        emitter.EventInstance.setParameterByName("MonsterState", (float)state);
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
}
