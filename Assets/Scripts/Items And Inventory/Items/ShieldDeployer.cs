using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShieldDeployer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LayerMask wallsLayer;
    [SerializeField] LayerMask floorLayer;
    [SerializeField] GameObject WallPrefab;
    [SerializeField] MeshRenderer airBags;

    [Header("characteristics")]
    [SerializeField] float maxScale = 5;
    [SerializeField] float maxDist = 10;

    GameObject player;
    GameObject wall;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Deploy();
        }
    }

    //return a quaternion in the nearest cardinal direction to the input euler rotation
    Quaternion snapToAxis(Vector3 input)
    { 
        float YVal;
        //find rotation around Y Axis
        if(Mathf.Abs(input.x) == 90) { YVal = input.z; }
        else { YVal = input.y; }

        //snap YVal to nearest cardinal direction
        if(YVal < 45 || YVal > 315) { YVal = -90; }
        else if(YVal < 135) { YVal = 0; }
        else if(YVal < 225) { YVal = 90; }
        else { YVal = 180; }

        //return the direction as a quaternion to be used on Vector3.Forward
        return  Quaternion.Euler(0, YVal, 0);
    }

    //returns a vector4 containing:
    //the w componenet is the scale required to make a 1 unit object fit between two wall tags
    //the xyz component is the middle floor location between the walls
    Vector4 FindPointAndScale(Vector3 forwardDirection, Vector3 origin)
    {
        //get dist to right wall
        Ray ray = new Ray(origin, Quaternion.Euler(0, 90, 0)*forwardDirection);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, maxScale, wallsLayer);
        float unityDist = Mathf.Abs((hit.point - origin).magnitude);

        //get dist to left wall
        ray = new Ray(origin, Quaternion.Euler(0, -90, 0) * forwardDirection);
        Physics.Raycast(ray, out hit, maxScale, wallsLayer);
        unityDist += Mathf.Abs((hit.point - origin).magnitude);

        //multiply half the distance by the direction back towards the other hit
        Vector3 centerPoint = hit.point + (Quaternion.Euler(0, 90, 0) * forwardDirection) * (unityDist / 2);

        //find the floor
        ray = new Ray(centerPoint, Vector3.down);
        Debug.DrawRay(centerPoint, Vector3.down, Color.red, 1, false);
        Physics.Raycast(ray, out hit, 10, floorLayer);

        //return the composit of the position and distance
        return new Vector4(hit.point.x, hit.point.y, hit.point.z,unityDist);
    }


    public void Deploy()
    {
        //find the origin and scale
        Quaternion forwardQuaternion = snapToAxis(transform.eulerAngles);
        Vector3 ForwardUnitVec = forwardQuaternion * Vector3.forward;
        Vector4 PointScale = FindPointAndScale(ForwardUnitVec, transform.position);

        //cap Direction
        if(Vector3.Magnitude( new Vector3(PointScale.x,PointScale.y,PointScale.z) - transform.position) > maxDist)
        {
            PointScale.x = transform.position.x;
            PointScale.z = transform.position.z;
        }
        //cap scale
        if(PointScale.w > maxScale) { PointScale.w = maxScale; }

        //instatiate the wall
        wall = Instantiate(WallPrefab, new Vector3(PointScale.x,PointScale.y,PointScale.z), forwardQuaternion);
        wall.GetComponent<ShieldWall>().finalScale = PointScale.w;

        //bomp player backwards
        player.GetComponent<Rigidbody>().AddForce(new Vector3(-600 * ForwardUnitVec.x, 300, -600 * ForwardUnitVec.z));

        //dissable airbags
        airBags.enabled = false;
    }
}
