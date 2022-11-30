using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject player;
    [SerializeField] Vector3 offset;
    [SerializeField] float sensitivity;
    [SerializeField] Vector2 yBoundaries;
    [SerializeField] Vector2 xBoundaries;
    public float testRot;
    // Update is called once per frame
    void Update()
    {


        float verticalInput = Input.GetAxis("Mouse Y") * -1*sensitivity * Time.deltaTime;
        float horizontalInput = Input.GetAxis("Mouse X") * -1*sensitivity * Time.deltaTime;


       

        if ((transform.localEulerAngles.x >= 300 && transform.localEulerAngles.x <= 360) || (transform.localEulerAngles.x >= 0 && transform.localEulerAngles.x <= 60))
        {
           // transform.Rotate(Vector3.down, horizontalInput, Space.World);
           transform.Rotate(Vector3.right, verticalInput);
        }
    else
        {
            Debug.Log(transform.localEulerAngles);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        transform.position = player.transform.position;
    }
    private void Start()
    {
        
            
        
    }
}
