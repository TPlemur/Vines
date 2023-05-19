using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DormantMonsterAudio : MonoBehaviour
{
    private FMODUnity.StudioEventEmitter emitter;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameStateManager.GeneratorOn)
                col.enabled = false;
            else
                emitter.Play();
        }
    }
}
