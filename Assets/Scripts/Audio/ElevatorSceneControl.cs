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

    [SerializeField]
    private float beforeVideoWaitTime = 5.0f;
    [SerializeField]
    private float afterVideoWaitTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.started += VideoStarted;
        videoPlayer.loopPointReached += VideoFinished;

        StartCoroutine(WaitAndStartVideo());
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

        StartCoroutine(WaitAndEndScene());
    }

    IEnumerator WaitAndStartVideo()
    {
        yield return new WaitForSeconds(beforeVideoWaitTime);
        videoPlayer.Play();
    }

    IEnumerator WaitAndEndScene()
    {
        yield return new WaitForSeconds(afterVideoWaitTime);
        StartCoroutine(transitionToPlay());
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
