using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCheck : MonoBehaviour
{
    public static bool isMonsterInside = false;
    static GameObject monstah;
    static BoxCollider monContainer;
    private void Start()
    {
        monstah = GameObject.Find("Monster");
        monContainer = GetComponent<BoxCollider>();
    }

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

    static public void CheckMonsterContained()
    {
        if (monContainer.bounds.Contains(monstah.transform.position))
        {
            isMonsterInside = true;
        } else
        {
            isMonsterInside = false;
        }
    }

}
