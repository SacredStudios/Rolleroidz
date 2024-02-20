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
    public GameObject dead_player;
    [SerializeField] float speed;
    [SerializeField] PhotonView pv;
    public int value;
    float delay = 0;
    [SerializeField] GameObject effect;


    void Update()
    {
        if (delay < 0.5f)
        {
            delay += Time.deltaTime;
        }
        transform.Rotate(Vector3.forward * speed * 15 * Time.deltaTime);
        if (player != null)
        {

            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
       
    }
    void OnCollisionStay(Collision collider) 
    {
        
        if (collider.gameObject.tag == "Player" && delay >= 0.5f && collider.gameObject != dead_player)
        {
            Instantiate(effect, this.transform.position, Quaternion.identity);
            if (player != null)
            {
                Debug.Log(value);
                collider.gameObject.GetComponent<PhotonView>().Owner.AddScore(value);
                GetComponent<CapsuleCollider>().enabled = false;
                PhotonNetwork.Destroy(this.gameObject);


            }



        }

    }
    

}
