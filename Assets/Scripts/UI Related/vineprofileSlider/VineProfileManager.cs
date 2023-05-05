using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VineProfileManager : MonoBehaviour
{
    [SerializeField] VineProfle[] MonProfiles;
    [SerializeField] ProceduralIvy MonIvy;

    MonVineStateMachine MVSM;
    Slider slid;


    // grab the appropreate scripts and set the qual level to the stored state
    void Start()
    {
        MVSM = GameObject.FindObjectOfType<MonVineStateMachine>();
        slid = GetComponent<Slider>();
        slid.value = PlayerPrefs.GetFloat("vineQuality",2);
        SetProfile(PlayerPrefs.GetFloat("vineQuality", 2));
    }

    //pass the setting on to the approprate scripts
    public void SetProfile(float sliderVal)
    {
        int i = (int)sliderVal;
        MVSM.setSettings(MonProfiles[i]);
        MonIvy.setSettings(MonProfiles[i]);
        PlayerPrefs.SetFloat("vineQuality", sliderVal);
    }
}
