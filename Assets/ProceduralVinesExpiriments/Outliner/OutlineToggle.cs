using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineToggle : MonoBehaviour
{
    [SerializeField] private Material outlineMat;
    [SerializeField] private MeshRenderer rend;
    [SerializeField] private bool separateMesh = false;
    private Material[] offMats;
    private Material[] onMats;
    bool on = false;

    //Turn on the outline
    public void On()
    {
        if (!on)
        {
            on = true;
            if (separateMesh)
            {
                rend.enabled = true;
            }
            else
            {
                rend.materials = onMats;
            }
        }
    }

    //turn off the outline
    public void Off()
    {
        if (on)
        {
            on = false;
            if (separateMesh)
            {
                rend.enabled = false;
            }
            else
            {
                rend.materials = offMats;
            }
        }
    }

    // create the approprate material arrays for on and off
    void Start()
    {
        offMats = rend.materials;
        onMats = rend.materials;
        onMats[onMats.Length - 1] = outlineMat;
    }
}
