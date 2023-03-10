using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideVineMoveEffects : MonoBehaviour
{
    //call to make vines drift outward
    public void startDrift()
    {
        StartCoroutine(drift(Vector3.zero, XMacceleration, Vector3.zero, XPacceleration, oldMaxDist, newMaxDist));
    }

    //call to make vines drift back to origional settings
    public void stopDrift()
    {
        StartCoroutine(drift(XMacceleration, Vector3.zero, XPacceleration, Vector3.zero, newMaxDist, oldMaxDist));
    }


    [SerializeField] GameObject XMBranches;
    [SerializeField] GameObject XPBranches;
    [SerializeField] Vector3 XMacceleration = new Vector3(-5, 0, 0);
    [SerializeField] Vector3 XPacceleration = new Vector3(5, 0, 0);
    [SerializeField] float newMaxDist = 1;
    [SerializeField] float driftTime = 6;

    Cloth[] XMcloth;
    Cloth[] XPcloth;
    float timer = 0;
    float oldMaxDist;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(lateStart());
    }

    //delay start functionality to allow vines to load first
    IEnumerator lateStart()
    {
        yield return new WaitForSeconds(0.1f);
        XMcloth = XMBranches.GetComponentsInChildren<Cloth>();
        XPcloth = XPBranches.GetComponentsInChildren<Cloth>();

        oldMaxDist = XMcloth[0].gameObject.GetComponent<Branch>().maxMove;
    }

    //lerps drift and max movement between values
    IEnumerator drift(Vector3 oldMdrift, Vector3 newMdrift, Vector3 oldPdrift, Vector3 newPdrift, float oldMax, float newMax)
    {
        timer = 0;
        while (timer < driftTime)
        {
            timer += Time.deltaTime;
            Vector3 tempXMaccel = Vector3.Lerp(oldMdrift, newMdrift, timer / driftTime);
            Vector3 tempXPaccel = Vector3.Lerp(oldPdrift, newPdrift, timer / driftTime);
            float skinn = Mathf.Lerp(oldMax, newMax, timer / driftTime);
            ClothSkinningCoefficient[] skinns = XMcloth[0].coefficients;
            for (int i = 0; i < skinns.Length; i++)
            {
                skinns[i].maxDistance = skinn;
            }
            skinns[0].maxDistance = 0.0f;
            setCloth(XMcloth, tempXMaccel, skinns);
            setCloth(XPcloth, tempXPaccel, skinns);
            yield return null;
        }
    }

    //helper function to set all cloth obj parameter
    void setCloth(Cloth[] cl, Vector3 accel, ClothSkinningCoefficient[] cs)
    {
        foreach (Cloth c in cl)
        {
            c.externalAcceleration = accel;
            c.coefficients = cs;
        }
    }


    //Debug manual toggle
    /*
    [SerializeField] bool doDrift = false;
    bool isDrift = false;

    // Update is called once per frame
    void Update()
    {
        if (doDrift && !isDrift)
        {
            startDrift();
            isDrift = true;
        }
        if (!doDrift && isDrift)
        {
            stopDrift();
            isDrift = false;
        }
    }
    */
}
