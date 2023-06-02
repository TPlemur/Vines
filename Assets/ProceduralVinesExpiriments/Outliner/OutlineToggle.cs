using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineToggle : MonoBehaviour
{
    [SerializeField] private Material outlineMat;
    [SerializeField] private MeshRenderer rend;
    [SerializeField] private bool separateMesh = false;
    [SerializeField] private SkinnedMeshRenderer smr;
    [SerializeField] private bool isSMR = false;
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
                if (isSMR) { smr.enabled = true; }
                else { rend.enabled = true; }
            }
            else
            {
                if (isSMR) { smr.materials = onMats; }
                else { rend.materials = onMats; }

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
                if (isSMR) { smr.enabled = false; }
                else { rend.enabled = false; }
            }
            else
            {
                if (isSMR) { smr.materials = offMats; }
                else { rend.materials = offMats; }
            }
        }
    }

    // create the approprate material arrays for on and off
    void Start()
    {
        if (isSMR)
        {
            offMats = smr.materials;
            onMats = smr.materials;
        }
        else
        {
            offMats = rend.materials;
            onMats = rend.materials;
        }
        onMats[onMats.Length - 1] = outlineMat;
    }
}
