using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamVines : MonoBehaviour
{
    /// <summary>
    /// use this to start the vines
    /// </summary>
    public void startVines()
    {
        gameObject.GetComponent<ProceduralIvy>().GenIvy();
    }


    //Debug
    /*
    public bool run = false;
    bool hasRun = false;

    private void Update()
    {
        if (run && !hasRun) { startVines(); hasRun = true; }
        if(hasRun && !run) { resetVines(); hasRun = false; }
    }
    */
}
