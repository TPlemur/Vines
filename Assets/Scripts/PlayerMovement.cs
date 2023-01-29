using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public CapsuleCollider capsuleCol;

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

    public bool IsGrounded() { return grounded; }
    public bool IsCrouching() { return crouching; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, 1.0f, whatIsGround);
        // get inputs
        GetInputs();
        Crouch();
        SpeedCapper();
        //handle drag
        if(grounded){
            rb.drag = groundDrag;
        }else{
            rb.drag = 0;
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
}
