using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPunCallbacks
{

    Vector3 input;
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
    private Weapon_Handler wh;
    //ADD PLAYER LEANING ANIMATION
    //Sound Effects
    [SerializeField] AudioSource skating_sound;
    [SerializeField] AudioSource offground_sound;
    [SerializeField] AudioSource landing_sound;


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
        sensitivity = PlayerPrefs.GetFloat("mouse_sensitivity");
    }
    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            Move();
            Gravity();
        }


    }
    void Gravity()
    {
        float acceleration = ((Mathf.Abs(rb.velocity.x - last_velocity.x) + Mathf.Abs(rb.velocity.y - last_velocity.y) +
        Mathf.Abs(rb.velocity.z - last_velocity.z)) * acc_multiplier) / Time.deltaTime;
        last_velocity = rb.velocity;
        if (Physics.Raycast(jump_pos.transform.position, Vector3.down, rayCastLength))
        {
            wh.weapon = wh.temp_weapon;

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
            if (time_airborne > 1.5f && !trick_mode_activated)
            {
                Trick_System ts = trick.GetComponent<Trick_System>();
                ts.Start_Trick_System();
                trick_mode_activated = true;
            }
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
            RotateWCamera();
            Jump();
        
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
        if (Ragdoll.is_Ragdoll == false && !boostMode)
        {
            input.x = Input.GetAxis("Horizontal");           
            input.z = Input.GetAxis("Vertical");
            animator.SetFloat("inputX", input.x);
            animator.SetFloat("inputZ", input.z);

            Vector3 inputDirection = new Vector3(input.x, 0, input.z);
            inputDirection = transform.TransformDirection(inputDirection);

            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            {
                input.x = 0;
                rb.velocity = new Vector3(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y, rb.velocity.z);
            }

            if (Mathf.Abs(rb.velocity.z) > maxSpeed)
            {
                input.z = 0;
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Mathf.Sign(rb.velocity.z) * maxSpeed);
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
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            rb.AddForce(Vector3.up * jumpStrength);
            canJump = false;
            trick_mode_activated = true;
            hasLanded = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(jump_pos.transform.position, Vector3.down, rayCastLength)) //IsJumping
        {
            animator.SetFloat("IsJumping", 0f);
            if (hasLanded == false) {
                Land();
                Debug.Log("Land");
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

        cineMachineYaw += Input.GetAxis("Mouse X") * sensitivity;
        cineMachinePitch += (-1) * Input.GetAxis("Mouse Y") * sensitivity;



        cineMachineYaw = ClampAngle(cineMachineYaw, float.MinValue, float.MaxValue);
        cineMachinePitch = ClampAngle(cineMachinePitch, -30, 70);
        animator.SetFloat("Bend", cineMachinePitch);

        CinemachineTarget.transform.rotation = Quaternion.Euler(cineMachinePitch, cineMachineYaw, 0.0f);


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
        skating_sound.Play();
    }
    public void Land()
    {
        landing_sound.Play();
    }

    public void Offground()
    {
        pv.RPC("SyncOffground", RpcTarget.All);

    }

}

