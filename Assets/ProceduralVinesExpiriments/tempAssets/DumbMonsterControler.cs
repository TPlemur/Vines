using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbMonsterControler : MonoBehaviour
{
    public float MoveSpeed = 1;
    int corner = 1;
    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
            switch (corner) {

                case 1:
                transform.position += (new Vector3(1, 0, 0) * MoveSpeed*Time.deltaTime);
                if (transform.position.x >= 2.75)
                    {
                        corner = 2;
                    transform.Rotate(new Vector3(0, -90, 0));
                    }
                    break;
            case 2:
                transform.position += (new Vector3(0, 0, 1) * MoveSpeed * Time.deltaTime);
                if (transform.position.z >= 2.75)
                {
                    corner = 3;
                    transform.Rotate(new Vector3(0, -90, 0));
                }
                break;
            case 3:
                transform.position += (new Vector3(-1, 0, 0) * MoveSpeed * Time.deltaTime);
                if (transform.position.x <= -2.75)
                {
                    corner = 4;
                    transform.Rotate(new Vector3(0, -90, 0));
                }
                break;
            case 4:
                transform.position += (new Vector3(0, 0, -1) * MoveSpeed * Time.deltaTime);
                if (transform.position.z <= -2.75)
                {
                    corner = 1;
                    transform.Rotate(new Vector3(0, -90, 0));
                }
                break;

        }
    }
}
