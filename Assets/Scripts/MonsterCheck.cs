using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCheck : MonoBehaviour
{
    public static bool isMonsterInside = false;

    void OnTriggerEnter(Collider col){
        if(col.transform.tag == "Monster"){
            Debug.Log("BRUH");
            isMonsterInside = true;
        }
    }

    void OnTriggerExit(Collider col){
        if(col.transform.tag == "Monster"){
            Debug.Log("NOT BRUH");
            isMonsterInside = false;
        }
    }

}
