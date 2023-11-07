using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Ball : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private static GameObject[] players;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Rigidbody rb;
    int num_bounces = 0;
    [SerializeField] int max_bounces;
    [SerializeField] GameObject Explosion;

    void Start()
    {
        if (players == null)
        {
            players = GameObject.FindGameObjectsWithTag(PlayerTag);
            if (players == null)
            {
                List<GameObject> playerList = new List<GameObject>(GameObject.FindGameObjectsWithTag(PlayerTag));

                playerList.RemoveAll(player =>
                    player.GetComponent<PhotonView>().Owner.GetPhotonTeam() == PhotonNetwork.LocalPlayer.GetPhotonTeam());

                players = playerList.ToArray();
            }

        }
        rb.AddForce(transform.up * moveSpeed*8f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PointToClosestPlayer();
        StartCoroutine(ApplyForwardForceAfterBounce());
        num_bounces++;
        if(num_bounces>max_bounces)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
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
        rb.AddForce(transform.forward * moveSpeed, ForceMode.Impulse);
    }
}