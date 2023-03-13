using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineProfileManager : MonoBehaviour
{
    [SerializeField] VineProfle[] MonProfiles;
    [SerializeField] ProceduralIvy MonIvy;
    MonVineStateMachine MVSM;


    // Start is called before the first frame update
    void Start()
    {
        MVSM = GameObject.FindObjectOfType<MonVineStateMachine>();
    }

    public void SetProfile(float sliderVal)
    {
        int i = (int)sliderVal;
        MVSM.setSettings(MonProfiles[i]);
        MonIvy.setSettings(MonProfiles[i]);
    }
}
