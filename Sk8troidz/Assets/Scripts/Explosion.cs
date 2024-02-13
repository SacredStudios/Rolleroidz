using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class Explosion : MonoBehaviour
{

    public GameObject explosion;
    public GameObject smoke;
    public float damage;
    public float power;
    public float radius;
    public float speed;
    public PhotonView pv;
    public Weapon weapon;
    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * speed);
    }
    private void OnCollisionEnter(Collision collision)
    {

            Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius);
            foreach (Collider hit in colliders)
            {
            Player_Health ph = hit.GetComponent<Player_Health>();
            if (ph != null)
                {
                ph.Add_Explosion(power, radius, this.transform.position.x, this.transform.position.y, this.transform.position.z);
                if (hit.gameObject.GetComponent<PhotonView>().Owner.GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
                {

                    ph.Remove_Health(damage);
                    if (ph.current_health > 0 && ph.current_health - damage <= 0)
                    {
                        weapon.SpawnCoin(hit.GetComponent<Collider>().gameObject, hit.transform);
                        ph.PlayerLastHit(pv.ViewID);
                    }
                }
                }
            }
            
        GameObject explosion_clone = PhotonNetwork.Instantiate(explosion.name, this.transform.position, this.transform.rotation);
        GameObject smoke_clone = PhotonNetwork.Instantiate(smoke.name, this.transform.position, this.transform.rotation);
        PhotonNetwork.Destroy(this.gameObject);
    }

}
