using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This file is basicly a mess of hand-coded animations, 
//there is probably a better way to do this using real animations, 
//but this is the skillset I have
public class DeathManager : MonoBehaviour
{
    [SerializeField] float playerHeight;

    [SerializeField] GameObject monster;
    [SerializeField] GameObject playerCam;
    [SerializeField] LayerMask wallsLayer;
    [SerializeField] LayerMask floorLayer;
    [SerializeField] MoveCamera moveCam;
    [SerializeField] PlayerMovement playermove;
    [SerializeField] playerCamera playerCamScript;
    [SerializeField] HeadBobbing headbob;
    [SerializeField] Animator monsterAnim;
    [SerializeField] Inventory inventory;
    [SerializeField] InventoryManager inventoryman;
    [SerializeField] GameObject player;
    [SerializeField] FadeController fadeCon;
    [SerializeField] List<ProceduralIvy> playerVineWrapSpawnners;


    [SerializeField] GameObject roarCamTarget;

    float fallTime = 0.5f;
    float afterFallTime = 2;
    float topAngle = Mathf.PI / 2;
    float bottomAngle = 0;
    float dragDist = 0.5f;

    public bool DIEDIEDIE = false;
    //start dying
    public void DIE()
    {
        //Freeze player and monster capabilites
        monsterAnim.Play("Sleep State");
        moveCam.enabled = false;
        playermove.enabled = false;
        playerCamScript.enabled = false;
        headbob.enabled = false;
        inventoryman.enabled = false;
        player.SetActive(false);


        //get the normalized direcion of the monster from the player, ignoring the Y coordinate
        Vector3 playerToMonster = monster.transform.position - playerCam.transform.position;
        playerToMonster.y = 0;
        playerToMonster = Vector3.Normalize(playerToMonster);

        //check if wall
        Ray wallCheck = new Ray(playerCam.transform.position, -playerToMonster);
        RaycastHit hit;
        if (Physics.Raycast(wallCheck, out hit, playerHeight, wallsLayer))
        {
            pinToWall(hit);
            return;
        }

        //find the angle between
        Vector3 playerForNoY = playerCam.transform.forward;
        playerForNoY.y = 0;
        float angleToMon = Vector3.Angle(playerForNoY, playerToMonster);



        if(angleToMon < 90) { fallBackwardsAndRoar(); }
        else { fallForwardsAndDrag(); }

    }

    /// <summary>
    /// Case where the player falls backwards and the monster looms over them
    /// </summary>
    void fallBackwardsAndRoar()
    {
        //find floor position to piviot around
        Ray floorPivotFinder = new Ray(playerCam.transform.position, new Vector3(0, -1, 0));
        RaycastHit hit;
        Physics.Raycast(floorPivotFinder, out hit, floorLayer);
        Vector3 floorPivot = hit.point + new Vector3(0, 0.1f, 0); //add a small amount to avoid clipping through floor

        //find normalized look angle with no Y coordinate
        Vector3 lookDir = playerCam.transform.forward;
        lookDir.y = 0;
        lookDir = lookDir.normalized;

        StartCoroutine(fallBack(lookDir, floorPivot));

    }

    IEnumerator fallBack(Vector3 lookAngle, Vector3 pivot )
    {
        float timer = fallTime;
        float angle = topAngle;
        float XZCo;
        float YCo;
        float xRotDelta = playerCam.transform.rotation.x - 90;
        Vector3 newPos;

        Vector3 monStartPos = monster.transform.position;
        Vector3 dirToRoarTarget = roarCamTarget.transform.position - playerCam.transform.position;
        dirToRoarTarget = Vector3.Normalize(dirToRoarTarget);
        Vector3 monLookDir = playerCam.transform.position - monster.transform.position;
        monLookDir.y = 0;
        var monLookRotation = Quaternion.LookRotation(monLookDir);

        //fall over and move monster to players feet
        while (angle > bottomAngle) {

            //change the position of the camera
            XZCo = Mathf.Cos(angle) * 1.1f;
            YCo = Mathf.Sin(angle);
            newPos = -lookAngle * XZCo + new Vector3(0, YCo, 0) + pivot;
            playerCam.transform.position = newPos;
            timer -= Time.deltaTime;
            angle = (timer / fallTime) * topAngle;

            //change the rotation of the camera
            float currentxRot =  xRotDelta * Time.deltaTime / fallTime;
            playerCam.transform.Rotate(new Vector3(currentxRot, 0, 0));

            //change the position and rotation of the monster
            monster.transform.position = Vector3.Lerp( pivot + 2 * new Vector3(dirToRoarTarget.x, 0, dirToRoarTarget.z), monStartPos, timer / fallTime);
            monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, monLookRotation, Time.deltaTime / fallTime);
            yield return 0;
        }
        

