using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReceiverListener : MonoBehaviour
{
    public FMODUnity.StudioListener mainListener;

    public Transform parentTransform = null;

    public List<FMODUnity.StudioEventEmitter> receivers;

    // Start is called before the first frame update
    void Start()
    {
        SetParentTransform(parentTransform);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var emitter in receivers)
        {
            
        }
    }

    public void SetParentTransform(Transform parent)
    {
        parentTransform = parent;
        transform.parent = parentTransform;
        transform.localPosition = Vector3.zero;
    }

    static public void SetBusVolume(float volume)
    {
        MixerController.SetCameraReceiverVolume(volume);
    }
    static public float GetBusVolume()
    {
        return MixerController.GetCameraReceiverVolume();
    }
    static public FMOD.Studio.Bus GetBus()
    {
        return MixerController.GetCameraReceiverBus();
    }
}
