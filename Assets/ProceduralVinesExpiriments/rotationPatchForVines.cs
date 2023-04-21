using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//utility that rotates a transform to point to a target transform
public class rotationPatchForVines : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 rotation = new Vector3(0, 90, 0);
    private void Update()
    {
        transform.LookAt(target.position + rotation);
    }
}
