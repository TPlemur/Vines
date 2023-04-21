using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foobar : MonoBehaviour
{

    [SerializeField] int xRes = 426;
    [SerializeField] int yRes = 240;
    [SerializeField] RenderTexture rt;

    public void setRes(int m)
    {
        rt.height = m * yRes;
        rt.width = m * xRes;
    }
        
        // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
