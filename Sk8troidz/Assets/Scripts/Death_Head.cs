using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Head : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 launch_force;
    void OnEnable()
    {
        rb.AddForce(launch_force);
        rb.AddTorque(0, 120, 90);
        Invoke("Destroy_Me", 2.5f);

    }
    void Destroy_Me()
    {
        Destroy(this.gameObject);
    }

   
}
