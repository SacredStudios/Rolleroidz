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
    [SerializeField] float maxSpeed;
    [SerializeField] Camera playerCam;
    [SerializeField] GameObject playerCam_gameObject;
    [SerializeField] GameObject player_ui;
    [SerializeField] GameObject vcam;
    [SerializeField] Rigidbody rb;
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
    [SerializeField] int jumpStrength;
    [SerializeField] bool canJump = true;
    [SerializeField] Animator animator;
    [SerializeField] PhotonView pv;
    Vector3 last_velocity;
    [SerializeField] float acc_multiplier;
    [SerializeField] float time_airborne;
    [SerializeField] GameObject trick;
    [SerializeField]  bool trick_mode_activated = false;
    //ADD PLAYER LEANING ANIMATION

  
    void Start()
    {
        targetRot = transform.eulerAngles.z;
        if(!pv.IsMine)
        {
            playerCam_gameObject.SetActive(false);
            player_ui.SetActive(false);
            vcam.SetActive(false);
        }
    }
    private void FixedUpdate()
    {  if (pv.IsMine)
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
            trick_mode_activated = false;
            time_airborne = 0f;
            extra_gravity = min_gravity;
            canJump = true;
            animator.SetFloat("IsJumping", 0f);
            animator.speed = 1f + acceleration;
        }
        else
        {
            time_airborne += Time.deltaTime;
            if(time_airborne > 2f && !trick_mode_activated)
            {
                Trick_System ts = trick.GetComponent<Trick_System>();
                ts.Start_Trick_System();
                trick_mode_activated = true;
            }
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
        if (pv.IsMine)
        Jump();    
    }
    private void LateUpdate()
    {
        if (pv.IsMine)
        { 
          RotateWCamera();
          CameraRotation();
        }
    }
    void Move()
    {
        input.x = Input.GetAxis("Horizontal");
        input.z = Input.GetAxis("Vertical");
        currSpeed = (transform.position - lastPos).magnitude;
        lastPos = transform.position;
        if (currSpeed < maxSpeed)
        {
            rb.AddRelativeForce(input * speedMultiplier * Time.deltaTime);   
        }
        if (currSpeed < 0.1)
        {
            animator.SetFloat("animSpeedCap", 0f);
        }
        else
        {
            animator.SetFloat("animSpeedCap", 1f);
        }
        
    }
    void Jump()
    { 
        if(Input.GetButtonDown("Jump") && canJump)
        {           
            rb.AddForce(Vector3.up * jumpStrength);                               
            canJump = false;
            trick_mode_activated = true;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(jump_pos.transform.position, Vector3.down, rayCastLength)) //IsJumping
        {
            animator.SetFloat("IsJumping", 0f);
        
        }
        
    }
    

   
    void RotateWCamera()
    {
        Vector3 newRotation = new Vector3(transform.eulerAngles.x, playerCam.gameObject.transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Euler(newRotation);
       
    }
    void CameraRotation()
    {
        //IF DEVICE IS ON MOBILE REMEMBER TO MULTIPLY BY TIME.DELTATIME
        cineMachineYaw += Input.GetAxis("Mouse X")*sensitivity; 
        cineMachinePitch += (-1)*Input.GetAxis("Mouse Y")*sensitivity ;
       


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
   
}
