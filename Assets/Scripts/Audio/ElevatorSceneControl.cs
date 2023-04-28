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
    float fadeOut = 1; //Set time of fade-out
    [SerializeField] float skipHoldTime = 1;
    [SerializeField] GameObject skipSliderObj;
    Slider skipSlider;

    float skipTimer = 0;

    [SerializeField]
    private float beforeVideoWaitTime = 5.0f;
    [SerializeField]
    private float afterVideoWaitTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        skipSlider = skipSliderObj.GetComponent<Slider>();
        videoPlayer.started += VideoStarted;
        videoPlayer.loopPointReached += VideoFinished;

        StartCoroutine(WaitAndStartVideo());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            skipSliderObj.SetActive(true);
            skipTimer += Time.deltaTime;
            skipSlider.value = skipTimer / skipHoldTime;
            if (skipTimer > skipHoldTime)
            {
                StartCoroutine(transitionToPlay());
            }
        }
        else {
            skipSliderObj.SetActive(false);
            skipTimer = 0;
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
