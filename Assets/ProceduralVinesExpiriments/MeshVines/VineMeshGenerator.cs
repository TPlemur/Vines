using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages meshVines
/// 
/// call makeSurface() to generate the grid of vine locations
/// needs to be attached to a gameobject at the <-x,+y,-z> corner of the map
/// 
/// </summary>

public class VineMeshGenerator : MonoBehaviour
{
    /// 
    /// PARAMATERS
    /// 
    [SerializeField] bool DEBUGon = false;
    [SerializeField] GameObject DebugDots;

    [SerializeField] GameObject terrian;
    [SerializeField] float range = 5;
    [SerializeField] float resolution = 1f;
    [SerializeField] float depth = 4;

    [SerializeField] float fuzzQty = 0.1f;

    //graph node data struct
    public struct gridDot
    {
        public Vector3 pos;
        public Vector3 norm;
        public int[] indexlocation;
        public List<int[]> connections;
        public GameObject Ddot;
        public bool valid;
    }

    /// 
    /// internal data
    /// 
    int gridWidth;
    int gridDepth;
    gridDot[,,] cubeMatrix;
    List<gridDot> cullList;
    Collider[] terainColiders;
    gridDot VaildPoint;
    float invRes;

    /// 
    /// Public functions 
    ///

    //call to initialize cubeMatrix and validPoint to surface of map
    public void makeSurface()
    {
        gridWidth = (int)(range / resolution);
        gridDepth = (int)(depth / resolution);
        invRes = 1 / resolution;
        terainColiders = terrian.GetComponentsInChildren<Collider>();
        makeCube();
        cullCube();
        if (DEBUGon)
        {
            foreach(gridDot d in cubeMatrix)
            {
                if (d.valid)
                {
                    Debug.Log(d.connections.Count);
                    GameObject dot = Instantiate(DebugDots);
                    dot.transform.position = d.pos;
                    dot.transform.parent = transform;
                }
            }
        }
    }

    //gives ivyNode list suitable for branch
    public List<IvyNode> makeList(gridDot start, Vector3 dir, int len)
    {
        List<IvyNode> nodes = new List<IvyNode>();
        nodes.Add(new IvyNode(start.pos, transform.eulerAngles));
        int ivyPTR = 1;
        gridDot headDot = start;
        gridDot lastDot = start;
        gridDot newDot = start;

        int firstDir = (int)Random.Range(0, start.connections.Count - 0.99f);

        headDot = cubeMatrix[start.connections[firstDir][0], start.connections[firstDir][1], start.connections[firstDir][2]];

        Vector3 lastDir = new Vector3(headDot.indexlocation[0] - lastDot.indexlocation[0], 
                                      headDot.indexlocation[1] - lastDot.indexlocation[1], 
                                      headDot.indexlocation[2] - lastDot.indexlocation[2]);
        while (ivyPTR < len)
        {
            List<gridDot> posibilities = new List<gridDot>();
            for (int i = 0; i < headDot.connections.Count; i++)
            {
                gridDot candidate = cubeMatrix[headDot.connections[i][0], headDot.connections[i][1], headDot.connections[i][2]];
                Vector3 nextDir = new Vector3(candidate.indexlocation[0] - headDot.indexlocation[0],
                                              candidate.indexlocation[1] - headDot.indexlocation[1],
                                              candidate.indexlocation[2] - headDot.indexlocation[2]);
                Vector3 dirDif = nextDir - lastDir;
                float dirDifSum = dirDif.x + dirDif.y + dirDif.z;
                if (Mathf.Abs(dirDifSum) < 2) {
                    posibilities.Add(candidate);
                }
            }
            if(posibilities.Count > 0) {newDot = posibilities[(int)Random.Range(0, posibilities.Count - 1)]; }
            else
            {
                int newid = (int)Random.Range(0, headDot.connections.Count - 0.99f);
                newDot = cubeMatrix[headDot.connections[newid][0], headDot.connections[newid][1], headDot.connections[newid][2]];
            }
            lastDot = headDot;
            headDot = newDot;

            Vector3 fuzz = new Vector3(Random.Range(-fuzzQty, fuzzQty), Random.Range(-fuzzQty, fuzzQty), Random.Range(-fuzzQty, fuzzQty));
            nodes.Add(new IvyNode(headDot.pos + fuzz, transform.eulerAngles));
            ivyPTR++;
        }

        return nodes;
    }

