using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost_Panel : MonoBehaviour
{
    [SerializeField] float multiplier;
    [SerializeField] Vector3 force;
    [SerializeField] AudioSource boost;
    void OnCollisionStay(Collision collision)
    {
        
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerMovement>().enabled == true)
        {
            StartCoroutine(ChangeMax(collision.gameObject));
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(rb.linearVelocity.normalized * multiplier + force);
            boost.Play();
        }
        else if (collision.gameObject.tag == "AI_Player")
        {
            boost.Play();
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
      
    }
    void OnTriggerStay(Collider collision)
    {

        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>().enabled == true)
            {
                StartCoroutine(ChangeMax(collision.gameObject));
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(rb.linearVelocity.normalized * multiplier);
                
            }
        }
        
    }
    void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerMovement>().enabled == true)
        {
            StartCoroutine(ChangeMax(collision.gameObject));
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(rb.linearVelocity.normalized * multiplier + -1 * force);
            boost.Play();
        }
        
    }
    void OnTriggerExit(Collider collision) { 
        
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerMovement>().enabled == true)
        {
            StartCoroutine(ChangeMax(collision.gameObject));
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(rb.linearVelocity.normalized * multiplier + 2f * force);
            boost.Play();
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
