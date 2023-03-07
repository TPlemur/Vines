using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorVibe : MonoBehaviour
{
    [SerializeField] private float vibeFrequency;
    [SerializeField] private Vector3 vibeAmount = new Vector3(0.1f,0.1f,0.1f);
    private float timer = 0;
    private bool vibeUP = true;


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > vibeFrequency)
        {
            timer = 0;
            if (vibeUP)
            {
                vibeUP = false;
                transform.localScale += vibeAmount;
            }
            else
            {
                vibeUP = true;
                transform.localScale -= vibeAmount;
            }
        }
    }
}
