using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSceneControl : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Video.VideoPlayer videoPlayer;
    [SerializeField]
    private FMODUnity.StudioEventEmitter videoAudio;
    [SerializeField]
    private FMODUnity.StudioEventEmitter elevatorSFX;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.started += VideoStarted;
        videoPlayer.loopPointReached += VideoFinished;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void VideoStarted(UnityEngine.Video.VideoPlayer source)
    {
        videoAudio.Play();
    }

    void VideoFinished(UnityEngine.Video.VideoPlayer source)
    {
        Debug.Log("video finished");

        elevatorSFX.EventInstance.setParameterByName("State", 0);

        // transition to next scene, hoping for a little bit of time for the elevator sfx to transition off, but that can happen during the transition to the next scene
    }
}
