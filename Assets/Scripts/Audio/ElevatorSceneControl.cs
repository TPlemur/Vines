using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ElevatorSceneControl : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Video.VideoPlayer videoPlayer;
    [SerializeField]
    private FMODUnity.StudioEventEmitter videoAudio;
    [SerializeField]
    private FMODUnity.StudioEventEmitter elevatorSFX;
    [SerializeField]
    private Image fadeScreen;
    [SerializeField]
    float fadeOut = 1;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.started += VideoStarted;
        videoPlayer.loopPointReached += VideoFinished;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(transitionToPlay());
        }
    }

    void VideoStarted(UnityEngine.Video.VideoPlayer source)
    {
        videoAudio.Play();
    }

    void VideoFinished(UnityEngine.Video.VideoPlayer source)
    {
        Debug.Log("video finished");

        elevatorSFX.EventInstance.setParameterByName("State", 0);

        StartCoroutine(transitionToPlay());
        // transition to next scene, hoping for a little bit of time for the elevator sfx to transition off, but that can happen during the transition to the next scene
    }


    float timer = 0;

    IEnumerator transitionToPlay()
    {
        while (timer < fadeOut)
        {
            timer += Time.deltaTime;
            fadeScreen.color = new Color(0, 0, 0, timer/fadeOut);
            yield return null;
        }
        Debug.Log("End this!!");
        SceneManager.LoadScene(1);
    }
}
