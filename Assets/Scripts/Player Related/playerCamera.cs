using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    public float _tiltAmount = 5;
    public float _rotationSpeed = 0.5f;
 

    // Start is called before the first frame update
    void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Tilt();
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        
        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        Vector3 v = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, v.z);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void Tilt()
    {
        float rotZ = -Input.GetAxis("Horizontal") * _tiltAmount;

        Quaternion finalRot = Quaternion.Euler(xRotation, yRotation, rotZ);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, finalRot, _rotationSpeed);
    }
}
