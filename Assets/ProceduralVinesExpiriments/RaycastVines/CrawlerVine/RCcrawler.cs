using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCcrawler : MonoBehaviour
{

    //Shader var names
    const string AMOUNTF = "_AmountFront";
    const string AMOUNTB = "_AmountBack";
    const float MaxAmount = 0.827f;

    //shader var inputs
    [SerializeField] float crawlSpeed = 1;
    float currentAmountF = 0.827f;
    float currentAmountB = 0.827f;
    LayerMask validSurfaces = ~0;
    float timer = 0;

    //mesh Properties
    [SerializeField] GameObject spawningObj;
    Mesh mesh;
    [SerializeField] Material material;
    [SerializeField] int numNodes = 10;
    [SerializeField] float segmentLength = .15f;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    float branchRadius = 0.04f;
    int meshFaces = 3;
    int randControl = 0;


    //Seek Propeties
    public bool isSeek = false;
    public GameObject seekTarget;
    [SerializeField] float SeakStrength = 0.5f;

    //currently active nodes
    List<CrawlerNode> branchNodes;

    private void Start()
    {
        amountPerNode = MaxAmount / numNodes;
        currentAmountF -= amountPerNode;

        transform.position = Vector3.zero;
        RaycastHit hit;
        Physics.Raycast(spawningObj.transform.position, -spawningObj.transform.up, out hit);
        branchNodes = createBranch(hit.point, hit.normal);
        init();
        doUpdate = true;
    }

    bool doUpdate = false;
    float amountPerNode;
    private void Update()
    {
        if (doUpdate)
        {
            timer += Time.deltaTime;
            if (timer > crawlSpeed)
            {
                addNode(branchNodes);
                branchNodes.RemoveAt(0);
                mesh = createMesh(branchNodes);
                meshFilter.mesh = mesh;
                currentAmountF -= amountPerNode;
                currentAmountB += amountPerNode;
                material.SetFloat(AMOUNTF, currentAmountF);
                material.SetFloat(AMOUNTB, currentAmountB);
                timer -= crawlSpeed;
            }

            currentAmountF += amountPerNode * Time.deltaTime / crawlSpeed;
            currentAmountB -= amountPerNode * Time.deltaTime / crawlSpeed;
            material.SetFloat(AMOUNTF, currentAmountF);
            material.SetFloat(AMOUNTB, currentAmountB);
        }
    }

    /// <summary>
    /// Initializes the mesh and associated graphics
    /// </summary>
    public void init()
    { 
        mesh = createMesh(branchNodes);
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        material = new Material(material);
        meshRenderer.material = material;
        meshFilter.mesh = mesh;

        material.SetFloat(AMOUNTF, currentAmountF);
        material.SetFloat(AMOUNTB, currentAmountB);
    }

    /// <summary>
    /// Creates a full branch at pos, with normal
    /// </summary>
    List<CrawlerNode> createBranch(Vector3 pos, Vector3 normal) {
        branchNodes = new List<CrawlerNode>();
        Vector3 tangent = findTangentFromArbitraryNormal(normal);
        CrawlerNode firstNode = new CrawlerNode(pos, normal, tangent, 0);
        branchNodes.Add(firstNode);
        for (int i = 0; i < numNodes; i++)
        {
            addNode(branchNodes);
        }
        return branchNodes;
    }

    //Helper function for createBranch
    Vector3 findTangentFromArbitraryNormal(Vector3 normal)
    {
        Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
        Vector3 t2 = Vector3.Cross(normal, Vector3.up);
        if (t1.magnitude > t2.magnitude)
        {
            return t1;
        }
        return t2;
    }

    /// <summary>
    /// Adds a node onto nodes while maintaining branch structure
    /// </summary>
    List<CrawlerNode> addNode(List<CrawlerNode> nodes)
    { 
        Vector3 pos = nodes[nodes.Count - 1].getPosition();
        Vector3 normal = nodes[nodes.Count - 1].getNormal();
        Vector3 dir = nodes[nodes.Count - 1].getDir();

        //Bias dir towards target if necessasary
        if (isSeek)
        {
            Vector3 dirToST = seekTarget.transform.position - pos;
            dirToST = dirToST - Vector3.Project(dirToST, normal);
            dirToST.Normalize();
            dir = dir + SeakStrength * dirToST;
            dir.Normalize();
        }

        //Add some randomness so it looks organic
        randControl += 1;
        if(randControl % 2 == 0)
        {
            dir = Quaternion.AngleAxis(Random.Range(-20.0f, 20.0f), normal) * dir;
        }

        RaycastHit hit;
        Ray ray = new Ray(pos, normal);

        //default p1 one segmentLength from pos in the direciton of normal
        Vector3 p1 = pos + normal * segmentLength;

        //cut p1 short if it hits any geometry
        if (Physics.Raycast(ray, out hit, segmentLength, validSurfaces))
        {
            p1 = hit.point;
        }

        //turn in the direction of dir and cast from p1
        ray = new Ray(p1, dir);

        //on hit there is a wall in front of the vine direction, so start growing up it
        if (Physics.Raycast(ray, out hit, segmentLength, validSurfaces))
        {
            Vector3 p2 = hit.point;
            CrawlerNode p2Node = new CrawlerNode(p2, -dir, normal, Mathf.Abs(Vector3.Magnitude(p2 - pos)));
            nodes.Add(p2Node);
            return nodes;
        }

        //no wall, so start curling back down in the direciton of -normal
        else
        {
            //set p2 to default length
            Vector3 p2 = p1 + dir * segmentLength;
            ray = new Ray(applyCorrection(p2, normal), -normal);
            //if wall found add node
            if (Physics.Raycast(ray, out hit, segmentLength, validSurfaces))
            {
                Vector3 p3 = hit.point;
                if (isOccluded(p3, pos, normal))
                {
                    Vector3 middle = calculateMiddlePoint(p3, pos, (normal + dir) / 2);
                    Vector3 m0 = (pos + middle) / 2;
                    CrawlerNode m0Node = new CrawlerNode(m0, normal, dir, Mathf.Abs(Vector3.Magnitude(m0 - pos)));
                    nodes.Add(m0Node);
                    return nodes;

                }
                else
                {
                    CrawlerNode p3Node = new CrawlerNode(p3, normal, dir, Mathf.Abs(Vector3.Magnitude(p3 - pos)));
                    nodes.Add(p3Node);
                    return nodes;
                }
            }
            //no wall, search a second time in the direction of - normal
            else
            {
                Vector3 p3 = p2 - normal * segmentLength;
                ray = new Ray(applyCorrection(p3, normal), -normal);

                //if wall found add node
                if (Physics.Raycast(ray, out hit, segmentLength, validSurfaces))
                {
                    Vector3 p4 = hit.point;
                    //if an object exists between point and start
                    if (isOccluded(p4, pos, normal))
                    {
                        Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);
                        Vector3 m0 = (pos + middle) / 2;
                        CrawlerNode m0Node = new CrawlerNode(m0, normal, dir, Mathf.Abs(Vector3.Magnitude(m0 - pos)));
                        nodes.Add(m0Node);
                        return nodes;
                    }
                    else
                    {
                        CrawlerNode p4Node = new CrawlerNode(p4, normal, dir, Mathf.Abs(Vector3.Magnitude(p4 - pos)));
                        nodes.Add(p4Node);
                        return nodes;
                    }
                }
                //cast back in the direction of - dir
                else
                {
                    Vector3 p4 = p3 - normal * segmentLength;


                    if (isOccluded(p4, pos, normal))
                    {
                        Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);
                        Vector3 m0 = (pos + middle) / 2;
                        CrawlerNode m0Node = new CrawlerNode(m0, normal, dir, Mathf.Abs(Vector3.Magnitude(m0 - pos)));
                        nodes.Add(m0Node);
                        return nodes;
                    }

                    CrawlerNode p4Node = new CrawlerNode(p4, dir, -normal, Mathf.Abs(Vector3.Magnitude(p4 - pos)));
                    nodes.Add(p4Node);
                    return nodes;
                }
            }
        }
    }

    //Helper function for addNode
    Vector3 applyCorrection(Vector3 p, Vector3 normal)
    {
        return p + normal * 0.01f;
    }
    //Helper function for addNode 
    bool isOccluded(Vector3 from, Vector3 to)
    {
        Ray ray = new Ray(from, (to - from) / (to - from).magnitude);
        return Physics.Raycast(ray, (to - from).magnitude, validSurfaces);
    }
    //Helper function for addNode
    bool isOccluded(Vector3 from, Vector3 to, Vector3 normal)
    {
        return isOccluded(applyCorrection(from, normal), applyCorrection(to, normal));
    }
    //Helper function for addNode
    Vector3 calculateMiddlePoint(Vector3 p0, Vector3 p1, Vector3 normal)
    {
        Vector3 middle = (p0 + p1) / 2;
        var h = p0 - p1;
        var distance = h.magnitude;
        var dir = h / distance;
        return middle + normal * distance;
    }

    /// <summary>
    /// Generate a mesh based on the list of nodes
    /// </summary>
    Mesh createMesh(List<CrawlerNode> nodes)
    {
        Mesh branchMesh = new Mesh();

        Vector3[] vertices = new Vector3[(nodes.Count) * meshFaces * 4];
        Vector3[] normals = new Vector3[nodes.Count * meshFaces * 4];
        Vector2[] uv = new Vector2[nodes.Count * meshFaces * 4];
        int[] triangles = new int[(nodes.Count - 1) * meshFaces * 6];

        for (int i = 0; i < nodes.Count; i++)
        {
            float vStep = (2f * Mathf.PI) / meshFaces;

            var fw = Vector3.zero;
            if (i > 0)
            {
                fw = branchNodes[i - 1].getPosition() - branchNodes[i].getPosition();
            }

            if (i < branchNodes.Count - 1)
            {
                fw += branchNodes[i].getPosition() - branchNodes[i + 1].getPosition();
            }

            if (fw == Vector3.zero)
            {
                fw = Vector3.forward;
            }

            fw.Normalize();

            var up = branchNodes[i].getNormal();
            up.Normalize();

            for (int v = 0; v < meshFaces; v++)
            {
                var orientation = Quaternion.LookRotation(fw, up);
                Vector3 xAxis = Vector3.up;
                Vector3 yAxis = Vector3.right;
                Vector3 pos = branchNodes[i].getPosition();
                pos += orientation * xAxis * (branchRadius * Mathf.Sin(v * vStep));
                pos += orientation * yAxis * (branchRadius * Mathf.Cos(v * vStep));

                vertices[i * meshFaces + v] = pos;

                var diff = pos - branchNodes[i].getPosition();
                normals[i * meshFaces + v] = diff / diff.magnitude;

                float uvID = remap(i, 0, nodes.Count - 1, 0, 1);
                uv[i * meshFaces + v] = new Vector2((float)v / meshFaces, uvID);
            }

            if (i + 1 < nodes.Count)
            {
                for (int v = 0; v < meshFaces; v++)
                {
                    triangles[i * meshFaces * 6 + v * 6] = ((v + 1) % meshFaces) + i * meshFaces;
                    triangles[i * meshFaces * 6 + v * 6 + 1] = triangles[i * meshFaces * 6 + v * 6 + 4] = v + i * meshFaces;
                    triangles[i * meshFaces * 6 + v * 6 + 2] = triangles[i * meshFaces * 6 + v * 6 + 3] = ((v + 1) % meshFaces + meshFaces) + i * meshFaces;
                    triangles[i * meshFaces * 6 + v * 6 + 5] = (meshFaces + v % meshFaces) + i * meshFaces;
                }
            }
        }

        branchMesh.vertices = vertices;
        branchMesh.triangles = triangles;
        branchMesh.normals = normals;
        branchMesh.uv = uv;
        return branchMesh;
    }

    //Helper function for CreateMesh
    float remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }
}