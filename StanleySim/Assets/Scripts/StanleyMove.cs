using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanleyMove : MonoBehaviour
{
    float yvel;
    float offsetAng = 1.6f;
    [SerializeField] float movespeed = 5f;
    [SerializeField] float jumpvel = 5f;
    [SerializeField] Transform cam;
    [SerializeField] CharacterController charControl;
    float turnvel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        yvel += Physics.gravity.y * Time.deltaTime;

        if (Input.GetButtonDown("Jump")) {
            yvel = jumpvel;
        }

        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(hori, 0, vert).normalized;
        direction = Quaternion.AngleAxis(cam.rotation.eulerAngles.y,Vector3.up)*direction;
        direction.Normalize();

        if (direction.x != 0 || direction.y != 0) {
            float angle = (Mathf.Atan2(direction.x, direction.z)-offsetAng)*Mathf.Rad2Deg;
            float smoothangle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnvel, 0.1f);
            transform.rotation = Quaternion.Euler(-90, smoothangle, 0);
        }

        if (direction.magnitude > 0 || yvel != 0) {
            direction = direction*movespeed;
            direction.y = yvel;
            charControl.Move(direction*Time.deltaTime);
        }

        if (Input.GetButtonDown("Sprint")) {
            movespeed*=1.5f;
        }

        if (Input.GetButtonUp("Sprint")) {
            movespeed/=1.5f;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
