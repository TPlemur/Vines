using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshFindTest : MonoBehaviour
{
    [SerializeField] VineMeshGenerator vg;
    [SerializeField] Material reColor;

    [Header("Required")]
    public GameObject[] targetObjs;
    public Material branchMaterial;
    [Space]
    [Header("Physical Characteristics")]
    public int branches = 3;
    public int maxPointsForBranch = 60;
    public float segmentLength = .15f;
    public float branchRadius = 0.04f;
    public LayerMask validSurfaces = ~0;

    [Space]
    [Header("Spawning Characteristics")]
    public float lowAngle = 0;
    public float highAngle = 360f;
    public bool useTargetForAngle = false;
    [Space]
    public float branchDelay = 0.75f;
    public float branchGrowSpeed = 1;
    public float branchShrinkSpeed = 1;
    public float initialDelay = 0;

    [Space]
    [Header("Branch Behavior")]
    [Space]
    public bool vinesAtStart = false;
    public bool continuousVines = true;
    public bool WitherBranch = true;
    public float timeAtGrown = 2;
    public bool canSense = false;
    public float senseMultiplier = 1;
    public string objTag = "Vine";

    [Header("Cloth Settings")]
    public bool isCloth = false;
    public float bendstiffness = 5;
    public float maxDist = 0.5f;
    public CapsuleCollider[] clothColliders;
    [Space]
    public float recycleInterval = 30;


    int ivyCount = 0;
    float ivyTimer = 0;

    void createBranch()
    {
        GameObject ivy = new GameObject("Ivy " + ivyCount);
        ivy.transform.SetParent(transform);
        for (int i = 0; i < branches; i++)
        {
            List<IvyNode> nodes = vg.makeList(vg.vWorldToValid(transform.position),new Vector3(1,1,1),maxPointsForBranch );
            GameObject branch = new GameObject("Branch " + i);

            //set all the approprate branch settings
            Branch b;
            b = branch.AddComponent<Branch>();
            b.gameObject.layer = 14;
            b.init(nodes, branchRadius, branchMaterial, segmentLength, senseMultiplier);
            branch.transform.SetParent(ivy.transform);
            b.growthSpeed = branchGrowSpeed;
            b.shrinkSpeed = branchShrinkSpeed;
            b.shrink = WitherBranch;
            b.iscloth = isCloth;
            b.bendStiff = bendstiffness;
            b.maxMove = maxDist;
            b.isSense = canSense;
            b.delayTime = timeAtGrown;
            b.tag = objTag;
            b.clothColliders = clothColliders;

        }

        ivyCount++;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(genOnDelay());
    }
    IEnumerator genOnDelay()
    {
        yield return new WaitForSeconds(2);
        createBranch();
    }

    // Update is called once per frame
    void Update()
    {
        ivyTimer += Time.deltaTime;
        if(ivyTimer> branchDelay)
        {
            createBranch();
            ivyTimer -= branchDelay;
        }

    }

    
}
