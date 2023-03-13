using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonVineStateMachine : MonoBehaviour
{
    public enum state
    {
        walk,
        charge,
        roar
    }
    public state currentState = state.walk;
    [SerializeField] ProceduralIvy staticIvyManager;

    [Header("Walk Characteristics")]
    [SerializeField] GameObject[] walkTargets;
    bool[] walkHasSpwned;
    [SerializeField] float zTransformBarriar = 0.001f;
    [SerializeField] float walkGrowSpeed = 2;
    [SerializeField] float walkvineDelay = 1;
    [SerializeField] float walkShrinkSpeed = 2;
    [SerializeField] float walkLowAngle = -15;
    [SerializeField] float walkHighAngle = 15;
    [SerializeField] int walkNumForward = 5;
    [SerializeField] int walkNumRadial = 1;

    [Header("Charge Characteristics")]
    [SerializeField] GameObject chargeTarget;
    [SerializeField] float chargeGrowSpeed = 0.5f;
    [SerializeField] float chargevineDelay = 0.5f;
    [SerializeField] float chargeShrinkSpeed = 0.5f;
    [SerializeField] float chargeLowAngle = -15;
    [SerializeField] float chargeHighAngle = 15;
    [SerializeField] int chargeNumForward = 5;
    [SerializeField] int chargeNumRadial = 1;
    [SerializeField] float chargeInterval = 0.1f;

    [Header("Roar Characteristics")]
    [SerializeField] GameObject[] roarTargets;
    [SerializeField] float roarGrowSpeed = 1.0f;
    [SerializeField] float roarvineDelay = 0.0f;
    [SerializeField] float roarShrinkSpeed = 1.0f;
    [SerializeField] int roarNum = 15;
    [SerializeField] float roarDuration = 6.208f;
    bool didRoar = false;

    float timer = 0;




    // Start is called before the first frame update
    void Start()
    {
        walkHasSpwned = new bool[walkTargets.Length];
        for (int i = 0; i < walkHasSpwned.Length; i++) { walkHasSpwned[i] = false; }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case state.walk:
                walk();
                didRoar = false;
                break;
            case state.charge:
                run();
                didRoar = false;
                break;
            case state.roar:
                if (!didRoar)
                {
                    StartCoroutine(roar());
                    didRoar = true;
                }
                break;
        }
    }

    void walk()
    {
        for (int i = 0; i < walkTargets.Length; i++)
        {
            if (walkTargets[i].transform.position.y < zTransformBarriar)
            {
                if (!walkHasSpwned[i])
                {
                    staticIvyManager.lowAngle = walkLowAngle;
                    staticIvyManager.highAngle = walkHighAngle;
                    staticIvyManager.branches = walkNumForward;
                    staticIvyManager.useTargetForAngle = true;
                    staticIvyManager.genVine(walkTargets[i], walkGrowSpeed, walkvineDelay, walkShrinkSpeed);


                    staticIvyManager.lowAngle = 0;
                    staticIvyManager.highAngle = 360;
                    staticIvyManager.branches = walkNumRadial;
                    staticIvyManager.useTargetForAngle = false;
                    staticIvyManager.genVine(walkTargets[i], walkGrowSpeed, walkvineDelay, walkShrinkSpeed);
                    walkHasSpwned[i] = true;

                }
            }
            else
            {
                walkHasSpwned[i] = false;
            }
        }
    }

    void run()
    {
        timer += Time.deltaTime;
        if (timer > chargeInterval)
        {
            staticIvyManager.lowAngle = chargeLowAngle;
            staticIvyManager.highAngle = chargeHighAngle;
            staticIvyManager.branches = chargeNumForward;
            staticIvyManager.useTargetForAngle = true;
            staticIvyManager.genVine(chargeTarget, chargeGrowSpeed, chargevineDelay, chargeShrinkSpeed);


            staticIvyManager.lowAngle = 0;
            staticIvyManager.highAngle = 360;
            staticIvyManager.branches = chargeNumRadial;
            staticIvyManager.useTargetForAngle = false;
            staticIvyManager.genVine(chargeTarget, chargeGrowSpeed, chargevineDelay, chargeShrinkSpeed);
            timer = 0;
        }
    }



    IEnumerator roar()
    {
        //clean up old vines
        //This moves the player around and I have no idea why
        /*
        Branch[] branches = staticIvyManager.gameObject.GetComponentsInChildren<Branch>();
        foreach (Branch b in branches)
        {
            b.shrinkSpeed = roarDuration;
            b.startSrhink();
        }
        */
        yield return new WaitForSeconds(roarDuration / 2);
        //spawn new vines
        staticIvyManager.lowAngle = 0;
        staticIvyManager.highAngle = 360;
        staticIvyManager.branches = roarNum;
        staticIvyManager.useTargetForAngle = false;
        foreach (GameObject tar in roarTargets)
        {
            staticIvyManager.genVine(tar, roarGrowSpeed, roarvineDelay, roarShrinkSpeed);
        }
    }

    public void setSettings(VineProfle vp)
    {
        walkNumForward = vp.walkNumForward;
        walkNumRadial = vp.walkNumRadial;

        chargeNumForward = vp.chargeNumForward;
        chargeNumRadial = vp.chargeNumRadial;
        chargeInterval = vp.chargeInterval;

        roarNum = vp.roarNum;
    }
}
