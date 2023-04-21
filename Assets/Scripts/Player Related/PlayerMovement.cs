using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public GameObject Monster;
    Brain mobBrain;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float vineDrag = 0.5f;
    public float dragCap = 4;
    public CapsuleCollider capsuleCol;

    float numVines = 0;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public Transform orientation;
    bool grounded;
    
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Movement Inputs")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public bool ToggleCrouch = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
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
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedCapper(){
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //limit velocity if exceeds max
        if(flatVel.magnitude > moveSpeed){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Crouch(){
        if(Input.GetKeyDown(crouchKey)){
            if(!crouching){
                crouching = true;
                capsuleCol.height = 1;
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
        capsuleCol.height = 2;
        crouching = false;
        moveSpeed *= 2;
    }

    //collision check
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Monster")
        {

            if (gameObject.GetComponent<InventoryManager>().inventory.EquippedIsShield())
            {
                Shield sh = (Shield)gameObject.GetComponent<InventoryManager>().inventory.equipped;
                if (sh.explode())
                {
                    //Monster.transform.position += (Monster.transform.position - transform.position);
                    collision.GetComponent<NavMeshAgent>().Warp(Monster.transform.position + mobBrain.shieldDir * mobBrain.shieldBumpDist);
                }
                else
                {
                    // player killed
                    OnPlayerKilled();
                }
            }
            else
            {
                // player killed
                OnPlayerKilled();
            }

        }
        else if(collision.tag == "Vine")
        {
            //Monster.GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
            mobBrain.detectsPlayer = true;
        }
        else if (collision.name == "HideTrigger")
        {
            MixerController.SetHiding(true);
            mobBrain.isHiding = true;
            mobBrain.detectsPlayer = false;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.name == "HideTrigger")
        {
            MixerController.SetHiding(false);
            mobBrain.isHiding = false;
        }

        if (collision.name == "Vine"){
            mobBrain.detectsPlayer = false;
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
}
