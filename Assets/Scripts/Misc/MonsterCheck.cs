using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCheck : MonoBehaviour
{
    public static bool isMonsterInside = false;

    void OnTriggerEnter(Collider col){
        if(col.transform.tag == "Monster"){
            isMonsterInside = true;
        }
    }

    void OnTriggerExit(Collider col){
        if(col.transform.tag == "Monster"){
            isMonsterInside = false;
        }
    }

}
