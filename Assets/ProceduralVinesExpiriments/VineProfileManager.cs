using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VineProfileManager : MonoBehaviour
{
    [SerializeField] VineProfle[] MonProfiles;
    [SerializeField] ProceduralIvy MonIvy;
    MonVineStateMachine MVSM;

    static int QualLevel = 2;
    Slider slid;


    // Start is called before the first frame update
    void Start()
    {
        MVSM = GameObject.FindObjectOfType<MonVineStateMachine>();
        slid = GetComponent<Slider>();
        slid.value = QualLevel;
        SetProfile((float)QualLevel);
    }

    public void SetProfile(float sliderVal)
    {
        int i = (int)sliderVal;
        MVSM.setSettings(MonProfiles[i]);
        MonIvy.setSettings(MonProfiles[i]);
        QualLevel = i;
    }
}
