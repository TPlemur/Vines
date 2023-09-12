using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartSeeker : MonoBehaviour
{
    const string emmisive = "_Emmisive";
    RCcrawler crawler;
    public float wanderSpeed = 0.5f;
    float searchSpeed = 0.4f;
    float seekSpeed = 0.16f;
    float reportSpeed = 0.16f;
    float surroundSpeed = 0.125f;

    float decayTime = 40;
    float cycleTimer = 0;
    float decayRecoveryRatio = 0.25f;

    float agroRange = 30;

    GameObject player;
    GameObject monster;
    GameObject monVineTarget;

    public static bool playerHidden;
    public static GameObject investigateTarget;

    public static bool setSearch = false;
    public static bool setSeek = false;
    public static bool setSurround = false;

    public enum seekerState
    {
        wander,
        surround,
        search,
        report,
        seek
    }

    [SerializeField] float startDelay = 0.02f;
    [SerializeField] Material flashMat;
    [SerializeField] Material baseMat;
    MeshRenderer vineRender;
    public seekerState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = seekerState.wander;
        crawler = GetComponent<RCcrawler>();
        player = GameObject.Find("Player");
        monster = GameObject.Find("Monster");
        monVineTarget = GameObject.Find("FreeFineTarget");
        cycleTimer = Random.Range(0, decayTime);
    }

    // Update is called once per frame
    void Update()
    {
        //decay state if timer takes too long
        cycleTimer += Time.deltaTime;
        if (cycleTimer > decayTime)
        {
            decay();
        }

        //exturnal behavior Overrides
        if (setSearch)
        {
            setState(seekerState.search);
            cycleTimer = 0;
            if (Random.Range(1, 5) == 1) { setSearch = false; } //add some randomization to grab a few different vines
        }
        if (setSeek)
        {
            setState(seekerState.seek);
            cycleTimer = 0;
            if (Random.Range(1, 5) == 1) { setSearch = false; }//add some randomization to grab a few different vines
        }
        if (setSurround)
        {
            setState(seekerState.surround);
            cycleTimer = 0;
            StartCoroutine(setSurroundOff());
        }
        if (Brain.isHiding && (currentState == seekerState.report || currentState == seekerState.search || currentState == seekerState.seek)) { setState(seekerState.surround); }
    }

    IEnumerator setSurroundOff()
    {
        yield return 0;
        setSurround = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        //report finding the player
        if(collider.tag == "Player")
        {
            StartCoroutine(flash());
            setState(seekerState.report);
            cycleTimer = 0;
        }
        //agro on the player
        if(collider.tag == "Monster" && currentState == seekerState.report)
        {
            if (GameStateManager.GeneratorOn)
            {
                if ((monster.transform.position - player.transform.position).magnitude < agroRange)
                {
                    Brain.detectsPlayer = true;
                    setState(seekerState.seek);
                }
                else
                {
                    Brain.investigating = true;
                    setState(seekerState.search);
                }
                cycleTimer = 0;

            }
            else
            {
                setState(seekerState.wander);
                cycleTimer = 0;
            }
        }
        //copy vines with higher priority states
        if(collider.tag == "Vine" && decayRecoveryRatio< cycleTimer/decayTime)
        {
            SmartSeeker colSmt = collider.gameObject.GetComponent<SmartSeeker>();
            if (colSmt != null && colSmt.currentState > currentState && colSmt.currentState != seekerState.surround) 
            {
                StartCoroutine(flash());
                setState(colSmt.currentState);
                cycleTimer = 0;
            }
        }
        if(collider.tag == "ReturnToMonster")
        {
            setState(seekerState.surround);
            cycleTimer = 0;
        }
    }

    //decay to wander, or randomly look for either the player or the monster
    void decay()
    {
        cycleTimer = 0;
        switch (currentState)
        {
            case seekerState.wander:
                if (Random.Range(1, 3) == 1 && GameStateManager.GeneratorOn) { setState(seekerState.search); }
                else if (Random.Range(1, 1) == 1 && GameStateManager.GeneratorOn) { setState(seekerState.surround); }
                break;
            case seekerState.search:
                setState(seekerState.wander);
                break;
            case seekerState.seek:
                setState(seekerState.wander);
                break;
            case seekerState.report:
                break;
            case seekerState.surround:
                setState(seekerState.wander);
                break;
        }
    }
    

    //set the current state to state, and update parameters apropratly
    public void setState(seekerState state)
    {
        currentState = state;
        switch (state)
        {
            case seekerState.wander:
                crawler.isSeek = false;
                crawler.crawlSpeed = wanderSpeed;
                break;
            case seekerState.search:
                crawler.isSeek = true;
                crawler.seekTarget = investigateTarget;
                crawler.crawlSpeed = searchSpeed;
                break;
            case seekerState.seek:
                crawler.isSeek = true;
                crawler.seekTarget = player;
                crawler.crawlSpeed = seekSpeed;
                break;
            case seekerState.report:
                crawler.isSeek = true;
                crawler.seekTarget = monster;
                crawler.crawlSpeed = reportSpeed;
                break;
            case seekerState.surround:
                crawler.isSeek = true;
                crawler.seekTarget = monVineTarget;
                crawler.crawlSpeed = surroundSpeed;
                break;
        }
    }

    float flashTime = 0.125f;
    Vector4 baseEmmisive = new Vector4(0.007843137255f, 0.06666666667f, 0, 0);
    Vector4 flashEmmisive = new Vector4(0.0941176489f, 1.20784318f, 0, 0);
    IEnumerator flash()
    {
        float timer = 0;
        vineRender = GetComponent<MeshRenderer>();
        while (timer < flashTime)
        {
            timer += Time.deltaTime;
            vineRender.material.SetColor(emmisive, Vector4.Lerp(baseEmmisive, flashEmmisive, timer / flashTime));
            yield return 0;
        }
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            vineRender.material.SetColor(emmisive, Vector4.Lerp(baseEmmisive, flashEmmisive, timer / flashTime));
            yield return 0;
        }
        vineRender.material.SetColor(emmisive, baseEmmisive);

    }
}
