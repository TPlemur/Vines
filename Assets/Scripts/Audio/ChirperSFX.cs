using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChirperSFX : MonoBehaviour
{
    public LayerMask impactLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & impactLayerMask) != 0)
        {
            ImpactSFX();
        }
    }

    private void ImpactSFX()
    {
        const float volume = 0.8f; // might use velocity to determine or something
        // flashlight switch sound
        const string eventName = "event:/SFX/Items/Chirper/Impact";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        sound.setVolume(volume);
        sound.start();
        sound.release();
    }
}
