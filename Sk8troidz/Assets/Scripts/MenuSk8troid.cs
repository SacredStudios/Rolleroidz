using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuSk8troid : MonoBehaviour
{
    public float rotationSpeed = 100.0f;
    [SerializeField] Camera mainCamera;
    [SerializeField] Animator animator;
    bool isRunning = false;
    
    bool canSpin;

    private void Start()
    {
        this.gameObject.transform.rotation = new Quaternion(0, -0.6229886f, 0, -0.1f);
    }
    public void Spin_Enabled()
    {
        canSpin = true;
    }
    void Update()
    {
        if (Input.GetMouseButton(0) && canSpin)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform) // Check if the raycast hit this GameObject
                {
                    float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                    transform.Rotate(Vector3.up, mouseX);
                }
            }
        }
        if (isRunning)
        {
            this.gameObject.transform.position += this.gameObject.transform.forward * 9.0f * Time.deltaTime;
        }
    }
    public void StartAnim()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            canSpin = false;
            this.gameObject.transform.rotation = new Quaternion(0, -0.6229886f, 0, -0.1f);
            animator.SetFloat("animSpeedCap", 1);
            animator.speed = 2;
            isRunning = true;
        }
        
    }
}

