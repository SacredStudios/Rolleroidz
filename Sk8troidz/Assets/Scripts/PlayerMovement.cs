using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    Vector3 input;
    Vector3 mousePos;
    Vector3 diff;
    [SerializeField] Camera playerCam;
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    Vector3 newRotation;
    // Update is called once per frame
    void Update()
    {   //Player movement
        input.x = Input.GetAxis("Horizontal");
        input.z = Input.GetAxis("Vertical");
        rb.AddRelativeForce(input*speed);
        Vector3 newRotation = new Vector3(transform.eulerAngles.x, playerCam.gameObject.transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Euler(newRotation);
    }
}
