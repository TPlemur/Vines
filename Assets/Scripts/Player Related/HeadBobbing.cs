using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{

    public float bobIntensity = 0.05f;
    public float bobSpeed = 14f;

    float defaultY = 0;
    //float defaultX = 0;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        defaultY = transform.localPosition.y;
        //defaultX = transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
            //Player is moving
            timer += Time.deltaTime * bobSpeed;
            //transform.localPosition = new Vector3(defaultX + Mathf.Sin(timer) * bobIntensity, defaultY + Mathf.Sin(timer) * bobIntensity, transform.localPosition.z);
            transform.localPosition = new Vector3(transform.localPosition.x, defaultY + Mathf.Sin(timer) * bobIntensity, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            //transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, defaultX, Time.deltaTime * bobSpeed), Mathf.Lerp(transform.localPosition.y, defaultY, Time.deltaTime * bobSpeed), transform.localPosition.z);
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultY, Time.deltaTime * bobSpeed), transform.localPosition.z);
        }
    }
}
