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
        foreach(Branch b in branches) { b.wait = false; }
    }

    /// <summary>
    /// use this to set the vines back to initial state
    /// </summary>
    public void resetVines()
    {
        foreach(Branch b in branches) { b.resetVine(); }
    }

    //Internal:
    Branch[] branches;

    //give the branches a set to spawn, then compile them to list
    void Start() { StartCoroutine(lateStart()); }
    IEnumerator lateStart()
    {
        yield return new WaitForSeconds(0.05f);
        branches = GetComponentsInChildren<Branch>();
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
