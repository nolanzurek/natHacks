using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanleyMove : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float movespeed = 5f;
    [SerializeField] float jumpvel = 5f;
    [SerializeField] Transform cam;
    [SerializeField] CharacterController charControl;
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

        Vector3 direction = new Vector3(hori, 0, vert);
        direction = Quaternion.AngleAxis(cam.rotation.eulerAngles.y,Vector3.up)*direction;
        direction.Normalize();

        charControl.Move(direction*movespeed*Time.deltaTime);
        if (Input.GetButtonDown("Jump")) {
            rb.velocity = new Vector3(rb.velocity.x, jumpvel, rb.velocity.z);
        }

        if (Input.GetButtonDown("Sprint")) {
            movespeed*=2;
        }

        if (Input.GetButtonUp("Sprint")) {
            movespeed/=2;
        }
    }
}
