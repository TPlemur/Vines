using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public GameObject Monster;
    Brain mobBrain;
    public float safeMonContactTime = 1.5f;
    private bool touchingMonster = false;
    [SerializeField] GameObject vineInvestigationTarget;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float vineDrag = 0.5f;
    public float dragCap = 4;
    public CapsuleCollider FloorCollider;
    public CapsuleCollider WallsCollider;

    float numVines = 0;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public Transform orientation;
    bool grounded;
    
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Movement Inputs")]
    public KeyCode crouchKey = KeyCode.LeftShift;
    public bool ToggleCrouch = false;
    public KeyCode sprintKey = KeyCode.LeftControl;
    private bool crouching = false;
    private float horizontalInput;
    private float verticalInput;

    [Header("Jumpscare Related")]
    public GameObject JumpscareWorldPosition;
    public GameObject JumpscareCameraPosition;
    public GameObject ItemCamera;
    public GameObject Objectives;
    public GameObject ItemPosition;

    [Header("Debug")]
    [SerializeField]
    private bool debugInvincibility = false;

    public bool IsGrounded() { return grounded; }
    public bool IsCrouching() { return crouching; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        mobBrain = Monster.GetComponentInChildren<Brain>();
        SmartSeeker.investigateTarget = vineInvestigationTarget;
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, 1.5f, whatIsGround);
        // get inputs
        GetInputs();
        Crouch();
        SpeedCapper();
        //handle drag
        float drag = vineDrag * numVines + groundDrag;
        if(dragCap < drag) { drag = dragCap; }
        if(grounded){
            rb.drag = drag;
        }else{
            rb.drag = 0;
        }

        //reset numVines
        numVines = 0;

        // Debug testing to instakill player lol
        if (GameStateManager.debug && Input.GetKeyDown(KeyCode.K)){
            OnPlayerKilled();
        }
        runHint();
    }

    private void FixedUpdate(){
        MovePlayer();
    }

    private void GetInputs(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer(){
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (Input.GetKey(KeyCode.S)) {
            rb.AddForce(moveDirection.normalized * moveSpeed * 4f, ForceMode.Force);
        }
        else {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
    }

    private void SpeedCapper(){
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //limit velocity if exceeds max
        if(flatVel.magnitude > moveSpeed){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    [SerializeField] float crouchHeight;

    private void Crouch(){
        if(Input.GetKeyDown(crouchKey)){
            if(!crouching){
                crouching = true;
                FloorCollider.height = 1;
                WallsCollider.height = 0.8f;
                moveSpeed /= 2;
            }
            else if(crouching && ToggleCrouch){
                UnCrouch();
            }
            else if(!ToggleCrouch && crouching){
                UnCrouch();
            }
        }
        if(Input.GetKeyUp(crouchKey) && !ToggleCrouch && crouching){
            UnCrouch();
        }
    }

    private void UnCrouch(){
        FloorCollider.height = 2;
        WallsCollider.height = 1.8f;
        crouching = false;
        moveSpeed *= 2;
    }

    //collision check
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Monster")
        {
            touchingMonster = true;
            if (gameObject.GetComponent<InventoryManager>().inventory.EquippedIsShield())
            {
                Shield sh = gameObject.GetComponent<InventoryManager>().inventory.equipped.GetComponent<Shield>();
                if (sh.explode())
                {
                    //Monster.transform.position += (Monster.transform.position - transform.position);
                    collision.GetComponent<NavMeshAgent>().Warp(Monster.transform.position + mobBrain.shieldDir * mobBrain.shieldBumpDist);
                }
                else
                {
                    // player killed
                    StartCoroutine(touchedMonster());
                }
            }
            else if(mobBrain.currentMonState != Brain.monState.sleep)
            {
                // player killed
                StartCoroutine(touchedMonster());
            }

        }
        else if(collision.tag == "Vine")
        {
            //Monster.GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
            //Brain.detectsPlayer = true;
        }
        else if (collision.name == "HideTrigger")
        {
            MixerController.SetHiding(true);
            Brain.isHiding = true;
            //Brain.detectsPlayer = false;
            SmartSeeker.playerHidden = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Monster")
        {
            touchingMonster = false;
        }
        
        if (collision.name == "HideTrigger")
        {
            MixerController.SetHiding(false);
            Brain.isHiding = false;
            SmartSeeker.playerHidden = false;
        }

        if (collision.name == "Vine"){
           // Brain.detectsPlayer = false;
        }
    }

    //count currently active vine collisions
    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Vine")
        {
            numVines++;
        }
        if (collision.name == "HideTrigger")
        {
            mobBrain.timeHidden += Time.deltaTime;
        }
    }

    IEnumerator LoadAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(3);
    }

    IEnumerator touchedMonster(){
        yield return new WaitForSeconds(safeMonContactTime);
        if(touchingMonster){
            OnPlayerKilled();
        }
    }

    private void OnPlayerKilled()
    {
        if (!debugInvincibility && !mobBrain.isShielded){
            JumpscareCameraPosition.SetActive(true);
            this.gameObject.transform.position = JumpscareWorldPosition.GetComponent<Transform>().position;
            ItemPosition.SetActive(false);
            ItemCamera.SetActive(false);
            Objectives.SetActive(false);
            StartCoroutine(LoadAfterTime((float)0.95));
        }
    }

    //update hints for the player
    float HintUpdateTime = 10;
    float HintTimer = 0;
    void runHint()
    {
        HintTimer += Time.deltaTime;
        if (HintTimer > HintUpdateTime)
        {
            HintTimer = 0;
            if (!Brain.isHiding)
            {
                SmartSeeker.investigateTarget.transform.position = transform.position;
            }
            else { SmartSeeker.investigateTarget.transform.position = Monster.transform.position; }
            SmartSeeker.setSearch = true;
        }
    }
}
