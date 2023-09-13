using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBox : MonoBehaviour
{
    [SerializeField] string targetTag;
    [SerializeField] UnityEvent triggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == targetTag)
        {
            triggered.Invoke();
        }
    }
}
