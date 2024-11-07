using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;


public class PlayerMovement : MonoBehaviourPunCallbacks //and taunting too
{

    Vector3 input;
    [SerializeField] Joystick joy;
    [SerializeField] HoldButton taunt_btn;
    Vector3 cam_Forward;
    Vector3 diff;
    public float maxSpeed; //current max speed
    public float maxSpeedBase; //base max speed
    public bool boostMode;
    bool hasLanded = false;
    [SerializeField] Camera playerCam;
    [SerializeField] GameObject playerCam_gameObject;
    [SerializeField] GameObject player_ui;
    [SerializeField] GameObject vcam;
    [SerializeField] public Rigidbody rb;
    [SerializeField] float speedMultiplier;
    [SerializeField] float currSpeed;
    [SerializeField] float sensitivity = 1f;
    float desiredRotation;
    Vector3 lastPos = Vector3.zero;
    Vector3 newRotation;
    float cineMachinePitch;
    float cineMachineYaw;
    float targetRot;
    public GameObject CinemachineTarget;



    [SerializeField] GameObject jump_pos;
    [SerializeField] float rayCastLength;
    [SerializeField] float extra_gravity;
    [SerializeField] float gravity_multiplier;
    [SerializeField] float max_gravity;
    [SerializeField] float min_gravity;
    [SerializeField] public int jumpStrength;
    [SerializeField] public bool canJump = true;
    [SerializeField] Animator animator;
    [SerializeField] PhotonView pv;
    Vector3 last_velocity;
    [SerializeField] float acc_multiplier;
    [SerializeField] float time_airborne;
    [SerializeField] GameObject trick;
    [SerializeField] public static bool trick_mode_activated = false;
    public static bool taunt_mode_activated = false;
    [SerializeField] GameObject crosshair; //disappears when taunting
    private Weapon_Handler wh;
    //Sound Effects
    [SerializeField] AudioSource skating_sound;
    [SerializeField] AudioSource offground_sound;
    [SerializeField] AudioSource landing_sound;
    public bool onRail;
    [SerializeField] public static List<GameObject> landing_list;
    void Start()
    {
        maxSpeedBase = maxSpeed;
        targetRot = transform.eulerAngles.z;
        wh = this.gameObject.GetComponent<Weapon_Handler>();
        if (!pv.IsMine)
        {
            playerCam_gameObject.SetActive(false);
            player_ui.SetActive(false);
            vcam.SetActive(false);
            this.gameObject.GetComponent<PlayerMovement>().enabled = false;
        }
        else
        {
            landing_list = new List<GameObject>();
        }
        sensitivity = PlayerPrefs.GetFloat("mouse_sensitivity");
    }
    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            if (!onRail)
            {
                Gravity();
                if (!taunt_mode_activated)
                {
                    Move();
                }
            }
        }
    }
    void Gravity()
    {
        float acceleration = ((Mathf.Abs(rb.linearVelocity.x - last_velocity.x) + Mathf.Abs(rb.linearVelocity.y - last_velocity.y) +
        Mathf.Abs(rb.linearVelocity.z - last_velocity.z)) * acc_multiplier) / Time.deltaTime;
        last_velocity = rb.linearVelocity;
        if (Physics.Raycast(jump_pos.transform.position, Vector3.down, rayCastLength))
        {
            if (!taunt_mode_activated)
            {
                wh.weapon = wh.temp_weapon;
            }

            time_airborne = 0f;
            extra_gravity = min_gravity;
            canJump = true;
            maxSpeed = maxSpeedBase;
            animator.SetFloat("IsJumping", 0f);
            animator.speed = 1f + acceleration;
            offground_sound.Play();
        }
        else
        {
            time_airborne += Time.deltaTime;           
            maxSpeed = maxSpeedBase / 2f;
            canJump = false;
            animator.SetFloat("IsJumping", 1f);
            animator.speed = 1f;
        }

        if (extra_gravity < max_gravity)
        {
            extra_gravity += gravity_multiplier;
        }
        rb.AddForce(0, -extra_gravity, 0);
    }
    private void Update()
    {
        if (pv.IsMine) {
            if ((Input.GetButton("Fire2") || taunt_btn.isDown) && !onRail)
            {
                if (canJump)
                {
                    taunt_mode_activated = true;
                    animator.speed = 1f;
                    animator.SetFloat("Bend", 0f);
                    Debug.Log("taunting");
                    crosshair.SetActive(false);
                }
                else
                {
                    if (!trick_mode_activated && time_airborne > 1.5f)
                    {
                        Trick_System ts = trick.GetComponent<Trick_System>();
                        ts.Start_Trick_System();
                        trick_mode_activated = true;
                        Debug.Log("trick");
                    }
                }

            }
            if (!taunt_mode_activated)
            {
                if (!onRail)
                {
                    RotateWCamera();
                }
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
            }
            else
            {
                animator.SetBool("tauntModeActivated", true);
                wh.weapon = null;
                animator.SetLayerWeight(2, 0f);
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    animator.SetBool("tauntModeActivated", false);
                    taunt_mode_activated = false;
                    crosshair.SetActive(true);
                }
            }
            

        }
    }
    private void LateUpdate()
    {
        if (pv.IsMine)           
        {                      
            CameraRotation();
        }
    }
   
    void Move()
    {
        
        if (Ragdoll.is_Ragdoll == false && !boostMode && !onRail)
        {
            input.x = Input.GetAxis("Horizontal");           
            input.z = Input.GetAxis("Vertical");
            if(joy.Horizontal != 0 && joy.Vertical != 0)
            {
                input.x = joy.Horizontal;
                input.z = joy.Vertical;
            }
            animator.SetFloat("inputX", input.x);
            animator.SetFloat("inputZ", input.z);

            Vector3 inputDirection = new Vector3(input.x, 0, input.z);
            inputDirection = transform.TransformDirection(inputDirection);

            if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
            {
                input.x = 0;
                rb.linearVelocity = new Vector3(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            }

            if (Mathf.Abs(rb.linearVelocity.z) > maxSpeed)
            {
                input.z = 0;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, Mathf.Sign(rb.linearVelocity.z) * maxSpeed);
            }

            currSpeed = (transform.position - lastPos).magnitude;
            lastPos = transform.position;

            rb.AddForce(inputDirection * speedMultiplier * Time.deltaTime);

            if (currSpeed < 0.1)
            {
                animator.SetFloat("animSpeedCap", 0f);
            }
            else
            {
                animator.SetFloat("animSpeedCap", 1f);
            }
        }
    }
    bool IsJoystickArea(Vector2 touchPosition)
    {
        float joystickAreaSize = 0.2f; // 20% of the screen
        float screenWidth = Screen.width * joystickAreaSize;
        float screenHeight = Screen.height * joystickAreaSize;

        // Define the joystick area
        Rect joystickArea = new Rect(0, Screen.height - screenHeight, screenWidth, screenHeight);

        // Check if the touch position is within the joystick area
        return joystickArea.Contains(touchPosition);
    }
    public void Jump()
    {
        if (canJump && !onRail)
        {
            onRail = false;
            rb.AddForce(Vector3.up * jumpStrength);
            canJump = false;
            hasLanded = false;
            extra_gravity = min_gravity;

        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(jump_pos.transform.position, Vector3.down, rayCastLength)) //IsJumping
        {
            landing_list.Add(collision.gameObject);
            Debug.Log(landing_list);
            animator.SetFloat("IsJumping", 0f);
            if (hasLanded == false) {
                Land();
                hasLanded = true;
        }
            trick_mode_activated = false;

        }
    }



    void RotateWCamera()
    {
        float cameraYaw = playerCam.transform.eulerAngles.y;

        transform.rotation = Quaternion.Euler(0, cameraYaw, 0);
    }

    void CameraRotation()
    {
        //IF DEVICE IS ON MOBILE REMEMBER TO MULTIPLY BY TIME.DELTATIME
        if (Application.isMobilePlatform)
        {
            foreach (Touch touch in Input.touches)
            {
                // Ensure the touch is not over a UI element or the joystick area
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId) && !IsJoystickArea(touch.position))
                {
                    // Only rotate camera if the touch phase is Moved
                    if (touch.phase == TouchPhase.Moved)
                    {
                        cineMachineYaw += touch.deltaPosition.x * sensitivity * Time.deltaTime;
                        cineMachinePitch += (-1) * touch.deltaPosition.y * sensitivity * Time.deltaTime;

                        // Clamp the pitch angle
                        cineMachinePitch = ClampAngle(cineMachinePitch, -30, 70);

                        // Update the camera rotation
                        CinemachineTarget.transform.rotation = Quaternion.Euler(cineMachinePitch, cineMachineYaw, 0.0f);
                    }
                }
                else
                {
                    input.x = joy.Horizontal;
                    input.z = joy.Vertical;
                }
            }
        }
        else
        {
            cineMachineYaw += Input.GetAxis("Mouse X") * sensitivity;
            cineMachinePitch += (-1) * Input.GetAxis("Mouse Y") * sensitivity;
            cineMachineYaw = ClampAngle(cineMachineYaw, float.MinValue, float.MaxValue);
            cineMachinePitch = ClampAngle(cineMachinePitch, -30, 70);
            animator.SetFloat("Bend", cineMachinePitch);

            CinemachineTarget.transform.rotation = Quaternion.Euler(cineMachinePitch, cineMachineYaw, 0.0f);
        }


    }
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
    //Sound Effects
    public void Skate()
    {
        if(skating_sound != null)
        skating_sound.Play();
    }
    public void Land()
    {
    if(landing_sound != null)
        landing_sound.Play();
    }

    public void Offground()
    {
        pv.RPC("SyncOffground", RpcTarget.All);

    }

}

