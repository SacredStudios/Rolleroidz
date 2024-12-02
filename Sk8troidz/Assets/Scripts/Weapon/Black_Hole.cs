using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class Black_Hole : MonoBehaviour
{
    [Header("Black Hole Settings")]
    public Weapon weapon;
    [SerializeField] float basePullForce = 1000f; // The base strength of the pull
    [SerializeField] float pullRadius = 10000f; // The radius within which objects are pulled
    [SerializeField] float updateInterval = 0.5f; // Time interval to check for new objects (in seconds)
    public static GameObject[] players;
    private List<GameObject> targets;
    private float nextUpdateTime = 0f; // Timer to control update frequency
    public PhotonView pv;
    public GameObject player;
    private Weapon temp;
    private Weapon_Handler weaponHandler;

    public GameObject deathEffect; // Assign this in the inspector

    void Start()
    {
        targets = new List<GameObject>();
        CheckForNewObjects();

        // Save the current weapon and disable shooting
        weaponHandler = player.GetComponent<Weapon_Handler>();
        temp = weaponHandler.weapon;
        weaponHandler.weapon = null;

        // Activate black hole for 30 seconds
        Invoke(nameof(Explode), 10f);
    }

    void Update()
    {
        // Periodically check for new objects
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            CheckForNewObjects();
        }
    }

    void FixedUpdate()
    {
        // Apply pull force to all valid targets
        foreach (GameObject obj in targets)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (transform.position - rb.position).normalized;
                float distance = Vector3.Distance(transform.position, rb.position);

                if (distance <= pullRadius)
                {
                    // Calculate pull strength using inverse square law
                    float pullStrength = 2 * basePullForce / Mathf.Pow(distance, 2);
                    rb.AddForce(direction * pullStrength, ForceMode.Acceleration);
                }
            }
        }

        // Check for collisions with the black hole's pull area
        ApplyBlackHoleEffects();
    }

    private void ApplyBlackHoleEffects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3);
        foreach (Collider hit in hitColliders)
        {
            if (targets.Contains(hit.gameObject))
            {
                Player_Health ph = hit.GetComponent<Player_Health>();
                if (ph != null && ph.current_health > 0)
                {
                    if (ph.current_health - 100 <= 0)
                    {
                        // Instantiate death effect and handle death logic
                        // PhotonNetwork.Instantiate(deathEffect.name, hit.transform.position, Quaternion.identity);
                        Debug.Log("someone got zucced");
                        Transform oldPos = hit.transform;
                        hit.transform.position = new Vector3(9999, 9999, 9999);

                        temp.SpawnCoin(hit.gameObject, hit.transform.position);
                    }
                    ph.Remove_Health(100);
                }
            }
        }
    }

    private void CheckForNewObjects()
    {
        // Find and filter targets
        targets.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        targets.AddRange(GameObject.FindGameObjectsWithTag("AI_Player"));

        targets.RemoveAll(currPlayer =>
        {
            PhotonView photonView = currPlayer.GetComponent<PhotonView>();
            if (photonView == null) return true;

            Photon.Realtime.Player owner = photonView.Owner;
            Photon.Realtime.Player localPlayer = PhotonNetwork.LocalPlayer;

            if (owner == null || localPlayer == null) return false;

            // Exclude players on the same team
            return player.GetComponent<Team_Handler>().GetTeam() == currPlayer.GetComponent<Team_Handler>().GetTeam();
        });
    }

    public void Explode()
    {
        // Restore weapon functionality after the black hole is deactivated
        weaponHandler.weapon = temp;

        // Optionally add an explosion effect here
        Debug.Log("Black Hole effect ended.");
        Destroy(gameObject); // Destroy the black hole object
    }
}
