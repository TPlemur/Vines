using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValveInteractable : MonoBehaviour
{
    /// Script for the turning valve handle
    /// 
    /// Gameobject MUST Be
    /// -tagged "Valve"
    /// -on layer interactable
    /// 
    /// set triggerOnComplete to the desired event

    [SerializeField] float rotationSpeed = 90f;    //cosmetic speed of rotation
    [SerializeField] float timeToRotate = 2;       //time in seconds spent rotating
    [SerializeField] UnityEvent triggerOnComplete; //functions to call when rotation is done

    //internal vars
    bool turning = false;
    bool canTurn = true;
    KeyCode interact;
    float timeRotated = 0;

    public void resetValve()
    {
        turning = false;
        canTurn = true;
        timeRotated = 0;
    }

    //starts turning, called by InventoryManager
    public void startInteract(KeyCode interactKey)
    {
        turning = true;
        interact = interactKey;
    }

    // Update is called once per frame
    void Update()
    {
        if (turning && canTurn)
        {
            //stop rotating if key not pressed
            if (!Input.GetKey(interact)) { turning = false; }

            //do the rotation
            transform.Rotate(0, Time.deltaTime * rotationSpeed, 0, Space.Self);

            //check if rotation is done
            timeRotated += Time.deltaTime;
            if (timeRotated > timeToRotate)
            {
                //stop the valve
                canTurn = false;
                //set tag to remove prompt
                this.tag = "Untagged";
                //call connected functions
                triggerOnComplete.Invoke();
            }
        }
    }
}
