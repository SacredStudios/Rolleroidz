using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Rotate : MonoBehaviour //for lobby camera
{
    [SerializeField] float amount;
   
    void FixedUpdate()
    {
        transform.Rotate(0, amount * Time.deltaTime, 0); 
    }
}
