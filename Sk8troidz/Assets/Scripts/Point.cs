using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Point : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int max;
    [SerializeField] int min;
    [SerializeField] int y_val;
    public GameObject player;
    [SerializeField] float speed;

    private void Start()
    {
        int x = Random.Range(min, max);
        int z = Random.Range(min, max);
        rb.AddForce(new Vector3(x, y_val, z));
    }

    void Update()
    {
        transform.Rotate(Vector3.left * speed * 15 * Time.deltaTime);
        if (player != null)
        {
            // Added Debugging
            Debug.Log("Rotating and Moving Coin");

            // Rotating the coin around the y-axis (up) as an example
            

            // Moving the coin towards the player
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

}
