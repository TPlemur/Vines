using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineToggle : MonoBehaviour
{
    [SerializeField] private Material outlineMat;
    [SerializeField]private MeshRenderer rend;
    private Material[] offMats;
    private Material[] onMats;
    bool on = false;

    //Turn on the outline
    public void On()
    {
        if (!on)
        {
            on = true;
            rend.materials = onMats;
        }
    }

    //turn off the outline
    public void Off()
    {
        if (on)
        {
            on = false;
            rend.materials = offMats;
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
