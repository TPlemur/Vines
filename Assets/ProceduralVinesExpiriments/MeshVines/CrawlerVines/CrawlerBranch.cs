using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerBranch : MonoBehaviour
{
 
    //Shader var names
    const string AMOUNTF = "_AmountFront";
    const string AMOUNTB = "_AmountBack";

    //shader var inputs
    [SerializeField] float crawlSpeed = 1;
    float MaxAmount = 0.827f;
    float amountPerNode;
    float currentAmountF = 0.827f;
    float currentAmountB = 0.827f;

    //mesh Properties
    Mesh mesh;
    [SerializeField] Material material;
    [SerializeField] int numNodes = 10;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    float branchRadius = 0.04f;
    int meshFaces = 3;


    //currently active nodes
    List<VineMeshGenerator.gridDot> branchDots;
    List<IvyNode> branchNodes;

    //tap into vmg
    [SerializeField] VineMeshGenerator vmg;

    
    

    private void Start()
    {
        StartCoroutine(waitTHing());
        amountPerNode = MaxAmount / numNodes;
        currentAmountF -= amountPerNode;
    }

    private void Update()
    {
        if (doUpdate)
        {
            timer += Time.deltaTime;
            if (timer > crawlSpeed)
            {
                advanceBranch();
                currentAmountF -= amountPerNode;
                currentAmountB += amountPerNode;
                material.SetFloat(AMOUNTF, currentAmountF);
                material.SetFloat(AMOUNTB, currentAmountB);
                timer -= crawlSpeed;
            }

            currentAmountF += amountPerNode * Time.deltaTime/crawlSpeed;
            currentAmountB -= amountPerNode * Time.deltaTime/crawlSpeed;
            material.SetFloat(AMOUNTF, currentAmountF);
            material.SetFloat(AMOUNTB, currentAmountB);
        }
    }

    ///
    /// TEMP FUNCT
    ///
    bool doUpdate = false;
    float timer = 0;
    public GameObject spawner;
    IEnumerator waitTHing()
    {
        yield return new WaitForSeconds(5);
        transform.position = Vector3.zero;
        branchDots = vmg.makeDotList(vmg.vWorldToValid(spawner.transform.position), transform.eulerAngles, numNodes);
        branchNodes = vmg.makeList(branchDots);
        setupMesh();
        doUpdate = true;
    }

    void advanceBranch()
    {
        vmg.advanceBranch(ref branchDots);
        branchNodes = vmg.makeList(branchDots);
        mesh = createMesh(branchNodes);
        meshFilter.mesh = mesh;
    }

    //creates mesh, meshFilter, and Render
    void setupMesh()
    {
        mesh = createMesh(branchNodes);
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        this.material = new Material(material);

        meshRenderer.material = material;
        if (mesh != null)
        {
            meshFilter.mesh = mesh;
        }


        material.SetFloat(AMOUNTF, currentAmountF);
        material.SetFloat(AMOUNTB, currentAmountB);
    }

    //create a mesh and run it
    Mesh createMesh(List<IvyNode> nodes)
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

    //helper function for createMesh
    float remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }

}