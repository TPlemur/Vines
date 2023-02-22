using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempLightSwitch : MonoBehaviour
{
    [SerializeField]
    public GameObject room1;
    public GameObject room2;
    public GameObject room3;
    public GameObject room4;

    public void TurnOnLights(){
        Debug.Log("TURNING ON LIGHTS");
        Transform lights = room1.transform.Find("Lights");
        if(lights != null){
            Debug.Log("LIGHT 1");
            lights.gameObject.SetActive(true);
        }
        lights = room2.transform.Find("Lights");
        if(lights == null){
            Debug.Log("BRUH");
        }
        if(lights != null){
            Debug.Log("LIGHT 2");
            lights.gameObject.SetActive(true);
        }
        lights = room3.transform.Find("Lights");
        if(lights != null){
            Debug.Log("LIGHT 3");
            lights.gameObject.SetActive(true);
        }
        lights = room4.transform.Find("Lights");
        if(lights != null){
            lights.gameObject.SetActive(true);
        }
    }
}
