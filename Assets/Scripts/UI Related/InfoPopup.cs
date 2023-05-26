using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
    [SerializeField] PauseMenu pm;
    [SerializeField] float timeForPrompt = 3;
    [SerializeField] GameObject toggledObjs;
    PauseMenu.techPage page;
    float inverseTime;
    float timer;
    Slider slider;
    
    public void wakeUp(PauseMenu.techPage NewPage)
    {
        toggledObjs.SetActive(true);
        timer = timeForPrompt;
        slider.value = 1;
        page = NewPage;
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        slider.value = timer * inverseTime;
        if(timer <= 0)
        {
            toggledObjs.SetActive(false);
        }
        if (Input.GetKeyDown(KeyMapper.techNotes)) { pm.openToEquip(page);}

    }

    private void Start()
    {
        slider = GetComponent<Slider>();
        inverseTime = 1 / timeForPrompt;
        toggledObjs.SetActive(false);
        page = PauseMenu.techPage.EE;
    }
}
