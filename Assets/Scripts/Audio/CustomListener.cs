using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomListener : MonoBehaviour
{
    [SerializeField]
    private GameObject player, cam;

    [SerializeField]
    private int listenerID;

    FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();

    FMOD.VECTOR playerPosition;

    void Update()
    {
        attributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(cam.transform.forward);
        attributes.up = FMODUnity.RuntimeUtils.ToFMODVector(cam.transform.up);
        attributes.position = FMODUnity.RuntimeUtils.ToFMODVector(cam.transform.position);
        playerPosition = FMODUnity.RuntimeUtils.ToFMODVector(player.transform.position);

        FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(listenerID, attributes, playerPosition);
    }
}
