using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StanleyMove : MonoBehaviour
{
    private float turnvel;
    private float yvel;
    private float offsetAng = 1.6f;
    private bool grounded = true;

    [Header("Movement Settings")]
    [SerializeField] private float movespeed = 5f;
    [SerializeField] private float jumpvel = 5f;

    [Header("Component Access")]
    [SerializeField] Transform cam;
    [SerializeField] CharacterController charControl;
    [SerializeField] Transform gc;
    [SerializeField] LayerMask terrain;
    [SerializeField] Image staminaBar;
    [SerializeField] CanvasGroup canvas;

    [Header("Stamina Parameters")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float sprintCost = 0.25f;
    [SerializeField] private float regenRate = 0.35f;
    [Header("Stamina Info")]
    [SerializeField] float currentStamina = 100.0f;
    [SerializeField] bool sprinting = false;
    [SerializeField] bool fullStamina = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateData();
        updateMove();
        updateStamina();
    }

    private void updateMove() {
        yvel += Physics.gravity.y * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && grounded) {
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

        if (Input.GetButtonDown("Sprint") && currentStamina > 0) {
            movespeed*=1.5f;
            sprinting = true;
        }

        if (Input.GetButtonUp("Sprint") && movespeed > 10) {
            movespeed/=1.5f;
            sprinting = false;
        }

        if (currentStamina < 0) {
            currentStamina = 0;
            sprinting = false;
            if (movespeed > 10) {
                movespeed/=1.5f;
            }
        }
    }

    private void updateData() {
        groundCheck();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void groundCheck() {
        grounded = Physics.CheckSphere(gc.position, .2f, terrain);
    }

    private void updateStamina() {
        updateUI();

        if(sprinting) {
            currentStamina -= sprintCost*Time.deltaTime;
        } else if (!fullStamina) {
            currentStamina += regenRate*Time.deltaTime;
        }

        if (currentStamina >= maxStamina) {
            fullStamina = true;
            canvas.alpha = 0;
        } else {
            fullStamina = false;
            canvas.alpha = 1;
        }
    }

    private void updateUI() {
        staminaBar.fillAmount = (currentStamina/maxStamina);
    }
}
