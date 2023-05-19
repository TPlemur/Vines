using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerTrigger : MonoBehaviour
{
    RCcrawler[] branches;
    CrawlerBranch[] gridBranches;

    public bool toggle = false;
    bool isTog = false;

    [SerializeField] GameObject monTarget;
    [SerializeField] float monSpeed = 0.125f;
    [SerializeField] GameObject playerTarget;
    [SerializeField] float playerSpeed = 0.08f;
    [SerializeField] float initializationDelay = 1f;
    [SerializeField] float startDelay = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        branches = gameObject.GetComponentsInChildren<RCcrawler>();
        gridBranches = gameObject.GetComponentsInChildren<CrawlerBranch>();
        StartCoroutine(startVines());
    }
    /*
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
    */
    public void setWander()
    {
        toggle = false;
    }

    public void setSeekTarget(GameObject target, float speed)
    {
        foreach (RCcrawler b in branches)
        {
            b.seekTarget = target;
            b.crawlSpeed = speed;
       
        }
    }

    public void setOnPlayer()
    {
        setSeekTarget(playerTarget,playerSpeed);
    }

    public void setOnMonster()
    {
        setSeekTarget(monTarget,monSpeed);
    }

    IEnumerator startVines()
    {
        yield return new WaitForSeconds(initializationDelay);

        foreach(RCcrawler b in branches)
        {
            //wait one frame to stagger vine updates
            yield return 0;
            b.spawn();
            b.gameObject.GetComponent<SmartSeeker>().setState(SmartSeeker.seekerState.wander);
        }
    }
}
