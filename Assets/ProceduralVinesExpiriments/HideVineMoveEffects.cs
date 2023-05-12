using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideVineMoveEffects : MonoBehaviour
{
    [SerializeField] GameObject toggledObjs;
    [SerializeField] ValveInteractable valve;
    [SerializeField] GameObject XMBranches;
    [SerializeField] GameObject XPBranches;
    [SerializeField] Vector3 XMacceleration = new Vector3(-5, 0, 0);
    [SerializeField] Vector3 XPacceleration = new Vector3(5, 0, 0);
    [SerializeField] float newMaxDist = 1;
    [SerializeField] float driftTime = 6;
    [SerializeField] float activDist = 5;
    [SerializeField] float witherTime = 1;
    [SerializeField] Color livingTip;
    [SerializeField] Color livingBase;
    List<Material> branchMats = new List<Material>();

    Cloth[] XMcloth;
    Cloth[] XPcloth;
    float timer = 0;
    float oldMaxDist;
    Vector3 oldXMacceleration = new Vector3(-5, 0, 0);
    Vector3 oldXPacceleration = new Vector3(5, 0, 0);

    GameObject player;
    bool isDrift = false;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        StartCoroutine(lateStart());
    }

    //delay start functionality to allow vines to load first
    IEnumerator lateStart()
    {
        yield return new WaitForSeconds(0.1f);
        XMcloth = XMBranches.GetComponentsInChildren<Cloth>();
        XPcloth = XPBranches.GetComponentsInChildren<Cloth>();
        List<SkinnedMeshRenderer> mr = new List<SkinnedMeshRenderer>();
        mr.AddRange(XMBranches.GetComponentsInChildren<SkinnedMeshRenderer>());
        mr.AddRange(XPBranches.GetComponentsInChildren<SkinnedMeshRenderer>());
        Debug.Log(mr.Count);
        foreach(SkinnedMeshRenderer m in mr) { branchMats.Add(m.material); }

        oldMaxDist = XMcloth[0].gameObject.GetComponent<Branch>().maxMove;
        isDrift = true;
        toggledObjs.SetActive(false);
    }

    //interpolates drift acording to currnet state of timer
    void driftInterpolate()
    {
        Vector3 currentXMDrift = new Vector3(Mathf.Lerp(oldXMacceleration.x, XMacceleration.x, timer / driftTime), 
                                             Mathf.Lerp(oldXMacceleration.y, XMacceleration.y, timer / driftTime), 
                                             Mathf.Lerp(oldXMacceleration.z, XMacceleration.z, timer / driftTime));

        Vector3 currentXPDrift = new Vector3(Mathf.Lerp(oldXPacceleration.x, XPacceleration.x, timer / driftTime),
                                             Mathf.Lerp(oldXPacceleration.y, XPacceleration.y, timer / driftTime),
                                             Mathf.Lerp(oldXPacceleration.z, XPacceleration.z, timer / driftTime));

        currentXMDrift = transform.localToWorldMatrix * currentXMDrift;
        currentXPDrift = transform.localToWorldMatrix * currentXPDrift;

        float skinCoef = Mathf.Lerp(oldMaxDist, newMaxDist, timer / driftTime);
        ClothSkinningCoefficient[] skinns = XMcloth[0].coefficients;
        for (int i = 0; i < skinns.Length; i++)
        {
            skinns[i].maxDistance = skinCoef;
        }
        skinns[0].maxDistance = 0.0f;
        setCloth(XMcloth, currentXMDrift, skinns);
        setCloth(XPcloth, currentXPDrift, skinns);
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

    IEnumerator witherBranchs()
    {
        timer = 0;
        while (timer < witherTime)
        {
            foreach (Material b in branchMats)
            {
                Debug.Log("MatFound");
                b.SetColor("_TipTint", Color.Lerp(livingTip, new Color(0, 0, 0), timer / witherTime));
                b.SetColor("_BaseTint", Color.Lerp(livingBase, new Color(0, 0, 0), timer / witherTime));
                b.SetFloat("_Amount", Mathf.Lerp(0.91f, 0.5f, timer / witherTime));
            }
            timer += Time.deltaTime;
            yield return 0;
        }
    }

    IEnumerator GrowBranchs()
    {
        timer = 0;
        while (timer < witherTime)
        {
            foreach (Material b in branchMats)
            {
                b.SetColor("_TipTint", Color.Lerp(new Color(0, 0, 0), livingTip, timer / witherTime));
                b.SetColor("_BaseTint", Color.Lerp(new Color(0, 0, 0), livingBase, timer / witherTime));
                b.SetFloat("_Amount", Mathf.Lerp(0.5f, 0.91f, timer / witherTime));
            }
            timer += Time.deltaTime;
            yield return 0;
        }
    }

    public void witherVines()
    {
        StartCoroutine(witherBranchs());
        toggledObjs.SetActive(true);
    }

    public void growVine()
    {
        StartCoroutine(GrowBranchs());
        toggledObjs.SetActive(false);
        valve.resetValve();
    }

    // Update is called once per frame
    void Update()
    {
        //start drift on player proximity
        if ((player.transform.position - transform.position).magnitude < activDist)
        {
            if (timer<driftTime)
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
            if (timer>0)
            {
                timer -= Time.deltaTime;
            }
        }
        if (isDrift)
        {
            driftInterpolate();
        }

    }
}
