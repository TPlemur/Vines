using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Holds a randomly generated doorcode and sets the attatched ui element
public class DoorCode : MonoBehaviour
{
    public static string code = "";
    public static Dictionary<int, int> camRes;
    Camera officeCam;

    void Start()
    {
        //gen door code and set to display
        code += Random.Range(0,9);
        code += Random.Range(0, 9);
        code += Random.Range(0, 9);
        code += Random.Range(0, 9);
        GetComponent<TMPro.TextMeshProUGUI>().text = "Door Code:\n" + code;

        //set cam resolution appropratly
        officeCam = GameObject.Find("OfficeCam").GetComponent<Camera>();
        camRes = new Dictionary<int, int>();
        camRes[1] = 2;
        camRes[2] = 6;
        camRes[3] = 11;
        camRes[4] = 15;
        camRes[5] = 18;
        officeCam.fieldOfView = camRes[(int)PlayerPrefs.GetFloat("psxQuality", 3)];
    }

}
