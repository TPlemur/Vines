using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistControl : MonoBehaviour
{
    public Transform targetObjectTransform = null;
    public Transform targetRotationTransform = null;
    public new ParticleSystem particleSystem = null;
    public PhysFootstepController footstepController = null;
    public float yOffset = -5f;

    Vector3 lastPosition = new();
    Vector3 direction = new();
    float speed = new();
    bool initialized = false;

    void Start()
    {

        // on footstep step
        footstepController.stepped += () => {

            // kick mist
            
            for (int i = 0; i < 10; i++)
            {
                Vector3 velocity = Quaternion.AngleAxis(Random.Range(-30f, 30f), Vector3.up) * direction;
                float particleSpeed = Random.Range(.8f, 1.1f);
                particleSystem?.Emit(new() { position = gameObject.transform.position, velocity = velocity * particleSpeed, startLifetime = 3f, startSize = 1f }, 1);
            }

        };
    }

    void Update()
    {
        if (targetObjectTransform != null)
        {
            Vector3 diff = targetObjectTransform.position - lastPosition;

            direction = diff.normalized;

            speed = initialized == true ? diff.magnitude : 0f;

            lastPosition = targetObjectTransform.position;

            gameObject.transform.position = targetObjectTransform.transform.position + new Vector3(0, yOffset, 0);
        }

        initialized = true;
    }
}
