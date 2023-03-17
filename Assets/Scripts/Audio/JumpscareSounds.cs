using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpscareSounds : MonoBehaviour
{
    [SerializeField]
    private Transform jumpscareMonster;
    [SerializeField]
    private FMODUnity.StudioEventEmitter monsterJumpscareEmitter;

    [SerializeField]
    private bool triggerOnStart = false;

    // Start is called before the first frame update
    void Start()
    {
        if (triggerOnStart)
            StartJumpscare();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartJumpscare()
    {
        monsterJumpscareEmitter.transform.parent = jumpscareMonster;
        monsterJumpscareEmitter.transform.localPosition = Vector3.zero;
        monsterJumpscareEmitter.Play();
    }
}
