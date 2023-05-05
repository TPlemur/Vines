using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
    [SerializeField]PauseMenu pm;
    [SerializeField] float timeForPrompt = 3;
    PauseMenu.techPage page;
    float inverseTime;
    float timer;
    Slider slider;
    
    public void wakeUp(PauseMenu.techPage NewPage)
    {
        gameObject.SetActive(true);
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
            gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.T)) { pm.openToEquip(page);}

    }

    private void Start()
    {
        slider = GetComponent<Slider>();
        inverseTime = 1 / timeForPrompt;
        gameObject.SetActive(false);
    }
}
