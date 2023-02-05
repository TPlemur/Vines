using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//quick function to attatch to the player to send the monster after the player on contact with senseVines
public class VineSense : MonoBehaviour
{
    public Enemy enemy;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Vine")
        {
            Debug.Log("seen");
            enemy.setTargetLocation(transform.position);
        }
    }
}
