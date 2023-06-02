using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ObjectiveScript : MonoBehaviour
{
    public enum ojbectives
    {
        move,
        switchItems,
        power,
        pvtm,
        document,
        escape,
        flash,
        camera,
    }

    [SerializeField] List<ObjectiveToggle> toggleslist = new List<ObjectiveToggle>();
     float cumulatveY;
     List<ObjectiveToggle> active = new List<ObjectiveToggle>();
    public static bool equipedisEE = true;

    public void activateObjective(ojbectives obj)
    {
        for(int i = 0;i < toggleslist.Count; i++)
        {
            if(toggleslist[i].objectiveType == obj)
            {
                if (toggleslist[i].objectiveType == ojbectives.switchItems)
                {
                    toggleslist[i].popIn(0);
                }
                else
                {
                    cumulatveY += toggleslist[i].popIn(-cumulatveY);
                    active.Add(toggleslist[i]);
                    break;
                }
            }
        }
    }

    public void deActivateObjective(ojbectives obj)
    {
        bool hasRemoved = false;
        int toRemove = -1;
        float yOffset = 0;
        for (int i = 0; i < active.Count; i++)
        {
            //remove objective if found
            if (active[i].objectiveType == obj)
            {
                hasRemoved = true;
                toRemove = i;
                yOffset = active[i].popOut();
                cumulatveY -= yOffset;
            }
            //adjust remaining objectives
            if (hasRemoved) { active[i].adjustY(yOffset); }
        }
        if (hasRemoved) { active.RemoveAt(toRemove); }
    }

    public int inputCounter = 0;

    public GameObject MoveText;
    public GameObject switchText;
    public GameObject PowerObj;
    public GameObject PVTMObj;
    public GameObject DocumentObj;
    public GameObject EscapeObj;
    public GameObject FlashObj;
    public GameObject CameraObj;
    [SerializeField] GameObject TrapObj;
    public static bool playerIsTrap = false;
    public TMP_Text timerText;
    public DateTime startTime;
    public DateTime pauseTime;

    UnityEngine.UI.Slider trapSlider;
    float trapTimer = 0;

    public static TimeSpan timeElapsed {get; private set;}
    public static TimeSpan timePaused {get; private set;}
    public static TimeSpan timeDifference = TimeSpan.Zero;

    private List<GameObject> ActiveUI = new List<GameObject>();
    private float UIyOffset = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (MainMenuScript.speedRun) {
            startTime = DateTime.Now;
            timerText.gameObject.SetActive(true);
        }
        //AddToActiveUI(MoveText);
        //AddToActiveUI(FlashObj);
        //AddToActiveUI(PowerObj);
        StartCoroutine(AnimInitObjs());
        trapSlider = TrapObj.GetComponentInChildren<UnityEngine.UI.Slider>();
        TrapObj.SetActive(false);
    }

    IEnumerator AnimInitObjs()
    {
        yield return new WaitForSeconds(0.5f);
        activateObjective(ojbectives.move);
        yield return new WaitForSeconds(0.25f);
        activateObjective(ojbectives.power);
        yield return new WaitForSeconds(0.25f);
        activateObjective(ojbectives.flash);
    }



    // Update is called once per frame
    void Update()
    {
        if (MainMenuScript.speedRun && Time.timeScale == 0) {
            ObjectiveScript.timePaused = ObjectiveScript.timeElapsed;
        }
        if (MainMenuScript.speedRun && Time.timeScale == 1) {
            ObjectiveScript.timeElapsed = DateTime.Now - startTime;
            if (ObjectiveScript.timePaused != TimeSpan.Zero) {
                ObjectiveScript.timeDifference = ObjectiveScript.timeDifference.Add(ObjectiveScript.timePaused.Subtract(ObjectiveScript.timeElapsed));
                ObjectiveScript.timePaused = TimeSpan.Zero;
            }
            timerText.text = "TIME: " + ObjectiveScript.timeElapsed.Add(ObjectiveScript.timeDifference).ToString(@"mm\:ss\:ff");
        }
        if ((Input.GetKeyDown(KeyMapper.forward) || Input.GetKeyDown(KeyMapper.backward) || Input.GetKeyDown(KeyMapper.left) || Input.GetKeyDown(KeyMapper.right)) && (inputCounter < 3))
        {
            inputCounter++;
            if (inputCounter == 1)
            {
                deActivateObjective(ojbectives.move);
            }
            
        }
        if (playerIsTrap)
        {
            TrapObj.SetActive(true);
            if(Input.GetKey(KeyMapper.primary) && equipedisEE)//set true to EE Equiped
            {
                trapSlider.gameObject.SetActive(true);
                trapTimer += Time.deltaTime;
                trapSlider.value = trapTimer / TrapSetter.chargingTime;
            }
            else
            {
                trapSlider.gameObject.SetActive(false); 
                trapTimer = 0;
                trapSlider.value = 0;
            }

        }
        else
        {
            TrapObj.SetActive(false);
        }
    }
}
