using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnforcer : MonoBehaviour
{
    [Header("Player reqs")]
    [SerializeField] float moveRad;
    [SerializeField] float timeBeforeSeeker;

    [Header("Spawner")]
    [SerializeField] GameObject SeekerSpawner;
    [SerializeField] float spawnDist;

    float vineSpeeds;
    float timer = 0;
    Vector3 lastPos;
    SeekingVines seekerScript;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        seekerScript = SeekerSpawner.GetComponent<SeekingVines>();
        vineSpeeds = timeBeforeSeeker * 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        //check if the player has moved outside their rad
        //if(Vector3.Distance(lastPos,transform.position) > moveRad)
        {
            //lastPos = transform.position;
            //timer = 0;
        }

        //spawn a seekerVine if player hasn't moved enough
        if (timer > timeBeforeSeeker)
        {
            timer = 0;
            if (!Brain.isHiding)
            {
                SmartSeeker.investigateTarget.transform.position = transform.position;
            }
            else { SmartSeeker.investigateTarget.transform.position = transform.position; }
            SmartSeeker.setSearch = true;
            /*
            //remove any old branches
            Branch[] bs = SeekerSpawner.GetComponentsInChildren<Branch>();
            foreach(Branch b in bs)
            {
                b.Remove();
            }

            //move spawenr to a random point on a circle around the player
            float theta = Random.value * 2 * Mathf.PI;
            float x = transform.position.x + spawnDist * Mathf.Cos(theta);
            float z = transform.position.z + spawnDist * Mathf.Sin(theta);
            SeekerSpawner.transform.position = new Vector3(x, SeekerSpawner.transform.position.y, z);

            //spawn a seekerVine
            seekerScript.genVine(SeekerSpawner, vineSpeeds, vineSpeeds, vineSpeeds);
            */
        }
    }
}
