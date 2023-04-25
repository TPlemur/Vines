using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shield : Item
{
    private Vector3 original;
    private bool toggled = false;
    private bool primed = true;

    Brain mobBrain;

    public void setup(GameObject stateManager, GameObject UIElement)
    {
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        LoadItem("Shield");
        original = itemObj.transform.localPosition;
        ItemUI.transform.GetChild(0).gameObject.SetActive(true);
        gameState.ShieldObtained();
        mobBrain = GameObject.FindGameObjectWithTag("Monster").GetComponentInChildren<Brain>();
        PickupSFX();
        //I don't know why this was initializing as false, but this fixes it
        primed = true;
    }

    public override void Secondary(){
        toggled = !toggled ? true : false;
        // figure out which UI element is active and inactive
        int active = toggled ? 0 : 1;
        int inactive = toggled ? 1 : 0;
        // set active UI to be inactive and inactive to be active
        ItemUI.transform.GetChild(active).gameObject.SetActive(false);
        ItemUI.transform.GetChild(inactive).gameObject.SetActive(true);
        itemObj.transform.localPosition = toggled ? new Vector3((float) 0.36, (float) 0.43, (float) -0.54) : original;
    }

    public override bool IsToggled(){
        return toggled;
    }

    public bool explode(){
        if (primed)
        {
            Debug.Log("EXPLODING SHIELD");
            //itemObj.GetComponentInChildren<ShieldDeployer>().

            //Code from Deploy
            //find the origin and scale
            Quaternion forwardQuaternion = snapToAxis(itemObj.transform.eulerAngles);
            Vector3 ForwardUnitVec = forwardQuaternion * Vector3.forward;

            //failDeploy if back too close to wall
            Ray backCheck = new Ray(player.transform.position, -ForwardUnitVec);
            if (Physics.Raycast(backCheck, maxBackDist,wallsLayer)) { return false; }

            monster.GetComponentInChildren<Brain>().shieldDir = ForwardUnitVec;
            Vector4 PointScale = FindPointAndScale(ForwardUnitVec, itemObj.transform.position);

            //cap Direction
            if (Vector3.Magnitude(new Vector3(PointScale.x, PointScale.y, PointScale.z) - itemObj.transform.position) > maxDist)
            {
                PointScale.x = itemObj.transform.position.x;
                PointScale.z = itemObj.transform.position.z;
            }
            //cap scale
            if (PointScale.w > maxScale) { PointScale.w = maxScale; }

            //instatiate the wall
            wall = Instantiate(WallPrefab, new Vector3(PointScale.x, PointScale.y, PointScale.z), forwardQuaternion);
            wall.GetComponent<ShieldWall>().finalScale = PointScale.w;

            //bomp player backwards
            player.GetComponent<Rigidbody>().AddForce(new Vector3(-600 * ForwardUnitVec.x, 300, -600 * ForwardUnitVec.z));


            ExplodeSFX();
            mobBrain.isShielded = true;
            primed = false;
            Debug.Log("Primed: " + primed);

            //remove shield from inventrory
            Secondary();
            GameStateManager.ShieldAcquired = false;
            InventoryManager im = FindObjectOfType<InventoryManager>();
            int shieldIndex = im.inventory.getCurrentIndex();
            im.inventory.setToZeroth();
            im.inventory.items.RemoveAt(shieldIndex);
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    private void PickupSFX()
    {
        const string eventName = "event:/SFX/Items/Shield/Pickup-1";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(itemObj));
        sound.start();
        sound.release();
    }
    private void ExplodeSFX()
    {
        const string eventName = "event:/SFX/Items/Shield/Explode";
        var sound = FMODUnity.RuntimeManager.CreateInstance(eventName);
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(itemObj.transform));
        sound.start();
        sound.release();
    }

    //DeployerFunctionality
    ///
    ///
    ///

    [Header("References")]
    [SerializeField] LayerMask wallsLayer = 1 << 10;
    [SerializeField] LayerMask floorLayer = 1 << 6;
    [SerializeField] string wallPath = "Prefabs/ClothWall";
    [SerializeField] GameObject WallPrefab;
    [SerializeField] MeshRenderer airBags;

    [Header("characteristics")]
    [SerializeField] float maxScale =  5;
    [SerializeField] float maxDist =  10;
    float maxBackDist = 2f;

    GameObject player;
    GameObject monster;
    GameObject wall;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        monster = GameObject.FindGameObjectWithTag("Monster");
        WallPrefab = Resources.Load(wallPath) as GameObject;
    }

    private void Update()
    {
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.Y))
        {
            explode();
        }
    }

    //return a quaternion in the nearest cardinal direction to the input euler rotation
    //Helper fuction for Deploy
    Quaternion snapToAxis(Vector3 input)
    {
        float YVal;
        //find rotation around Y Axis
        if (Mathf.Abs(input.x) == 90) { YVal = input.z; }
        else { YVal = input.y; }

        //snap YVal to nearest cardinal direction
        if (YVal < 45 || YVal > 315) { YVal = -90; }
        else if (YVal < 135) { YVal = 0; }
        else if (YVal < 225) { YVal = 90; }
        else { YVal = 180; }

        //return the direction as a quaternion to be used on Vector3.Forward
        return Quaternion.Euler(0, YVal, 0);
    }

    //returns a vector4 containing:
    //the w componenet is the scale required to make a 1 unit object fit between two wall tags
    //the xyz component is the middle floor location between the walls
    Vector4 FindPointAndScale(Vector3 forwardDirection, Vector3 origin)
    {
        //get dist to right wall
        Ray ray = new Ray(origin, Quaternion.Euler(0, 90, 0) * forwardDirection);
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
        return new Vector4(hit.point.x, hit.point.y, hit.point.z, unityDist);
    }


}
