using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoSyncAudio : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer videoPlayer;
    private FMODUnity.StudioEventEmitter emitter;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        videoPlayer.started += VideoStarted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void VideoStarted(UnityEngine.Video.VideoPlayer source)
    {
        emitter.Play();
    }
}
