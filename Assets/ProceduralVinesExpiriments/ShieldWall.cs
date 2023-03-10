using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//handles the behavior of the wall after it is spawned
public class ShieldWall : MonoBehaviour
{
    //Tuning Variables
    public float finalScale = 2;
    [SerializeField] float lerpTime = 2f;
    [SerializeField] float clothLerp = 8f;
    [SerializeField] Vector3 maxAccel = new Vector3(0, -200, 0);

    //Internal Variables
    Cloth wallCloth;
    bool doLerp = false;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        wallCloth = GetComponentInChildren<Cloth>();
        doLerp = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (doLerp)
        {
            //scale the wall up then make it cloth
            timer += Time.deltaTime;
            float currentScale = Mathf.Lerp(0, finalScale, timer / lerpTime);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            if (timer > lerpTime) { doLerp = false; StartCoroutine(startCloth(wallCloth)); }

        }
    }

    IEnumerator startCloth(Cloth cl)
    {
        //reset time and wait to make sure scale and cloth do not overlap
        timer = 0;
        yield return new WaitForEndOfFrame();

        //enable cloth and increase acceleration over time.
        cl.enabled = true;
        while(timer < clothLerp)
        {
            cl.externalAcceleration = Vector3.Lerp(Vector3.zero,maxAccel, timer / clothLerp);
            timer += Time.deltaTime;
            yield return null;
        }

        //snap to final values
        cl.gameObject.GetComponent<BoxCollider>().enabled = false;
        cl.gameObject.GetComponent<NavMeshObstacle>().enabled = false;
        cl.externalAcceleration = maxAccel;
    }
}
