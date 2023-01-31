using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col){
        PlayerItemsAndInventory script = col.transform.parent.gameObject.GetComponent<PlayerItemsAndInventory>();
        List<Item> playerItems =script.inventory.items;
        for(int i = 0; i < playerItems.Count; i++){
            if(playerItems[i].name == "PVTM"){
                Debug.Log("HERE");
            }
        }
    }
}
