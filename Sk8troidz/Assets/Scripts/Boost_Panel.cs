using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost_Panel : MonoBehaviour
{
    [SerializeField] float multiplier;
    [SerializeField] Vector3 force;
    void OnCollisionStay(Collision collision)
    {
        Debug.Log("collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(rb.velocity.normalized * multiplier);
    }
    void OnTriggerStay(Collider collision)
    {
        Debug.Log("collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(rb.velocity.normalized * multiplier);
    }
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(rb.velocity.normalized * multiplier + -1*force);
    }
    void OnTriggerExit(Collider collision)
    {
        Debug.Log("collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(rb.velocity.normalized * multiplier + 2f*force);
    }
}
