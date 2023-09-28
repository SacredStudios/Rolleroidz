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
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(ChangeMax(collision.gameObject));
        }
        rb.AddForce(rb.velocity.normalized * multiplier);
    }
    void OnTriggerStay(Collider collision)
    {
        Debug.Log("collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(ChangeMax(collision.gameObject));
        }
        rb.AddForce(rb.velocity.normalized * multiplier);
    }
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(ChangeMax(collision.gameObject));
        }
        rb.AddForce(rb.velocity.normalized * multiplier + -1*force);
    }
    void OnTriggerExit(Collider collision)
    {
        Debug.Log("collided");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(ChangeMax(collision.gameObject));
        }
        rb.AddForce(rb.velocity.normalized * multiplier + 2f*force);

    }
    IEnumerator ChangeMax(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerMovement>().maxSpeed *= 1000f;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<PlayerMovement>().maxSpeed = gameObject.GetComponent<PlayerMovement>().maxSpeedBase;
    }
    
}