    //turns a list of dots into ivyNodes
    public List<IvyNode> makeList(List<gridDot> dots)
    {
        List<IvyNode> inodes = new List<IvyNode>();
        foreach(gridDot d in dots)
        {
            inodes.Add(new IvyNode(d.pos, transform.eulerAngles));
        }
        return inodes;
    }

    //Mades a list of gridDots suitable for a branch
    public List<gridDot> makeDotList(gridDot start, Vector3 dir, int len)
    {
        List<gridDot> nodes = new List<gridDot>();
        nodes.Add(start);
        int ivyPTR = 1;
        gridDot headDot = start;
        gridDot lastDot = start;
        gridDot newDot = start;

        int firstDir = (int)Random.Range(0, start.connections.Count - 0.99f);

        headDot = cubeMatrix[start.connections[firstDir][0], start.connections[firstDir][1], start.connections[firstDir][2]];
        nodes.Add(start);

        Vector3 lastDir = new Vector3(headDot.indexlocation[0] - lastDot.indexlocation[0],
                                      headDot.indexlocation[1] - lastDot.indexlocation[1],
                                      headDot.indexlocation[2] - lastDot.indexlocation[2]);
        while (ivyPTR < len)
        {
            List<gridDot> posibilities = new List<gridDot>();
            for (int i = 0; i < headDot.connections.Count; i++)
            {
                gridDot candidate = cubeMatrix[headDot.connections[i][0], headDot.connections[i][1], headDot.connections[i][2]];
                Vector3 nextDir = new Vector3(candidate.indexlocation[0] - headDot.indexlocation[0],
                                              candidate.indexlocation[1] - headDot.indexlocation[1],
                                              candidate.indexlocation[2] - headDot.indexlocation[2]);
                Vector3 dirDif = nextDir - lastDir;
                float dirDifSum = dirDif.x + dirDif.y + dirDif.z;
                if (Mathf.Abs(dirDifSum) < 2)
                {
                    posibilities.Add(candidate);
                }
            }
            if (posibilities.Count > 0) { newDot = posibilities[(int)Random.Range(0, posibilities.Count - 1)]; }
            else
            {
                int newid = (int)Random.Range(0, headDot.connections.Count - 0.99f);
                newDot = cubeMatrix[headDot.connections[newid][0], headDot.connections[newid][1], headDot.connections[newid][2]];
            }
            lastDot = headDot;
            headDot = newDot;

            nodes.Add(headDot);
            ivyPTR++;
        }

        return nodes;
    }

    //returns the nearest dot to a world position
    public gridDot vWorldToValid(Vector3 targetPos)
    {
        //convert location to grid co-ordinates
        float fx = (targetPos.x - transform.position.x) * invRes;
        float fz = (targetPos.z - transform.position.z) * invRes;

        //round to nearst int
        int x = (int)Mathf.Round(fx);
        int y = gridDepth - 1;
        int z = (int)Mathf.Round(fz);

        //ADD CHECK FOR OUT OF BOUNDS HERE

        //search vertically for valid dot
        while (y > 0)
        {
            if (cubeMatrix[x, y, z].valid)
            {
                return cubeMatrix[x, y, z];
            }
            y--;
        }
        //add H search if necessasary

        Debug.Log("Failed to find Grid Point");
        return new gridDot();
    }

