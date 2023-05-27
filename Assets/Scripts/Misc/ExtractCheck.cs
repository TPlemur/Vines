using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractCheck : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(GameStateManager.ValidSplitjawPic){
            if(!MainMenuScript.goodEndingReached){
                MainMenuScript.endingsReached++;
                MainMenuScript.goodEndingReached = true;
            }
            SceneManager.LoadScene(2);
        }
    }
}
