using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractCheck : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        // get player script and inventory
        PlayerItemsAndInventory script = col.transform.parent.gameObject.GetComponent<PlayerItemsAndInventory>();
        if(script != null){
            List<Item> playerItems = script.inventory.items;
            // find item of type PVTM if the player has it then they can extract
            // .Find() takes a lambda function and performs it on every element in the list
            var item = playerItems.Find(x => x.GetType() == typeof(PVTM));
            if(item != null){
                Debug.Log("PLAYER EXTRACTING WITH PVTM");
                //go to winning ending the scene
                SceneManager.LoadScene(2);
            }
        }
    }
}