    //Takes a list of dots, expands it, and removes the first elemet
    public void advanceBranch(ref List<gridDot> branchDots)
    {
        //pull the front two nodes
        gridDot headDot = branchDots[branchDots.Count - 1];
        gridDot lastDot = branchDots[branchDots.Count - 2];

        Vector3 lastDir = new Vector3(headDot.indexlocation[0] - lastDot.indexlocation[0],
                              headDot.indexlocation[1] - lastDot.indexlocation[1],
                              headDot.indexlocation[2] - lastDot.indexlocation[2]);

        List<gridDot> posibilities = new List<gridDot>();
        for (int i = 0; i < headDot.connections.Count; i++)
        {
            gridDot candidate = cubeMatrix[headDot.connections[i][0], headDot.connections[i][1], headDot.connections[i][2]];
            Vector3 nextDir = new Vector3(candidate.indexlocation[0] - headDot.indexlocation[0],
                                          candidate.indexlocation[1] - headDot.indexlocation[1],
                                          candidate.indexlocation[2] - headDot.indexlocation[2]);
            Vector3 dirDif = nextDir - lastDir;
            float dirDifSum = dirDif.x + dirDif.y + dirDif.z;

            if (Mathf.Abs(dirDifSum) < 2)
            {
                posibilities.Add(candidate);
            }
        }
        gridDot newDot;
        if (posibilities.Count > 0) { newDot = posibilities[(int)Random.Range(0, posibilities.Count)]; }
        else
        {
            int newid = (int)Random.Range(0, headDot.connections.Count);
            newDot = cubeMatrix[headDot.connections[newid][0], headDot.connections[newid][1], headDot.connections[newid][2]];
        }
        branchDots.Add(newDot);
        branchDots.RemoveAt(0);

    }

    //WIP WIP WIP WIP
    public void advanceTowards(ref List<gridDot> branchDots,Vector3 destination)
    {
        //pull the front two nodes
        gridDot headDot = branchDots[branchDots.Count - 1];
        gridDot lastDot = branchDots[branchDots.Count - 2];

        Vector3 lastDir = new Vector3(headDot.indexlocation[0] - lastDot.indexlocation[0],
                              headDot.indexlocation[1] - lastDot.indexlocation[1],
                              headDot.indexlocation[2] - lastDot.indexlocation[2]);

        List<gridDot> posibilities = new List<gridDot>();
        for (int i = 0; i < headDot.connections.Count; i++)
        {
            gridDot candidate = cubeMatrix[headDot.connections[i][0], headDot.connections[i][1], headDot.connections[i][2]];
            Vector3 nextDir = new Vector3(candidate.indexlocation[0] - headDot.indexlocation[0],
                                          candidate.indexlocation[1] - headDot.indexlocation[1],
                                          candidate.indexlocation[2] - headDot.indexlocation[2]);
            Vector3 dirDif = nextDir - lastDir;
            float dirDifSum = dirDif.x + dirDif.y + dirDif.z;

            if (Mathf.Abs(dirDifSum) < 2)
            {
                posibilities.Add(candidate);
            }
        }
        gridDot newDot;
        if (posibilities.Count > 0) { newDot = posibilities[(int)Random.Range(0, posibilities.Count)]; }
        else
        {
            int newid = (int)Random.Range(0, headDot.connections.Count);
            newDot = cubeMatrix[headDot.connections[newid][0], headDot.connections[newid][1], headDot.connections[newid][2]];
        }
        branchDots.Add(newDot);
        branchDots.RemoveAt(0);
    }

    /// 
    /// TEMP functs
    ///

