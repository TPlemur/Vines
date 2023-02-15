using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVTMglow : MonoBehaviour
{
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            mat.EnableKeyword("_EMISSION");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            mat.DisableKeyword("_EMISSION");
        }
    }
}
