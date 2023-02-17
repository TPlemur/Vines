using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasLight : MonoBehaviour
{
    public List<Transform> lights;
    [SerializeField] private float IllumDist = 10; //maximum distance
    [SerializeField] private float illumAngle = 1; //maximum angle in radians (about a quarter of light cone)

    //find lights of type
    public void findLights(UnityEngine.LightType lT)
    {
        Light[] foundLights = Object.FindObjectsOfType<Light>();
        foreach (Light l in foundLights)
        {
            if (l.type == lT)
            {
                lights.Add(l.transform);
            }
        }
    }

    //find all lights in scene
    public void findLights()
    {
        Light[] foundLights = Object.FindObjectsOfType<Light>();
        foreach (Light l in foundLights)
        {
            lights.Add(l.transform);
        }
    }

    //check if attached gameobject is within light area
    public bool isLit()
    {
        foreach(Transform l in lights)
        {
            Vector3 ld = transform.position - l.position;
            if (Mathf.Abs(ld.magnitude) < IllumDist)
            {
                if (Mathf.Acos( Vector3.Dot(ld, l.forward) / ld.magnitude) < illumAngle)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
