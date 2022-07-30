using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanleyMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) {
            Vector3 temp = GetComponent<Rigidbody>().velocity;
            temp.y = 5;
            GetComponent<Rigidbody>().velocity = temp;
        }

        if (Input.GetKey("up")) {
            Vector3 temp = GetComponent<Rigidbody>().velocity;
            temp.x = 5;
            GetComponent<Rigidbody>().velocity = temp;
        }

        if (Input.GetKey("down")) {
            Vector3 temp = GetComponent<Rigidbody>().velocity;
            temp.x = -5;
            GetComponent<Rigidbody>().velocity = temp;
        }

        if (Input.GetKey("left")) {
            Vector3 temp = GetComponent<Rigidbody>().velocity;
            temp.z = 5;
            GetComponent<Rigidbody>().velocity = temp;
        }

        if (Input.GetKey("right")) {
            Vector3 temp = GetComponent<Rigidbody>().velocity;
            temp.z = -5;
            GetComponent<Rigidbody>().velocity = temp;
            }
    }
}
