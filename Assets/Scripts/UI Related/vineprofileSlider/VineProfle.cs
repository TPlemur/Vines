using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//big package of data to overwrite vine settings
public class VineProfle : MonoBehaviour
{ 

    [Header("Walk Characteristics")]
    public int walkNumForward = 5;
    public int walkNumRadial = 1;

    [Header("Charge Characteristics")]
    public int chargeNumForward = 5;
    public int chargeNumRadial = 1;
    public float chargeInterval = 0.1f;

    [Header("Roar Characteristics")]
    public int roarNum = 15;

    //ivyCharateristics
    public int maxPointsForBranch = 60;
    public float segmentLength = .15f;
    public float senseMultiplier = 1;

}
