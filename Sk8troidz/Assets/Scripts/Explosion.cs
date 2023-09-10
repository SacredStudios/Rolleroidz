using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explosion : MonoBehaviour
{

    public GameObject explosion;
    public GameObject smoke;
    public float damage;
    public float power;
    public float radius;
    public float speed;
    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * speed);
    }
    private void OnCollisionEnter(Collision collision)
    {
 // && collision.gameObject.GetComponent<PhotonView>().Owner.GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())

            Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius);
            foreach (Collider hit in colliders)
            {
                if (hit.gameObject.GetComponent<Player_Health>() != null)
                {
                    Debug.Log("hit target");
                    hit.gameObject.GetComponent<Player_Health>().Remove_Health(damage);
                    hit.gameObject.GetComponent<Player_Health>().Add_Explosion(power, radius, this.transform.position.x, this.transform.position.y, this.transform.position.z);
                }
            }
            
        GameObject explosion_clone = Instantiate(explosion, this.transform.position, this.transform.rotation);
        explosion_clone.transform.localScale += new Vector3(10, 10, 10);
        GameObject smoke_clone = Instantiate(smoke, this.transform.position, this.transform.rotation);
        smoke_clone.transform.localScale += new Vector3(10, 10, 10);
        PhotonNetwork.Destroy(this.gameObject);
    }

}
