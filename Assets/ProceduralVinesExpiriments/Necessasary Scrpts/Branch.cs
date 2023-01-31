using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    const string AMOUNT = "_Amount";
    const string RADIUS = "_Radius";
    const float MAX = 0.91f;

    List<IvyNode> branchNodes;

    Mesh mesh;
    Material material;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    float branchRadius = 0.02f;
    int meshFaces = 3;

    bool animate;
    public float GrowMultiplyer = 1;
    public bool shrink = true;
    float growthSpeed = 0.5f;
    float shrinkSpeed = 0.3f;
    float currentAmount = 0;
    bool deAnimate = false;
    float delayTime = 2;
    float delayTimer = 0;

    public bool iscloth = true;
    public float bendStiff = 5f;
    public float maxMove = 0.5f;

    public void init(List<IvyNode> branchNodes, float branchRadius, Material material)
    {
        this.branchNodes = branchNodes;
        this.branchRadius = branchRadius;
        this.material = new Material(material);
        mesh = createMesh(branchNodes);
    }


    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (material == null)
        {
            material = new Material(Shader.Find("Specular"));
        }

        meshRenderer.material = material;
        if (mesh != null)
        {
            meshFilter.mesh = mesh;
        }

        material.SetFloat(RADIUS, branchRadius);
        material.SetFloat(AMOUNT, currentAmount);
        animate = true;
        if (iscloth)
        {
            Cloth cloth = gameObject.AddComponent(typeof(Cloth)) as Cloth;
            ClothSkinningCoefficient[] newConstraints;
            newConstraints = cloth.coefficients;
            for(int i = 0; i < newConstraints.Length; i++)
            {
                newConstraints[i].maxDistance = maxMove;
            }
            newConstraints[0].maxDistance = 0.0f;
            cloth.bendingStiffness = bendStiff;
            cloth.stretchingStiffness = 100;
            //cloth.damping = 0;
            cloth.coefficients = newConstraints;
        }
    }

    void Update()
    {
        if (animate)
        {
            currentAmount += Time.deltaTime * growthSpeed * GrowMultiplyer;
            material.SetFloat(AMOUNT, currentAmount);

            if (currentAmount >= MAX)
            {
                animate = false;
            }
        }
        else
        {
            if (delayTimer > delayTime && shrink)
            {
                deAnimate = true;
            }
            delayTimer += Time.deltaTime;
        }
        if(deAnimate)
        {
            currentAmount -= Time.deltaTime * shrinkSpeed * GrowMultiplyer;
            material.SetFloat(AMOUNT, currentAmount);
            if(currentAmount <= -0.5)
            {
                Destroy(transform.parent.gameObject);
            }
        }

    }
    

    float remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }

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

    /*
    void OnDrawGizmosSelected()
    {

        if (branchNodes != null)
        {
            for (int i = 0; i < branchNodes.Count; i++)
            {
                Gizmos.DrawSphere(branchNodes[i].getPosition(), .002f);
                Gizmos.color = Color.white;

                Gizmos.color = Color.blue;

                var fw = Vector3.zero;
                if (i > 0)
                {
                    fw = branchNodes[i - 1].getPosition() - branchNodes[i].getPosition();
                }

                if (i < branchNodes.Count - 1)
                {
                    fw += branchNodes[i].getPosition() - branchNodes[i + 1].getPosition();
                }

                fw.Normalize();

                var up = branchNodes[i].getNormal();
                up.Normalize();

                Vector3.OrthoNormalize(ref up, ref fw);

                float vStep = (2f * Mathf.PI) / meshFaces;
                for (int v = 0; v < meshFaces; v++)
                {

                    Gizmos.DrawLine(branchNodes[i].getPosition(), branchNodes[i].getPosition() + fw * .05f);

                    var orientation = Quaternion.LookRotation(fw, up);
                    Vector3 xAxis = Vector3.up;
                    Vector3 yAxis = Vector3.right;
                    Vector3 pos = branchNodes[i].getPosition();
                    pos += orientation * xAxis * (branchRadius * Mathf.Sin(v * vStep));
                    pos += orientation * yAxis * (branchRadius * Mathf.Cos(v * vStep));

                    Gizmos.color = new Color(
                        (float)v / meshFaces,
                        (float)v / meshFaces,
                        1f
                    );
                    Gizmos.DrawSphere(pos, .002f);
                }
            }
        }

    }
    */
}