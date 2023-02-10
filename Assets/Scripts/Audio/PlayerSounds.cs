using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerSounds : MonoBehaviour
{
    public enum CHASE_STATES { CHASING, PATROLLING };

    [SerializeField]
    private FMODUnity.StudioEventEmitter breathingEmitter = null;

    // probably just temp for now
    public bool muted = false;

    // Start is called before the first frame update
    void Start()
    {
        if (breathingEmitter == null)
            breathingEmitter = GetComponent<FMODUnity.StudioEventEmitter>();

        if (!breathingEmitter.IsPlaying())
            breathingEmitter.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Chase()
    {
        if (muted)
            return;

        if (!breathingEmitter.IsPlaying())
            breathingEmitter.Play();

        //StartCoroutine(WaitAndChangeState(0.75f, MUSIC_STATES.CHASING));
        SetState(CHASE_STATES.CHASING);
    }

    public void EndChase()
    {
        SetState(CHASE_STATES.PATROLLING);
    }

    private void SetState(CHASE_STATES state)
    {
        breathingEmitter.SetParameter("MonsterChaseState", (float)state);
    }

    private IEnumerator WaitAndChangeState(float seconds, CHASE_STATES state)
    {
        yield return new WaitForSeconds(seconds);
        SetState(state);
    }
}
