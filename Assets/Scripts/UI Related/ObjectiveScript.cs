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
    
    public void activateObjective(ojbectives obj)
    {
        for(int i = 0;i < toggleslist.Count; i++)
        {
            if(toggleslist[i].objectiveType == obj)
            {
                cumulatveY += toggleslist[i].popIn(-cumulatveY);
                active.Add(toggleslist[i]);
                break;
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
    public TMP_Text timerText;
    public DateTime startTime;

    public static TimeSpan timeElapsed {get; private set;}

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
        if (MainMenuScript.speedRun) {
            ObjectiveScript.timeElapsed = DateTime.Now - startTime;
            timerText.text = "Time: " + ObjectiveScript.timeElapsed.ToString(@"mm\:ss\:ff");
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && (inputCounter < 3))
        {
            inputCounter++;
            if (inputCounter == 1)
            {
                deActivateObjective(ojbectives.move);
            }
            
        }

        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)))
        {
                deActivateObjective(ojbectives.switchItems);
        }

    }

    public void AddToActiveUI(GameObject objective)
    {
        if(!objective.activeSelf){
            // get original position of ui element and place it at an offset
            var rt = objective.transform.GetComponent<RectTransform>();
            var position = rt.anchoredPosition;
            rt.anchoredPosition = new Vector2(position.x, position.y + UIyOffset);
            ActiveUI.Add(objective);
            objective.SetActive(true);
            // subtract the ui elements height from the current y offset
            UIyOffset -= rt.rect.height/2;
        }
    }

    public void RemoveFromActiveUI(GameObject objective){
        int index = ActiveUI.FindIndex(x => x.name == objective.name);
        if(index > -1){
            objective.SetActive(false);
            ActiveUI.RemoveAt(index);
            // get height of element just removed, adjust all current elements, add to current y offset
            float height = objective.transform.GetComponent<RectTransform>().rect.height;
            AdjustActiveUI(index, height/2);
            UIyOffset += height/2;
        }
    }

    public void AdjustActiveUI(int index, float height){
        for(int i = index; i < ActiveUI.Count; i++){
            var rt = ActiveUI[i].transform.GetComponent<RectTransform>();
            var position = rt.anchoredPosition;
            rt.anchoredPosition = new Vector2(position.x, position.y + height);
            float temp = position.y + UIyOffset;
        }
    }

    public void SetUIElements(bool status){
        foreach(GameObject elem in ActiveUI){
            elem.SetActive(status);
        }
    }
}
