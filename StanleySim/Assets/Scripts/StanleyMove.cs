using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanleyMove : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float movespeed = 5f;
    [SerializeField] float jumpvel = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(hori*movespeed, rb.velocity.y, vert*movespeed);

        if (Input.GetButtonDown("Jump")) {
            rb.velocity = new Vector3(rb.velocity.x,jumpvel,rb.velocity.z);
        }
    }
}
