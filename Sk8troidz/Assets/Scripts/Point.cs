using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
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
        transform.Rotate(Vector3.forward * speed * 15 * Time.deltaTime);
        if (player != null)
        {

            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }
    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (player != null)
            {
                collider.gameObject.GetComponent<PhotonView>().Owner.AddScore(1);
                PhotonNetwork.Destroy(this.gameObject);
                
            }



        }

    }

}
