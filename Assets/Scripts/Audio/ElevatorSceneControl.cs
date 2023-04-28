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

    // Start is called before the first frame update
    void Start()
    {
        fadeController = this.AddComponent<FadeController>();
        fadeController.FadeIn(0.25f);
        skipSlider = skipSliderObj.GetComponent<Slider>();
        videoPlayer.started += VideoStarted;
        videoPlayer.loopPointReached += VideoFinished;

        StartCoroutine(WaitAndStartVideo());
    }

    bool ending = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            skipSliderObj.SetActive(true);
            skipTimer += Time.deltaTime;
            skipSlider.value = skipTimer / skipHoldTime;
            if (skipTimer > skipHoldTime && ending)
            {
                ending = false;
                PlayerMovement.posRelElevator = player.transform.position + new Vector3(44.52f, 0.05f, -51.1f); //vector is difference in location between scenes
                PlayerMovement.savedViewDir = player.transform.eulerAngles;
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
        fadeController.FadeOutToSceen(0.25f, 1); ;
    }
}
