using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//public CapsuleCollider CrouchedCol;
	Rigidbody rb;

	[SerializeField] private float jumpForce = 5;

	[SerializeField] private float RunSpeed = .2f;
	[SerializeField] private float crouchSpeed = .06f;

	[SerializeField] private float moveSpeedHorBase = .15f;
	[SerializeField] private float moveSpeedVerBase = .15f;
	private float moveSpeedHorCur;
	private float moveSpeedVerCur;


	[SerializeField] private float bodyRotationSensativity = 2;
	[SerializeField] private float mainCamRotationSensativity = 2;

	[SerializeField] private float camMinAng = -60;
	[SerializeField] private float camMaxAng = 60;

	private float camStandHeight = .727f;
	private float camCrouchHeight = .276f;

	[SerializeField] private Camera mainCam;

	private float mouseX;
	private float mouseY;



	private bool canRun;

	public bool Crouching;
	public bool Runing;
   public  bool Grounded;

    public void Start(){
		Awake ();
	}

	public void Awake(){
		rb = GetComponent<Rigidbody> ();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}

	public void Update(){
        Crouch();

        Movment();

        Rotation();
        //camera
        CameraRotation();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.01f))
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }        
        //body
        if (Grounded ==true)
        {
            Debug.DrawRay(transform.position, -transform.up, Color.cyan);
            Jump();
            Run();
        }
        else
        {
            Debug.DrawRay(transform.position, -transform.up, Color.red);
            return;
        }
       
       
      
    }

	public void Movment(){
		
		float Horizontal = Input.GetAxis ("Horizontal");
		float Vertical = Input.GetAxis ("Vertical");

		transform.position += transform.forward * moveSpeedVerCur * Vertical;
		transform.position += transform.right  * moveSpeedHorCur * Horizontal;

	}

	public void Run(){
		float Vertical = Input.GetAxis ("Vertical");

		if (Vertical >= 1 && Input.GetKey(KeyCode.LeftShift) && canRun) {
			Runing = true;

		} else {
			Runing = false;
		}

		if (Runing) {
			if (Crouching) {
				canRun = false;

			}
			//Movement

			moveSpeedHorCur = moveSpeedHorCur;
			transform.position += transform.forward * RunSpeed * Vertical;


		} else {
			if (!Crouching) {
				canRun = true;
				//Movement
				moveSpeedHorCur = moveSpeedHorBase;
				moveSpeedVerCur = moveSpeedVerBase;
			} else {
				canRun = false;
			}

		}

	}

	public void Crouch(){
		
		if (Input.GetKey(KeyCode.C)) {
			Crouching = true;

		} else {
			Crouching = false;
		}

		if (Crouching) {
			
			//Movement

			moveSpeedHorCur = crouchSpeed;
			moveSpeedVerCur = crouchSpeed;
			//Col
			GetComponent<CapsuleCollider> ().center = new Vector3 (0, -.345f, 0);
			GetComponent<CapsuleCollider> ().height = 1.3f;
			//Camera
			mainCam.transform.localPosition = new Vector3(0, camCrouchHeight ,0) ;

		} else {
			
			//Movement
			moveSpeedHorCur = moveSpeedHorBase;
			moveSpeedVerCur = moveSpeedVerBase;
			//Col
			GetComponent<CapsuleCollider> ().center = new Vector3 (0, 0, 0);
			GetComponent<CapsuleCollider> ().height = 2;
			//Camera
			mainCam.transform.localPosition = new Vector3(0, camStandHeight,0) ;

		}

	}

	public void Jump(){
		
		
		

			if (Input.GetKeyDown(KeyCode.Space)) {
				rb.AddForce (transform.up * jumpForce, ForceMode.Impulse);
			}
		
	}

	public void Rotation(){
		

		 mouseX += Input.GetAxis ("Mouse X") * bodyRotationSensativity ;

		if (mouseX >= 360) {
			mouseX = -360;
		}else if (mouseX <= -360) {
			mouseX = 360;
		}
			

		transform.rotation = Quaternion.Euler (0, mouseX, 0);

	}

	public void CameraRotation(){

		mouseY += -Input.GetAxis ("Mouse Y") * mainCamRotationSensativity ;
		mouseY = Mathf.Clamp (mouseY, camMinAng, camMaxAng);

		mainCam.transform.rotation = Quaternion.Euler (mouseY, transform.eulerAngles.y, 0);


	}
}