    IEnumerator waitTHing()
    {
        yield return new WaitForSeconds(1);
        makeSurface();
        yield return new WaitForSeconds(1);
        foreach (gridDot d in cubeMatrix)
        {
            if (d.valid)
            {
                //addDot(d.indexlocation[0], d.indexlocation[1], d.indexlocation[2]);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //makeSurface();
        StartCoroutine(waitTHing());
    }


    ///
    /// Functions for the generation of the location graph
    /// 
    /// these functions are for internal use - to be called by makeSurface()
    /// it's unlikely that you need to call one
    /// 

    //Creates a cube of dots spaced evenly saves it to cubeMatrix
    void makeCube()
    {
        cubeMatrix = new gridDot[gridWidth, gridDepth, gridWidth];

        //create grid of dots
        for (int xi = 0; xi < gridWidth; xi++)
        {
            for (int zi = 0; zi < gridWidth; zi++)
            {
                for(int yi = 0;yi < gridDepth; yi++)
                {
                    cubeMatrix[xi, yi, zi] = new gridDot();
                    cubeMatrix[xi, yi, zi].valid = true;
                    cubeMatrix[xi, yi, zi].pos = transform.position + new Vector3(xi * resolution, -yi * resolution, zi * resolution);
                    cubeMatrix[xi, yi, zi].indexlocation = new int[] { xi, yi, zi };
                }
            }
        }

        //add neighbors to each dot
        for (int xi = 0; xi < gridWidth; xi++)
        {
            for (int zi = 0; zi < gridWidth; zi++)
            {
                for (int yi = 0; yi < gridDepth; yi++)
                {
                    addptrNeighbors(xi, yi, zi);
                }
            }
        }

    }

    //takes a grid location, and initializes neighborCons apropratly
    //Helper funct for makeCube
    void addptrNeighbors(int x, int y, int z)
    {
        List<int[]> neighborCons = new List<int[]>();

        //var is true if direction is valid
        bool mx = !(x == 0);
        bool px = !(x == gridWidth - 1);
        bool my = !(y == 0);
        bool py = !(y == gridDepth - 1);
        bool mz = !(z == 0);
        bool pz = !(z == gridWidth - 1);
        
        //this is bad, but I'm not sure if loops can cover it more elegantly
        if (mx)
        {
            if (my)
            {
                if (mz) { neighborCons.Add((int[])cubeMatrix[x - 1, y - 1, z - 1].indexlocation); }
                          neighborCons.Add((int[])cubeMatrix[x - 1, y - 1, z + 0].indexlocation);
                if (pz) { neighborCons.Add((int[])cubeMatrix[x - 1, y - 1, z + 1].indexlocation); }
            }
                if (mz) { neighborCons.Add((int[])cubeMatrix[x - 1, y + 0, z - 1].indexlocation); }
                          neighborCons.Add((int[])cubeMatrix[x - 1, y + 0, z + 0].indexlocation);
                if (pz) { neighborCons.Add((int[])cubeMatrix[x - 1, y + 0, z + 1].indexlocation); }
            if (py)
            {
                if (mz) { neighborCons.Add((int[])cubeMatrix[x - 1, y + 1, z - 1].indexlocation); }
                          neighborCons.Add((int[])cubeMatrix[x - 1, y + 1, z + 0].indexlocation);
                if (pz) { neighborCons.Add((int[])cubeMatrix[x - 1, y + 1, z + 1].indexlocation); }
            }
        }
        if (my)
        {
            if (mz) { neighborCons.Add((int[])cubeMatrix[x + 0, y - 1, z - 1].indexlocation); }
                      neighborCons.Add((int[])cubeMatrix[x + 0, y - 1, z + 0].indexlocation);
            if (pz) { neighborCons.Add((int[])cubeMatrix[x + 0, y - 1, z + 1].indexlocation); }
        }
            if (mz) { neighborCons.Add((int[])cubeMatrix[x + 0, y + 0, z - 1].indexlocation); }
                    //neighborCons.Add((int[])cubeMatrix[x + 0, y + 0, z + 0].indexlocation);
            if (pz) { neighborCons.Add((int[])cubeMatrix[x + 0, y + 0, z + 1].indexlocation); }
        if (py)
        {
            if (mz) { neighborCons.Add((int[])cubeMatrix[x + 0, y + 1, z - 1].indexlocation); }
                      neighborCons.Add((int[])cubeMatrix[x + 0, y + 1, z + 0].indexlocation);
            if (pz) { neighborCons.Add((int[])cubeMatrix[x + 0, y + 1, z + 1].indexlocation); }
        }
        if (px) {
            if (my)
            {
                if (mz) { neighborCons.Add((int[])cubeMatrix[x + 1, y - 1, z - 1].indexlocation); }
                          neighborCons.Add((int[])cubeMatrix[x + 1, y - 1, z + 0].indexlocation);
                if (pz) { neighborCons.Add((int[])cubeMatrix[x + 1, y - 1, z + 1].indexlocation); }
            }
                if (mz) { neighborCons.Add((int[])cubeMatrix[x + 1, y + 0, z - 1].indexlocation); }
                          neighborCons.Add((int[])cubeMatrix[x + 1, y + 0, z + 0].indexlocation);
                if (pz) { neighborCons.Add((int[])cubeMatrix[x + 1, y + 0, z + 1].indexlocation); }
            if (py)
            {
                if (mz) { neighborCons.Add((int[])cubeMatrix[x + 1, y + 1, z - 1].indexlocation); }
                          neighborCons.Add((int[])cubeMatrix[x + 1, y + 1, z + 0].indexlocation);
                if (pz) { neighborCons.Add((int[])cubeMatrix[x + 1, y + 1, z + 1].indexlocation); }
            }
        }
        cubeMatrix[x, y, z].connections = neighborCons;
    }

    //places dot at each vertex of cubeMatrix and culls terrain dots
    void cullCube()
    {
        for (int xi = 0; xi < gridWidth; xi++)
        {
            for (int zi = 0; zi < gridWidth; zi++)
            {
                for (int yi = 0; yi < gridDepth; yi++)
                {
                    culcollided(xi, yi, zi);

                }
            }
        }
        cullList = new List<gridDot>();
        //cull dots with too many connections (interior to the play area)
        for (int xi = 0; xi < gridWidth; xi++)
        {
            for (int zi = 0; zi < gridWidth; zi++)
            {
                for (int yi = 0; yi < gridDepth; yi++)
                {
                    //printDot(cubeMatrix[xi, yi, zi]);
                    if (cubeMatrix[xi, yi, zi].connections.Count == 26)
                    {
                        cullList.Add(cubeMatrix[xi, yi, zi]);
                    }
                }
            }
        }
        foreach (gridDot d in cullList)
        {
            cullNode(d.indexlocation[0], d.indexlocation[1], d.indexlocation[2]);
        }
    }

    //cull a node at a given grid location
    //Helper funct forcullcollided & cullCube
    void cullNode(int x, int y, int z)
    {
        cubeMatrix[x, y, z].valid = false;
        //remove all neighbors
        int[] coords = cubeMatrix[x, y, z].indexlocation;
        foreach (int[] index in cubeMatrix[x, y, z].connections)
        {
            cubeMatrix[index[0], index[1], index[2]].connections.Remove(coords);
        }
        cubeMatrix[x, y, z].connections.Clear();
    }

    //culls dots based on being inside terain 
    //Helper funct for cullCube
    //add logic to break wharehouse into individual rooms
    void culcollided(int x, int y, int z)
    {
        //check if dot is inside an object
        foreach (Collider c in terainColiders)
        {
            if (c.bounds.Contains(cubeMatrix[x, y, z].pos))
            {
                cullNode(x, y, z);
            }
        }
    }



    ///
    /// Debug utilities
    /// 

    //adds a debug dot
    void addDot(int x, int y, int z)
    {
        GameObject dot = Instantiate(DebugDots, transform);
        dot.transform.position = cubeMatrix[x, y, z].pos;
        cubeMatrix[x, y, z].Ddot = dot;
    }

    //prints info about a dot
    void printDot(gridDot output)
    {
        Debug.Log("Dotnum: " + output.indexlocation[0] + " " + output.indexlocation[1] + " " + output.indexlocation[2] + " Connections: " + output.connections.Count);
    }
}

