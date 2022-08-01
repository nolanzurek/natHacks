using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rngMove : MonoBehaviour
{
    CharacterController charControl;
    Vector3 direction; 
    private float turnvel;
    private int frame = 0;
    [SerializeField] float offset;

    // Start is called before the first frame update
    void Start()
    {
        charControl = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (frame > 90) {
            direction = new Vector3(Random.Range(-3,3), 0, Random.Range(-3,3));
            frame = 0;
        }
        float angle = (Mathf.Atan2(direction.x, direction.z)-offset)*Mathf.Rad2Deg;
        float smoothangle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnvel, 0.1f);
        transform.rotation = Quaternion.Euler(-90, smoothangle, 0);
        direction.y = Physics.gravity.y;
        charControl.Move(direction*Time.deltaTime);
        frame++;
    }
}
