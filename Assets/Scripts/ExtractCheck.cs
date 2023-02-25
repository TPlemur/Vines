using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractCheck : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.gameObject.GetComponent<PlayerItemsAndInventory>().validPic){
            Debug.Log("HERE");
            SceneManager.LoadScene(2);
        }
    }
}
