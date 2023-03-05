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
    [SerializeField] Collider body_collider;
    [SerializeField] float rayCastLength;
    [SerializeField] float extra_gravity;
    [SerializeField] float gravity_multiplier;
    [SerializeField] float max_gravity;
    [SerializeField] float min_gravity;
    [SerializeField] int jumpStrength;
    [SerializeField] bool canJump = true;
    [SerializeField] Animator animator;
    [SerializeField] PhotonView pv;
    //ADD PLAYER LEANING ANIMATION

    /*private Crosshair m_Crosshair; //Do this instead of running GetComponent every frame
    private Crosshair Crosshair 
    {
        get
        {
            if (m_Crosshair == null)
            {
                m_Crosshair = GetComponentInChildren<Crosshair>();
            }
            return m_Crosshair;
        }
    }
    */
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
        if (Physics.Raycast(body_collider.transform.position, Vector3.down, rayCastLength))
        {
            extra_gravity = min_gravity;
            canJump = true;
            animator.SetFloat("IsJumping", 0f);
        }
        else
        {
            canJump = false;
            animator.SetFloat("IsJumping", 1f);
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
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(body_collider.transform.position, Vector3.down, rayCastLength)) //IsJumping
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
