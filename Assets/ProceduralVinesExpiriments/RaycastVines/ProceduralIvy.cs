using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralIvy : MonoBehaviour
{
    [Header("Required")]
    public GameObject[] targetObjs;        //vines will spawn at the end of a cast in the -y direction
    public Material branchMaterial;        //Must be set to an approprate branch material
    [Space]
    [Header("Physical Characteristics")]
    public int branches = 3;               //number of branches spawnend in this cluster
    public int maxPointsForBranch = 60;    //the number of nodes spawned for this branch
    public float segmentLength = .15f;     //the length between each node "maxPointsForBranch" * "segmentLength" is the aprox length of the vine
    public float branchRadius = 0.04f;     //the approximate radius of the branch
    public LayerMask validSurfaces = ~0;   //determines what colliders vines will grow across

    [Space]
    [Header("Spawning Characteristics")]
    public float lowAngle = 0;              //minimum spawn angle
    public float highAngle = 360f;          //maximum spawn angle (clusters spawn evenly and randomly between these)
    public bool useTargetForAngle = false;  //determines if target.transform.forward is used as the initial angle
    [Space]
    public float branchDelay = 0.75f;       //time in seconds between clusters of vines if "continuousVines"
    public float branchGrowSpeed = 1;       //time in seconds between zero and fully grown
    public float branchShrinkSpeed = 1;     //time in seconds between full grown and fully shrunk
    public float initialDelay = 0;          //if "vinesAtStart" delay before vines start growing
    
    [Space]
    [Header("Branch Behavior")]
    [Space]
    public bool vinesAtStart = false;       //determines if vines will spanwn from the start function
    public bool continuousVines = true;     //determines if vines will continue to spawn from this
    public bool WitherBranch = true;        //determines if branches will shrink and be destroyed after growing
    public float timeAtGrown = 2;           //time in seconds spent at max size
    public bool canSense = false;           //if collideers should be spawned
    public float senseMultiplier = 1;       //collider size = "senseMultiplier" * "branchRadius"
    public string objTag = "Vine";          //object tag applied to each branch
    public bool wait = false;

    [Header("Cloth Settings")]
    public bool isCloth = false;            //determines if the branch should have cloth physics
    public float bendstiffness = 5;         //the stiffness coeficiant of the branch's cloth
    public float maxDist = 0.5f;            //the max travel distance of the branch's cloth
    public CapsuleCollider[] clothColliders;//colliders that interact with the branch's cloth


    int ivyCount = 0;
    float ivyTimer = 0;

    void Start()
    {
        //run vines once
        if (vinesAtStart)
        {
            StartCoroutine(genOnDelay());
        }
    }

    public void Update()
    {
        //create ivy clusters at regular (but dynamic) intervals
        ivyTimer += Time.deltaTime;
        if (continuousVines && ivyTimer >= branchDelay)
        {
            GenIvy();
            ivyTimer = 0;
        }
    }

    IEnumerator genOnDelay()
    {
        yield return initialDelay;
        GenIvy();
    }

    //GenVines calls used by MonVineStateMachine to generate vines at specific times
    //multy target
    public void genVines(GameObject[] targets, float growTime, float waitTime, float shrinkTime)
    {
        branchGrowSpeed = growTime;
        timeAtGrown = waitTime;
        branchShrinkSpeed = shrinkTime;
        targetObjs = targets;
        GenIvy();
    }
    //single target
    public void genVine(GameObject target, float growTime, float waitTime, float shrinkTime)
    {
        branchGrowSpeed = growTime;
        timeAtGrown = waitTime;
        branchShrinkSpeed = shrinkTime;
        targetObjs = new GameObject[1] { target };
        GenIvy();
    }

    //generates an ivy cluster at the location directly below the target transform
    public void GenIvy()
    {
        foreach (GameObject target in targetObjs)
        {
            Ray ray = new Ray(target.transform.position, -target.transform.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, validSurfaces))
            {
                createIvy(hit);
            }
        }
    }

    Vector3 findTangentFromArbitraryNormal(Vector3 normal)
    {
        if(useTargetForAngle)
        {
            return targetObjs[0].transform.forward;
        }
        Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
        Vector3 t2 = Vector3.Cross(normal, Vector3.up);
        if (t1.magnitude > t2.magnitude)
        {
            return t1;
        }
        return t2;
    }

    public void createIvy(RaycastHit hit)
    {
        Vector3 tangent = findTangentFromArbitraryNormal(hit.normal);
        GameObject ivy = new GameObject("Ivy " + ivyCount);
        ivy.transform.SetParent(transform);
        for (int i = 0; i < branches; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(highAngle / branches * i + Random.Range(lowAngle, highAngle / branches), hit.normal) * tangent;
            List<IvyNode> nodes = createBranch(maxPointsForBranch, hit.point, hit.normal, dir);
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
            b.wait = wait;

}

        ivyCount++;
    }


    Vector3 applyCorrection(Vector3 p, Vector3 normal)
    {
        return p + normal * 0.01f;
    }

    bool isOccluded(Vector3 from, Vector3 to)
    {
        Ray ray = new Ray(from, (to - from) / (to - from).magnitude);
        return Physics.Raycast(ray, (to - from).magnitude, validSurfaces);
    }

    bool isOccluded(Vector3 from, Vector3 to, Vector3 normal)
    {
        return isOccluded(applyCorrection(from, normal), applyCorrection(to, normal));
    }

    Vector3 calculateMiddlePoint(Vector3 p0, Vector3 p1, Vector3 normal)
    {
        Vector3 middle = (p0 + p1) / 2;
        var h = p0 - p1;
        var distance = h.magnitude;
        var dir = h / distance;
        return middle + normal * distance;
    }

    //creates a list of IvyNodes (a packaged position and normal) following the path of the branch
    List<IvyNode> createBranch(int count, Vector3 pos, Vector3 normal, Vector3 dir)
    {

        if (count == maxPointsForBranch)
        {
            IvyNode rootNode = new IvyNode(pos, normal);
            return new List<IvyNode> { rootNode }.join(createBranch(count - 1, pos, normal, dir));
        }
        else if (count < maxPointsForBranch && count > 0)
        {

            if (count % 2 == 0)
            {
                dir = Quaternion.AngleAxis(Random.Range(-20.0f, 20.0f), normal) * dir;
            }

            RaycastHit hit;
            Ray ray = new Ray(pos, normal);
            Vector3 p1 = pos + normal * segmentLength;

            if (Physics.Raycast(ray, out hit, segmentLength, validSurfaces))
            {
                p1 = hit.point;
            }
            ray = new Ray(p1, dir);

            if (Physics.Raycast(ray, out hit, segmentLength, validSurfaces))
            {
                Vector3 p2 = hit.point;
                IvyNode p2Node = new IvyNode(p2, -dir);
                return new List<IvyNode> { p2Node }.join(createBranch(count - 1, p2, -dir, normal));
            }
            else
            {
                Vector3 p2 = p1 + dir * segmentLength;
                ray = new Ray(applyCorrection(p2, normal), -normal);
                if (Physics.Raycast(ray, out hit, segmentLength, validSurfaces))
                {
                    Vector3 p3 = hit.point;
                    IvyNode p3Node = new IvyNode(p3, normal);

                    if (isOccluded(p3, pos, normal))
                    {
                        Vector3 middle = calculateMiddlePoint(p3, pos, (normal + dir) / 2);

                        Vector3 m0 = (pos + middle) / 2;
                        Vector3 m1 = (p3 + middle) / 2;

                        IvyNode m0Node = new IvyNode(m0, normal);
                        IvyNode m1Node = new IvyNode(m1, normal);

                        return new List<IvyNode> { m0Node, m1Node, p3Node }.join(createBranch(count - 3, p3, normal, dir));
                    }

                    return new List<IvyNode> { p3Node }.join(createBranch(count - 1, p3, normal, dir));
                }
                else
                {
                    Vector3 p3 = p2 - normal * segmentLength;
                    ray = new Ray(applyCorrection(p3, normal), -normal);

                    if (Physics.Raycast(ray, out hit, segmentLength, validSurfaces))
                    {
                        Vector3 p4 = hit.point;
                        IvyNode p4Node = new IvyNode(p4, normal);

                        if (isOccluded(p4, pos, normal))
                        {
                            Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);
                            Vector3 m0 = (pos + middle) / 2;
                            Vector3 m1 = (p4 + middle) / 2;

                            IvyNode m0Node = new IvyNode(m0, normal);
                            IvyNode m1Node = new IvyNode(m1, normal);

                            return new List<IvyNode> { m0Node, m1Node, p4Node }.join(createBranch(count - 3, p4, normal, dir));
                        }

                        return new List<IvyNode> { p4Node }.join(createBranch(count - 1, p4, normal, dir));
                    }
                    else
                    {
                        Vector3 p4 = p3 - normal * segmentLength;
                        IvyNode p4Node = new IvyNode(p4, dir);

                        if (isOccluded(p4, pos, normal))
                        {
                            Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);

                            Vector3 m0 = (pos + middle) / 2;
                            Vector3 m1 = (p4 + middle) / 2;

                            IvyNode m0Node = new IvyNode(m0, dir);
                            IvyNode m1Node = new IvyNode(m1, dir);

                            return new List<IvyNode> { m0Node, m1Node, p4Node }.join(createBranch(count - 3, p4, dir, -normal));
                        }
                        return new List<IvyNode> { p4Node }.join(createBranch(count - 1, p4, dir, -normal));
                    }
                }
            }

        }
        return null;
    }

    void combineAndClear()
    {
        MeshManager.instance.combineAll();
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }

    //set to approprate vine quality
    public void setSettings(VineProfle vp)
    {
        maxPointsForBranch = vp.maxPointsForBranch;
        segmentLength = vp.segmentLength;
        senseMultiplier = vp.senseMultiplier;
    }
}