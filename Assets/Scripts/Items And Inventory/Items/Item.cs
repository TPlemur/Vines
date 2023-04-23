using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject itemObj;
    public Camera playerCam;
    public GameStateManager gameState;
    public GameObject ItemUI;

    public void LoadItem(string name){
        var loadedItem = Resources.Load("Prefabs/" + name);
        GameObject iPos = GameObject.Find("ItemPos");
        this.itemObj = (GameObject)Instantiate(loadedItem, iPos.transform.position, iPos.transform.rotation, iPos.transform);
        this.itemObj.SetActive(false);
    }

    public void Equip(){
        itemObj.SetActive(true);
        ItemUI.SetActive(true);
    }

    public void UnEquip(){
        itemObj.SetActive(false);
        ItemUI.SetActive(false);
    }

    public virtual void setup()
    {
        // overwritten
    }

    public virtual void Primary(){
        // overwritten
    }

    public virtual void Secondary(){
        // overwritten
    }

    public virtual void CycleRight(){
        // overwritten
    }

    public virtual void CycleLeft(){
        // overwritten
    }

    public virtual bool IsToggled(){
        // overwritten by PVTM and Shield
        return false;
    }

    // Raycast helper functions for shooting from 
    // different origin points, distances, and at layers
    public GameObject ShootRaycast(Camera obj, float dist, LayerMask layer){
        RaycastHit hit;
        if(Physics.Raycast(obj.transform.position, obj.transform.forward, out hit, dist, layer)){
            return hit.transform.gameObject;
        }
        return null;
    }

    public GameObject ShootRaycast(GameObject obj, float dist, LayerMask layer){
        RaycastHit hit;
        if(Physics.Raycast(obj.transform.position, obj.transform.forward, out hit, dist, layer)){
            return hit.transform.gameObject;
        }
        return null;
    }
}
