using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Ball : MonoBehaviour
{
    private const string PlayerTag = "Player";
    public static GameObject[] players;
    List<GameObject> playerList;
    int num_bounces = 0;
    //change this to just grab the weapon itself
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
    [SerializeField] Weapon weapon;
    



    void Start()
    {
        Invoke("Explode", 3f);
        if (players == null || players.Length == 0)
        {
            GameObject[] human_players = GameObject.FindGameObjectsWithTag(PlayerTag);
            GameObject[] ai_players = GameObject.FindGameObjectsWithTag("AI_Player");           
            players = human_players.Concat(ai_players).ToArray();

            playerList = new List<GameObject>(players);
            playerList.RemoveAll(player =>
            {
                PhotonView photonView = player.GetComponent<PhotonView>();
                if (photonView == null) return true;

                Photon.Realtime.Player owner = photonView.Owner;
                Photon.Realtime.Player localPlayer = PhotonNetwork.LocalPlayer;

                if (owner == null || localPlayer == null) return false;

                return weapon.player.GetComponent<Team_Handler>().GetTeam() == player.GetComponent<Team_Handler>().GetTeam();
            });
              
            players = playerList.ToArray();
            for (int i = 0; i < players.Length; i++)
            {
                Debug.Log(players[i] + " is the playerlist");
            }


        }
        rb.AddForce(transform.up * speed*4f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        sound.Play();
        PointToClosestPlayer();
        StartCoroutine(ApplyForwardForceAfterBounce());
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "AI_Player")
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
                if (hit.gameObject.GetComponent<Team_Handler>().GetTeam() != weapon.player.GetComponent<Team_Handler>().GetTeam())
                {
                    if (hit.gameObject.GetComponent<Player_Health>().current_health - damage <= 0)
                    {
                        parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);
                        weapon.SpawnCoin(hit.GetComponent<Collider>().gameObject, hit.transform.position);

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
            Debug.Log("glitch");
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
            Debug.Log(closestPlayer);
            Vector3 direction = (closestPlayer.transform.position - currentPosition).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;
        }
    }

    private IEnumerator ApplyForwardForceAfterBounce()
    {
        yield return new WaitForFixedUpdate();
        rb.linearVelocity = new Vector3(0f, 5f, 0f);
        rb.AddForce(transform.forward * speed*2f, ForceMode.Impulse);
    }
}