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
    [SerializeField] LayerMask validCullColliders;

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
    public List<IvyNode> makeList(List<gridDot> dots,float fuzz)
    {
        List<IvyNode> inodes = new List<IvyNode>();
        
        foreach(gridDot d in dots)
        {
            Vector3 fuzzVec = new Vector3(Random.Range(-fuzz, fuzz), Random.Range(-fuzz, fuzz), Random.Range(-fuzz, fuzz));
            inodes.Add(new IvyNode(d.pos + fuzzVec, transform.eulerAngles));
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
    public void advanceBranch(ref List<gridDot> branchDots, ref int strCount, int strFactor)
    {
        //pull the front two nodes
        gridDot headDot = branchDots[branchDots.Count - 1];
        gridDot lastDot = branchDots[branchDots.Count - 2];

        //calculate the last position relitive to the current position
        Vector3 lastDir = new Vector3(lastDot.indexlocation[0] - headDot.indexlocation[0],
                              lastDot.indexlocation[1] - headDot.indexlocation[1],
                              lastDot.indexlocation[2] - headDot.indexlocation[2]);
        //index strCount
        strCount++;

        //continue straight if availible and strCount is true
        if (cubeMatrix[headDot.indexlocation[0] - (int)lastDir.x, headDot.indexlocation[1] - (int)lastDir.y,headDot.indexlocation[2] - (int)lastDir.z].valid
            && strCount % strFactor == 0)
        {
            branchDots.Add(cubeMatrix[headDot.indexlocation[0] - (int)lastDir.x, headDot.indexlocation[1] - (int)lastDir.y, headDot.indexlocation[2] - (int)lastDir.z]);
            branchDots.RemoveAt(0);
            return;
        }

        //add +- 1 to each x,y,z of -lastDir and choose a valid one at random
        List<int> xdirs = new List<int>();
        if (-lastDir.x == 0) { xdirs.Add(1); xdirs.Add(0); xdirs.Add(-1); }
        else { xdirs.Add((int)-lastDir.x); xdirs.Add(0); }

        List<int> ydirs = new List<int>();
        if (-lastDir.y == 0) { ydirs.Add(1); ydirs.Add(0); ydirs.Add(-1); }
        else { ydirs.Add((int)-lastDir.y); ydirs.Add(0); }

        List<int> zdirs = new List<int>();
        if (-lastDir.z == 0) { zdirs.Add(1); zdirs.Add(0); zdirs.Add(-1); }
        else { zdirs.Add((int)-lastDir.z); zdirs.Add(0); }

        List<gridDot> posibilities = new List<gridDot>();

        foreach(int xi in xdirs)
        {
            foreach(int yi in ydirs)
            {
                foreach(int zi in zdirs)
                {
                    if(cubeMatrix[xi+headDot.indexlocation[0], yi + headDot.indexlocation[1], zi + headDot.indexlocation[2]].valid && (xi!=0||yi!=0||zi!=0))
                    {
                        posibilities.Add(cubeMatrix[xi + headDot.indexlocation[0], yi + headDot.indexlocation[1], zi + headDot.indexlocation[2]]);
                    }
                }
            }
        }

        //choose a random dot
        gridDot newDot;
        if (posibilities.Count > 0) { newDot = posibilities[(int)Random.Range(0, posibilities.Count)];}
        //choose a random connection if no possibilities are found
        else
        {
            int newid = (int)Random.Range(0, headDot.connections.Count);
            newDot = cubeMatrix[headDot.connections[newid][0], headDot.connections[newid][1], headDot.connections[newid][2]];
        }
        branchDots.Add(newDot);
        branchDots.RemoveAt(0);

    }

    //WIP WIP WIP WIP
    public void advanceTowards(ref List<gridDot> branchDots,Vector3 destination, ref int strCount, int strFactor)
    {
        //pull the front two nodes
        gridDot headDot = branchDots[branchDots.Count - 1];
        gridDot lastDot = branchDots[branchDots.Count - 2];

        //calculate the last position relitive to the current position
        Vector3 lastDir = new Vector3(lastDot.indexlocation[0] - headDot.indexlocation[0],
                              lastDot.indexlocation[1] - headDot.indexlocation[1],
                              lastDot.indexlocation[2] - headDot.indexlocation[2]);
        //index strCount
        strCount++;

        //continue straight if availible and strCount is true
        if (cubeMatrix[headDot.indexlocation[0] - (int)lastDir.x, headDot.indexlocation[1] - (int)lastDir.y, headDot.indexlocation[2] - (int)lastDir.z].valid
            && strCount % strFactor == 0)
        {
            branchDots.Add(cubeMatrix[headDot.indexlocation[0] - (int)lastDir.x, headDot.indexlocation[1] - (int)lastDir.y, headDot.indexlocation[2] - (int)lastDir.z]);
            branchDots.RemoveAt(0);
            Debug.Log("straight Vine");
            return;
        }

        //find the most alligned dot
        Vector3 dirToTarget = destination - headDot.pos;
        gridDot newDot = cubeMatrix[headDot.connections[0][0], headDot.connections[0][1],headDot.connections[0][2]];
        Vector3 ndDir = dotToDir(headDot, newDot);
        float nAng = Vector3.Angle(dirToTarget, ndDir);
        gridDot compDot;
        Vector3 compDir;
        float compAng;
        foreach(int[] i in headDot.connections)
        {
            compDot = cubeMatrix[i[0], i[1], i[2]];
            compDir = dotToDir(headDot, compDot);
            compAng = Vector3.Angle(dirToTarget, compDir);
            if (compAng < nAng)
            {
                newDot = compDot;
                nAng = compAng;
            }
        }
        branchDots.Add(newDot);
        branchDots.RemoveAt(0);


    }

    //helper funct for advancetowards
    //returns the direciton vector from center to connection
    Vector3 dotToDir(gridDot center, gridDot connection)
    {
        return new Vector3(center.indexlocation[0] - connection.indexlocation[0],
                           center.indexlocation[1] - connection.indexlocation[1],
                           center.indexlocation[2] - connection.indexlocation[2]);
    }

    /// 
    /// TEMP functs
    ///

    IEnumerator waitTHing()
    {
        yield return new WaitForSeconds(1);
        makeSurface();
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
        CFcull();
        cullList = new List<gridDot>();
        //cull dots with too many connections (interior to the play area)
        int xi = 0; int yi = 0; int zi = 0;
        for (xi = 0; xi < gridWidth; xi++)
        {
            for (zi = 0; zi < gridWidth; zi++)
            {
                for (yi = 0; yi < gridDepth; yi++)
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
        //cull edges to prevent edge cases later
        xi = 0;
        for (zi = 0; zi < gridWidth; zi++)
        {
            for (yi = 0; yi < gridDepth; yi++)
            {
                cullNode(xi, yi, zi);
            }
        }
        xi = gridWidth-1;
        for (zi = 0; zi < gridWidth; zi++)
        {
            for (yi = 0; yi < gridDepth; yi++)
            {
                cullNode(xi, yi, zi);
            }
        }

        yi = 0;
        for (zi = 0; zi < gridWidth; zi++)
        {
            for (xi = 0; xi < gridWidth; xi++)
            {
                cullNode(xi, yi, zi);
            }
        }
        yi = gridDepth - 1;
        for (zi = 0; zi < gridWidth; zi++)
        {
            for (xi = 0; xi < gridWidth; xi++)
            {
                cullNode(xi, yi, zi);
            }
        }

        zi = 0;
        for (xi = 0; xi < gridWidth; xi++)
        {
            for (yi = 0; yi < gridDepth; yi++)
            {
                cullNode(xi, yi, zi);
            }
        }
        zi = gridWidth - 1;
        for (xi = 0; xi < gridWidth; xi++)
        {
            for (yi = 0; yi < gridDepth; yi++)
            {
                cullNode(xi, yi, zi);
            }
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
    //LEGACY - USE CFcull instead
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

    //collider first terrain culling
    //helper function for cullCube
    void CFcull()
    {
        Debug.Log("CM dim: " + cubeMatrix.GetLength(0) + " " + cubeMatrix.GetLength(1) + " " + cubeMatrix.GetLength(2) + " ");
        //itterate throught the collieders
        foreach (Collider col in terainColiders)
        {
            //discard colliders on improper layers
            //this mess sets the bit of the layermask corrisponding to the layer of the object to 1 then checks if the layermask has changed
            if (!(validCullColliders == (validCullColliders | (1 << col.gameObject.layer)))) 
            {
                continue;
            }

            //max and min corners of world alligned bounding box
            Vector3 maxBounds = worldToGrid(col.bounds.max);
            Vector3 minBounds = worldToGrid(col.bounds.min);

            //clamp bounds to cubeMatrix
            Vector3 minCo = new Vector3(0, 0, 0);
            Vector3 maxCo = new Vector3(0, 0, 0);
            minCo.x = Mathf.Floor(Mathf.Clamp(Mathf.Min(minBounds.x, maxBounds.x), 0, cubeMatrix.GetLength(0)));
            minCo.y = Mathf.Floor(Mathf.Clamp(Mathf.Min(minBounds.y, maxBounds.y), 0, cubeMatrix.GetLength(1)));
            minCo.z = Mathf.Floor(Mathf.Clamp(Mathf.Min(minBounds.z, maxBounds.z), 0, cubeMatrix.GetLength(2)));

            maxCo.x = Mathf.Ceil(Mathf.Clamp(Mathf.Max(minBounds.x, maxBounds.x), 0, cubeMatrix.GetLength(0)));
            maxCo.y = Mathf.Ceil(Mathf.Clamp(Mathf.Max(minBounds.y, maxBounds.y), 0, cubeMatrix.GetLength(1)));
            maxCo.z = Mathf.Ceil(Mathf.Clamp(Mathf.Max(minBounds.z, maxBounds.z), 0, cubeMatrix.GetLength(2)));

            //check each dot in region of cube
            for (int ix = (int)minCo.x; ix < (int)maxCo.x; ix++)
            {
                for (int iy = (int)(minCo.y); iy < (int)maxCo.y; iy++)
                {
                    for (int iz = (int)(minCo.z); iz < (int)maxCo.z; iz++)
                    {
                        //check if already culled
                        if (cubeMatrix[ix, iy, iz].valid)
                        {
                            //double check for non axis-aligned bbounding boxes
                            if (col.bounds.Contains(cubeMatrix[ix, iy, iz].pos))
                            {
                                cullNode(ix, iy, iz);
                            }
                        }
                    }
                }
            }

        }
    }

    //returns a grid co-ordinate of a given world position
    //Helper for CFcull
    Vector3 worldToGrid(Vector3 worldPos)
    {
        //convert location to grid co-ordinates
        float fx = (worldPos.x - transform.position.x) * invRes;
        float fz = (worldPos.z - transform.position.z) * invRes;
        float fy = -(worldPos.y - transform.position.y) * invRes;

        return new Vector3(fx, fy, fz);
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

