using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ElevatorSceneControl : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Video.VideoPlayer videoPlayer;
    [SerializeField]
    private FMODUnity.StudioEventEmitter videoAudio;
    [SerializeField]
    private FMODUnity.StudioEventEmitter elevatorSFX;
    [SerializeField] float skipHoldTime = 1;
    [SerializeField] GameObject skipSliderObj;
    Slider skipSlider;

    float skipTimer = 0;

    [SerializeField]
    private float beforeVideoWaitTime = 5.0f;
    [SerializeField]
    private float afterVideoWaitTime = 1.0f;

    private FadeController fadeController;
    //used to localize player in next scene
    [SerializeField] GameObject player;
    [SerializeField] GameObject elevConsole;

    private bool videoFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        fadeController = this.AddComponent<FadeController>();
        fadeController.FadeIn(0.25f);
        skipSlider = skipSliderObj.GetComponent<Slider>();
        videoPlayer.started += VideoStarted;
        videoPlayer.loopPointReached += VideoFinished;

        videoFinished = false;

        StartCoroutine(WaitAndStartVideo());
    }

    bool ending = true;
    // Update is called once per frame
    void Update()
    {
        if (!videoFinished)
        {
            if (Time.timeScale == 0)
            {
                videoPlayer.Pause();
                videoAudio.EventInstance.setPaused(true);
            }
            if (!videoPlayer.isPlaying && Time.timeScale == 1)
            {
                videoPlayer.Play();
                videoAudio.EventInstance.setPaused(false);
            }
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            skipSliderObj.SetActive(true);
            skipTimer += Time.deltaTime;
            skipSlider.value = skipTimer / skipHoldTime;
            if (skipTimer > skipHoldTime && ending)
            {
                ending = false;
                fadeController.FadeOutToSceen(0.25f, 1);
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
        videoPlayer.started -= VideoStarted;
    }

    void VideoFinished(UnityEngine.Video.VideoPlayer source)
    {
        Debug.Log("video finished");
        videoFinished = true;

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
        fadeController.FadeOutToSceen(0.25f, 1); 
    }
}
