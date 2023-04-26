using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerTrigger : MonoBehaviour
{
    RCcrawler[] branches;
    CrawlerBranch[] gridBranches;

    public bool toggle = false;
    bool isTog = false;

    // Start is called before the first frame update
    void Start()
    {
        branches = gameObject.GetComponentsInChildren<RCcrawler>();
        gridBranches = gameObject.GetComponentsInChildren<CrawlerBranch>();
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
            foreach(CrawlerBranch c in gridBranches)
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
            foreach (CrawlerBranch c in gridBranches)
            {
                c.isSeek = false;
            }
            //toggle off
            isTog = false;
        }
    }
}