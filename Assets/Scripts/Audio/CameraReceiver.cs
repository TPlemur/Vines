using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReceiver : MonoBehaviour
{
    //public Transform transmitter;
    public FMODUnity.StudioListener mainListener;
    public FMODUnity.StudioEventEmitter receiver;
    public CameraReceiverListener cameraListener;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = this.transform.position - cameraListener.transform.position;
        //Vector3 test = mainListener.transform.position + offset;
        //Vector3 test2 = mainListener.transform.position + ((cameraListener.transform.rotation * mainListener.transform.rotation) * offset);
        //Vector3 test3 = mainListener.transform.position + ((cameraListener.transform.rotation * Quaternion.Inverse(mainListener.transform.rotation)) * offset);
        //Vector3 test4 = mainListener.transform.position + ((Quaternion.Inverse(cameraListener.transform.rotation) * mainListener.transform.rotation) * offset);
        //Vector3 test5 = mainListener.transform.position + ((cameraListener.transform.rotation) * offset);
        receiver.transform.position = mainListener.transform.position + ((cameraListener.transform.rotation * Quaternion.Inverse(mainListener.transform.rotation)) * offset); ;
    }
}
