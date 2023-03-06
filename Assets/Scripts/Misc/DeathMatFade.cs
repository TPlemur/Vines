using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMatFade : MonoBehaviour
{

    // Material bloodMat;
    Color32 newAlpha;
    Color32 bloodColor;

    void Start()
    {
        bloodColor = this.gameObject.GetComponent<Image>().material.color;
        newAlpha = bloodColor;
        newAlpha.a = 255;
        bloodColor = newAlpha;
        // bloodMat = GetComponent<Renderer>().material;
        // newAlpha = bloodMat.color;
        // newAlpha.a = 1f;
        // bloodMat.color = newAlpha;
    }

    void FixedUpdate()
    {
        // if (bloodMat.color.a > 0.01f){
        //     newAlpha = bloodMat.color;
        //     newAlpha.a -= .05f;
        //     bloodMat.color = newAlpha;
        // }
        // else{
        //     newAlpha = bloodMat.color;
        //     newAlpha.a = 0f;
        //     bloodMat.color = newAlpha;
        // }
        // Debug.Log(bloodMat.color.a);
        Debug.Log(bloodColor.a);
        if (bloodColor.a > 1){
            newAlpha = bloodColor;
            newAlpha.a -= 1;
            bloodColor = newAlpha;
        }
        else{
            newAlpha = bloodColor;
            newAlpha.a = 0;
            bloodColor = newAlpha;
        }
    }
}