        angle = bottomAngle;
        XZCo = Mathf.Sin(angle);
        YCo = Mathf.Cos(angle);
        newPos = lookAngle * XZCo + new Vector3(0, YCo, 0) + pivot;
        
        playerCam.transform.position = newPos;

        monsterAnim.enabled = true;
        monsterAnim.Play("Precharge State");

        //monster roars and player looks back up at him
        timer = 0;
        dirToRoarTarget = roarCamTarget.transform.position - playerCam.transform.position;
        var angleToLook = Quaternion.LookRotation(dirToRoarTarget);
        while(timer < afterFallTime)
        {
            //dirToRoarTarget = roarCamTarget.transform.position - playerCam.transform.position;
            timer += Time.deltaTime;
            playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, angleToLook, Time.deltaTime / afterFallTime) ;
            yield return 0;
        }

        //load the end scene
        fadeCon.FadeOutToSceen(1, 3);
    }

    /// <summary>
    /// Case where the player falls forward and is dragged backwards
    /// </summary>
    void fallForwardsAndDrag()
    {         
        //find floor position to piviot around
        Ray floorPivotFinder = new Ray(playerCam.transform.position, new Vector3(0, -1, 0));
        RaycastHit hit;
        Physics.Raycast(floorPivotFinder, out hit, floorLayer);
        Vector3 floorPivot = hit.point + new Vector3(0, 0.1f, 0); //add a small amount to avoid clipping through floor

        //find normalized look angle with no Y coordinate
        Vector3 lookDir = playerCam.transform.forward;
        lookDir.y = 0;
        lookDir = lookDir.normalized;

        StartCoroutine(fallForwardAndDrag(lookDir, floorPivot));
    }

    IEnumerator fallForwardAndDrag(Vector3 lookAngle, Vector3 pivot)
    {
        StartCoroutine(fallFor(lookAngle, pivot));
        yield return new WaitForSeconds(fallTime);

        //start lifting head and looking to the side
        StartCoroutine(acLift());
        StartCoroutine(lookRight());
        yield return new WaitForSeconds(afterFallTime / 3);

        //Start growing vines
        foreach (ProceduralIvy v in playerVineWrapSpawnners) {
            v.gameObject.transform.position = Vector3.zero;
            v.gameObject.transform.eulerAngles = Vector3.zero;
            v.GenIvy();
        }

        //drag in spurts
        Vector3 dragDelta = monster.transform.position - playerCam.transform.position;
        dragDelta.y = 0;
        dragDelta = dragDelta.normalized;
        dragDelta *= dragDist;

        StartCoroutine(drag(dragDelta, (afterFallTime / 2)));
        yield return new WaitForSeconds(afterFallTime / 2);

        StartCoroutine(drag(dragDelta, (afterFallTime / 2)));
        yield return new WaitForSeconds(afterFallTime / 2);

        //start fadeout;
        fadeCon.FadeOutToSceen(1, 3);

        StartCoroutine(drag(dragDelta, (afterFallTime / 2)));
        yield return new WaitForSeconds(afterFallTime / 2);

        StartCoroutine(drag(dragDelta, (afterFallTime / 2)));
        yield return new WaitForSeconds(afterFallTime / 2);
    }

    IEnumerator lookRight()
    {
        //look sideways
        Vector3 newLookDir = playerCam.transform.right;
        Quaternion newLookQuat = Quaternion.LookRotation(newLookDir);
        Quaternion oldLookQuat = playerCam.transform.rotation;
        float timer = 0;
        while (timer < afterFallTime / 2)
        {
            timer += Time.deltaTime;

            //rotate camera
            playerCam.transform.RotateAround(playerCam.transform.position, playerCam.transform.up, 80f * Time.deltaTime);
            //stop vines from shifting
            foreach (ProceduralIvy v in playerVineWrapSpawnners)
            {
                v.gameObject.transform.position = Vector3.zero;
                v.gameObject.transform.eulerAngles = Vector3.zero;
            }
            yield return 0;
        }
    }
    
    IEnumerator acLift()
    {
        //lift to avoid clipping
        float timer = 0;
        float yAdj = playerCam.transform.position.y + 0.1f;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            playerCam.transform.position = new Vector3(playerCam.transform.position.x, Mathf.Lerp(playerCam.transform.position.y, yAdj, timer / 0.2f), playerCam.transform.position.z);
            yield return 0;
        }
    }

    IEnumerator fallFor(Vector3 lookAngle, Vector3 pivot)
    {
        float timer = fallTime;
        float angle = topAngle;
        float XZCo;
        float YCo;
        float xRotDelta = playerCam.transform.parent.rotation.x - 90;
        Vector3 newPos;

        Vector3 monStartPos = monster.transform.position;
        Vector3 dirToRoarTarget = roarCamTarget.transform.position - playerCam.transform.position;
        dirToRoarTarget = Vector3.Normalize(dirToRoarTarget);
        Vector3 monLookDir = playerCam.transform.position - monster.transform.position;
        monLookDir.y = 0;
        var monLookRotation = Quaternion.LookRotation(monLookDir);

        //fall over and move monster to players feet
        while (angle > bottomAngle)
        {
            //change the position of the camera
            XZCo = Mathf.Cos(angle)*1.1f;
            YCo = Mathf.Sin(angle);
            newPos = lookAngle * XZCo + new Vector3(0, YCo, 0) + pivot;
            playerCam.transform.position = newPos;
            timer -= Time.deltaTime;
            angle = (timer / fallTime) * topAngle;

            //change the rotation of the camera
            float currentxRot = xRotDelta * Time.deltaTime / fallTime;
            playerCam.transform.Rotate(new Vector3(-currentxRot, 0, 0));

            //change the position and rotation of the monster
            monster.transform.position = Vector3.Lerp(pivot + 2 * new Vector3(dirToRoarTarget.x, 0, dirToRoarTarget.z), monStartPos, timer / (fallTime + 0.1f));
            monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, monLookRotation, Time.deltaTime / fallTime);
            yield return 0;
        }
    }

    IEnumerator drag(Vector3 dragDelta, float time)
    {
        float timer = -0;
        Vector3 playerStartPos = playerCam.transform.position;
        Vector3 monsterStartPos = monster.transform.position;

        while (timer < afterFallTime / 4)
        {
            timer += Time.deltaTime;

            //move Player and monster
            float newPX = playerStartPos.x + Mathf.SmoothStep(0, dragDelta.x, timer / time);
            float newPZ = playerStartPos.z + Mathf.SmoothStep(0, dragDelta.z, timer / time);
            playerCam.transform.position = new Vector3(newPX, playerCam.transform.position.y, newPZ);

            float newMX = monsterStartPos.x + Mathf.SmoothStep(0, dragDelta.x, timer / time);
            float newMZ = monsterStartPos.x + Mathf.SmoothStep(0, dragDelta.x, timer / time);
            monster.transform.position = new Vector3(newMX, monster.transform.position.y, newMZ);

            yield return 0;
        }
    }

    void pinToWall(RaycastHit hit) { playermove.enabled = true; playermove.OnPlayerKilled();}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (DIEDIEDIE)
     //   {
    //        DIEDIEDIE = false;
     //       DIE();
    //    }
  //  }
}
