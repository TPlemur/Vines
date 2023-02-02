using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MonsterMusic : MonoBehaviour
{
    public enum MUSIC_STATES { CHASING, PATROLLING };

    private FMODUnity.StudioEventEmitter emitter = null;

    // Start is called before the first frame update
    void Start()
    {
        if (emitter == null)
            emitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Chase()
    {
        if (!emitter.IsPlaying())
            emitter.Play();

        //StartCoroutine(WaitAndChangeState(0.75f, MUSIC_STATES.CHASING));
        SetState(MUSIC_STATES.CHASING);
    }

    public void EndChase()
    {
        SetState(MUSIC_STATES.PATROLLING);
    }

    private void SetState(MUSIC_STATES state)
    {
        emitter.SetParameter("MonsterChaseState", (float)state);
    }

    private IEnumerator WaitAndChangeState(float seconds, MUSIC_STATES state)
    {
        yield return new WaitForSeconds(seconds);
        SetState(state);
    }
}
