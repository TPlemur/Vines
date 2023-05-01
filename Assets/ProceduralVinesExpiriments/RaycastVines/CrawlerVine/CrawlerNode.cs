using UnityEngine;

public class CrawlerNode
{
    Vector3 position;
    Vector3 normal;
    Vector3 dir;
    float dist;
    public Collider col;

    public CrawlerNode(Vector3 position, Vector3 normal, Vector3 dir, float dist)
    {
        this.position = position;
        this.normal = normal;
        this.dir = dir;
        this.dist = dist;
    }

    public Vector3 getPosition() => position;
    public Vector3 getNormal() => normal;
    public Vector3 getDir() => dir;
    public float getDist() => dist;

}