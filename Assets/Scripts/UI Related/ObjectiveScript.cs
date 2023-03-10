using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveScript : MonoBehaviour
{
    public int inputCounter = 0;

    public GameObject MoveText;
    public GameObject PowerObj;
    public GameObject PVTMObj;
    public GameObject DocumentObj;
    public GameObject EscapeObj;
    public GameObject FlashObj;
    public GameObject PauseMenu;
    public GameObject CameraObj;

    private List<GameObject> ActiveUI = new List<GameObject>();
    private float UIyOffset = 0;
    // Start is called before the first frame update
    void Start()
    {
        AddToActiveUI(MoveText);
        AddToActiveUI(FlashObj);
        AddToActiveUI(PowerObj);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else if(Time.timeScale == 0)
            {
                ResumeGame();
            }
            
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && (inputCounter < 3))
        {
            inputCounter++;
            if (inputCounter == 1)
            {
                RemoveFromActiveUI(MoveText);
            }
            
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
            UIyOffset -= rt.rect.height;
        }
    }

    public void RemoveFromActiveUI(GameObject objective){
        int index = ActiveUI.FindIndex(x => x.name == objective.name);
        if(index > -1){
            objective.SetActive(false);
            ActiveUI.RemoveAt(index);
            // get height of element just removed, adjust all current elements, add to current y offset
            float height = objective.transform.GetComponent<RectTransform>().rect.height;
            AdjustActiveUI(index, height);
            UIyOffset += height;
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

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        SetUIElements(false);
    }

    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        SetUIElements(true);
    }

    public void SetUIElements(bool status){
        foreach(GameObject elem in ActiveUI){
            elem.SetActive(status);
        }
    }
}
