using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerTrigger : MonoBehaviour
{
    RCcrawler[] branches;

    public bool toggle = false;
    bool isTog = false;

    // Start is called before the first frame update
    void Start()
    {
        branches = gameObject.GetComponentsInChildren<RCcrawler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(toggle && !isTog)
        {
            foreach(RCcrawler c in branches)
            {
                c.isSeek = true;
            }
            //toggle on
            isTog = true;
        }
        if(!toggle && isTog)
        {
            foreach (RCcrawler c in branches)
            {
                c.isSeek = false;
            }
            //toggle off
            isTog = false;
        }
    }
}
