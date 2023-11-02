using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost_Panel : MonoBehaviour
{
    [SerializeField] float multiplier;
    [SerializeField] Vector3 force;
    void OnCollisionStay(Collision collision)
    {
        
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerMovement>().enabled == true)
        {
            StartCoroutine(ChangeMax(collision.gameObject));
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(rb.velocity.normalized * multiplier + force);
        }
        
    }
    void OnTriggerStay(Collider collision)
    {

        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>().enabled == true)
            {
                StartCoroutine(ChangeMax(collision.gameObject));
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(rb.velocity.normalized * multiplier);
            }
        }
        
    }
    void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerMovement>().enabled == true)
        {
            StartCoroutine(ChangeMax(collision.gameObject));
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(rb.velocity.normalized * multiplier + -1 * force);
        }
        
    }
    void OnTriggerExit(Collider collision) { 
        
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerMovement>().enabled == true)
        {
            StartCoroutine(ChangeMax(collision.gameObject));
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(rb.velocity.normalized * multiplier + 2f * force);
        }
        

    }
    IEnumerator ChangeMax(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerMovement>().maxSpeed *= 1000f;
        gameObject.GetComponent<PlayerMovement>().boostMode = true;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<PlayerMovement>().maxSpeed = gameObject.GetComponent<PlayerMovement>().maxSpeedBase;
        gameObject.GetComponent<PlayerMovement>().boostMode = false;
    }
    
}
