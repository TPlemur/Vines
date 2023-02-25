using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShieldDeployer : MonoBehaviour
{
    public LayerMask wallsLayer;
    public LayerMask floorLayer;
    public GameObject WallPrefab;
    GameObject player;
    GameObject wall;
    Cloth wallCloth;
    float finalScale;
    float lerpTime = 0.25f;
    float timer = 0;
    bool doLerp = false; 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (doLerp)
        {
            //scale the wall up over a few secs then make it cloth
            timer += Time.deltaTime;
            float currentScale = Mathf.Lerp(0, finalScale, timer/lerpTime);
            wall.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            if (timer > lerpTime) { doLerp = false; StartCoroutine(startCloth(wallCloth)); }

        }
    }

    //find the wide direction of the corridor
    public void findWidth(Transform spawnpoint)
    {
        float forwardBackward = 0;
        float sideSide = 0;
        //allign with world axis
        Ray scanningRay = new Ray(transform.position,Vector3.forward);
        RaycastHit hit;
        //cast forward and backward
        Physics.Raycast(scanningRay, out hit, 10, wallsLayer);
        forwardBackward += Mathf.Abs((hit.point - transform.position).magnitude);

        RaycastHit hitTwo;
        Ray scanningRaytwo = new Ray(transform.position,-Vector3.forward);
        Physics.Raycast(scanningRaytwo, out hitTwo, 10, wallsLayer);
        forwardBackward += Mathf.Abs((hitTwo.point - transform.position).magnitude);

        //cast to either side
        RaycastHit hitThree;
        Ray scanningRayThree = new Ray(transform.position, Vector3.right);
        Physics.Raycast(scanningRayThree, out hitThree, 10, wallsLayer);
        sideSide += Mathf.Abs((hitThree.point - transform.position).magnitude);

        RaycastHit hitFour;
        Ray scanningRayFour = new Ray(transform.position, -Vector3.right);
        Physics.Raycast(scanningRayFour, out hitFour, 10, wallsLayer);
        sideSide += Mathf.Abs((hitFour.point - transform.position).magnitude);

        RaycastHit floorhit;
        Ray ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(scanningRay, out floorhit, 10, floorLayer);

        //spawn in the shorter of the two directions
        if (forwardBackward < sideSide)
        {
            Debug.Log("ZWide");
            float zCoord = (hit.point.z + hitTwo.point.z) / 2;
            Vector3 wallOrigin = new Vector3(transform.position.x, floorhit.point.y + 1, zCoord);
            wall = Instantiate(WallPrefab, wallOrigin, new Quaternion(0.0f,0.7f,0.0f,0.7f));
            wallCloth = wall.GetComponentInChildren<Cloth>();

            wallCloth.enabled = false;
            float zScale = Mathf.Abs(hit.point.z - hitTwo.point.z);
            finalScale = zScale;
            //bump the player backwards
            if (player.transform.position.x < wall.transform.position.x)
            {
                player.GetComponent<Rigidbody>().AddForce(new Vector3(-600, 300, 0));
            }
            else
            {
                player.GetComponent<Rigidbody>().AddForce(new Vector3(600, 300, 0));
            }

        }
        else
        {
            Debug.Log("XWide");
            float xCoord = (hitThree.point.x + hitFour.point.x) / 2;
            Vector3 wallOrigin = new Vector3(xCoord, floorhit.point.y + 1, transform.position.z);
            wall = Instantiate(WallPrefab, wallOrigin, new Quaternion(0, 0, 0, 0));
            wallCloth = wall.GetComponentInChildren<Cloth>();

            wallCloth.enabled = false;
            float xScale = Mathf.Abs(hitThree.point.x - hitFour.point.x);
            finalScale = xScale;
            //bump the player backwards
            if (player.transform.position.z < wall.transform.position.z)
            {
                player.GetComponent<Rigidbody>().AddForce(new Vector3(0, 300, -600));
            }
            else
            {
                player.GetComponent<Rigidbody>().AddForce(new Vector3(0, 300, 600));
            }

        }

        if(finalScale > 5) { finalScale = 5; }
        doLerp = true;

    }

    IEnumerator startCloth(Cloth cl)
    {
        yield return new WaitForEndOfFrame();
        cl.enabled = true;
        yield return new WaitForSeconds(8);
        cl.gameObject.GetComponent<BoxCollider>().enabled = false;
        cl.gameObject.GetComponent<NavMeshObstacle>().enabled = false;
        cl.externalAcceleration = new Vector3(0, -200, 0);
    }
}
