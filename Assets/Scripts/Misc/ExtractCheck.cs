using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractCheck : MonoBehaviour
{
    void OnTriggerStay(Collider col){
        if(GameStateManager.ValidSplitjawPic && col.tag == "Player"){
            if(!MainMenuScript.goodEndingReached){
                MainMenuScript.endingsReached++;
                MainMenuScript.goodEndingReached = true;
            }
            SceneManager.LoadScene(2);
        }
    }
}
