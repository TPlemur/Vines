using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderNotifyer : MonoBehaviour
{
    [SerializeField] string validTag = "Player";
    public bool triggered = false;
    public Collider collision;

    public void EarlyUpdate()
    {
        triggered = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == validTag)
        {
            collision = other;
            triggered = true;
        }
        else { triggered = false; }
    }
}
