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
    public bool shrink = true;
    public float growthSpeed = 0.5f;
    public float shrinkSpeed = 0.3f;
    float currentAmount = 0;
    bool deAnimate = false;
    public float delayTime = 2;
    float delayTimer = 0;

    public bool iscloth = true;
    public float bendStiff = 5f;
    public float maxMove = 0.5f;
    public CapsuleCollider[] clothColliders;

    public bool isSense = false;

    SphereCollider[] colliders;
    float activeColliders = 0;
    float prevColliders = 0;
    float colliderSize = 1;
    float colliderFrequency = 1;

    public void init(List<IvyNode> branchNodes, float branchRadius, Material material, float segmentLength, float colliderSize)
    {
        this.branchNodes = branchNodes;
        this.branchRadius = branchRadius;
        this.material = new Material(material);
        mesh = createMesh(branchNodes);
        colliders = new SphereCollider[branchNodes.Count];
        //calculate spacing to minimize collider overlap
        this.colliderSize = colliderSize;
        while (segmentLength*colliderFrequency < colliderSize*branchRadius*2)
        {
            colliderFrequency++;
        }
    }

    //creates meshFilter and Render
    void setupMesh()
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
    }


    void Start()
    {

        setupMesh();

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
            cloth.capsuleColliders = clothColliders;
        }
        if (!isSense)
        {
            colliders = null;
        }
    }

    void Update()
    {
        //grow
        if (animate)
        {
            delayTimer += Time.deltaTime;
            currentAmount = (delayTimer*MAX) / growthSpeed;
            material.SetFloat(AMOUNT, currentAmount);

            if (currentAmount >= MAX)
            {
                animate = false;
                delayTime = 0;
            }
        }
        //wait
        else
        {
            if (delayTimer > delayTime && shrink)
            {
                shrink = false;
                deAnimate = true;
                delayTime = shrinkSpeed;
            }
            delayTimer += Time.deltaTime;
        }
        //shrink
        if(deAnimate)
        {
            delayTime -= Time.deltaTime * 2;
            currentAmount = delayTime * MAX / shrinkSpeed;
            material.SetFloat(AMOUNT, currentAmount);
            if(currentAmount <= -0.5)
            {
                Destroy(transform.parent.gameObject);
            }
        }
        //update colliders if necccasary
        if (isSense)
        {
            checkColliders();
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

    //update which colliderers should be active
    void checkColliders()
    {
        //adjust amount to a scale of 0 to 1
        float colliderAmount = currentAmount / MAX;
        //find perportion of coliders that should be visible
        activeColliders = branchNodes.Count * colliderAmount;

        if (activeColliders > prevColliders) //if growing
        {
            for (int i = (int)prevColliders; i < (int)activeColliders && i < branchNodes.Count; i++)
            {
                if (i % colliderFrequency == 0)//check spacing
                {
                    //add coliders as necessasary
                    colliders[i] = this.gameObject.AddComponent<SphereCollider>();
                    colliders[i].radius = branchRadius * colliderSize;
                    colliders[i].center = branchNodes[i].getPosition();
                    colliders[i].isTrigger = true;
                }
            }
        }
        else // if shrinking
        {
            for (int i = (int)prevColliders; i > (int)activeColliders && i < branchNodes.Count && i >= 0; i--)
            {
                //remove colliders as necessasary
                if (colliders[i] != null)
                {
                    Destroy(colliders[i]);
                }
            }
        }

        prevColliders = activeColliders;
    }
    
    //utility to allow shrinking of existing vines
    public void startSrhink()
    {
        animate = false;
        shrink = false;
        deAnimate = true;
        currentAmount = delayTime * shrinkSpeed / MAX;
    }

    public void Remove()
    {
        Destroy(transform.parent.gameObject);
    }
}