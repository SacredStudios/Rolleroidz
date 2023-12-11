using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Ball : MonoBehaviour
{
    private const string PlayerTag = "Player";
    public static GameObject[] players;
    int num_bounces = 0;
    public GameObject explosion;
    public GameObject smoke;
    public float damage;
    public float power;
    public float radius;
    public float speed;
    public float range;
    public PhotonView pv;
    public GameObject parent;
    [SerializeField] Rigidbody rb;
    [SerializeField] AudioSource sound;



    void Start()
    {
        Invoke("Explode", 3f);
        if (players == null)
        {
            players = GameObject.FindGameObjectsWithTag(PlayerTag);
            
                List<GameObject> playerList = new List<GameObject>(GameObject.FindGameObjectsWithTag(PlayerTag));

                playerList.RemoveAll(player =>
                {
                    PhotonView photonView = player.GetComponent<PhotonView>();
                    if (photonView == null) return true;

                    Photon.Realtime.Player owner = photonView.Owner;
                    Photon.Realtime.Player localPlayer = PhotonNetwork.LocalPlayer;

                    if (owner == null || localPlayer == null) return false;

                    return owner.GetPhotonTeam() == localPlayer.GetPhotonTeam();
                });
                players = playerList.ToArray();
            

        }
        rb.AddForce(transform.up * speed*8f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        sound.Play();
        Debug.Log("playing sound");
        PointToClosestPlayer();
        StartCoroutine(ApplyForwardForceAfterBounce());
        if(collision.gameObject.tag == "Player")
        {
            Explode();
        }
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.GetComponent<Player_Health>() != null)
            {
                hit.gameObject.GetComponent<Player_Health>().Add_Explosion(power, radius, this.transform.position.x, this.transform.position.y, this.transform.position.z);
                if (hit.gameObject.GetComponent<PhotonView>().Owner.GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
                {

                    if (hit.gameObject.GetComponent<Player_Health>().current_health - damage <= 0)
                    {
                        parent.GetComponentInParent<Super_Bar>().ChangeAmount(25);
                        
                    }
                    hit.gameObject.GetComponent<Player_Health>().Remove_Health(damage);
                }
            }
        }

        GameObject explosion_clone = PhotonNetwork.Instantiate(explosion.name, this.transform.position, this.transform.rotation);
        GameObject smoke_clone = PhotonNetwork.Instantiate(smoke.name, this.transform.position, this.transform.rotation);
        PhotonNetwork.Destroy(this.gameObject);
    }

    private void PointToClosestPlayer()
    {
        if (players == null || players.Length == 0)
        {
            return;
        }

        GameObject closestPlayer = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject player in players)
        {
            Vector3 directionToPlayer = player.transform.position - currentPosition;
            float dSqrToPlayer = directionToPlayer.sqrMagnitude;
            if (dSqrToPlayer < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToPlayer;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null)
        {
            Vector3 direction = (closestPlayer.transform.position - currentPosition).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;
        }
    }

    private IEnumerator ApplyForwardForceAfterBounce()
    {
        yield return new WaitForFixedUpdate();
        rb.velocity = new Vector3(0f, 5f, 0f);
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }
}