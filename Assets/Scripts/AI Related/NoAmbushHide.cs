using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAmbushHide : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Brain.isHiding = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Brain.isHiding = false;
        }
    }
}
