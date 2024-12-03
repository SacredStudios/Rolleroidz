using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class Black_Hole : MonoBehaviour
{
    [Header("Black Hole Settings")]
    public Weapon weapon;
    [SerializeField] float basePullForce = 1000f; // The base strength of the pull
    [SerializeField] float pullRadius = 50f; // The radius within which objects are pulled
    [SerializeField] float pulseSpeed = 5f; // Speed of pulsating effect
    [SerializeField] float pulseAmplitude = 0.5f; // Amplitude of the pulsating effect
    [SerializeField] float updateInterval = 0.5f; // Time interval to check for new objects (in seconds)
    [SerializeField] Vector3 offset;
    public GameObject deathEffect; // Assign this in the inspector

    public PhotonView pv;
    public GameObject player;
    private Weapon temp;
    private Weapon_Handler weaponHandler;

    private Vector3 originalScale; // Original scale of the black hole
    private List<GameObject> targets;
    private float nextUpdateTime;


    void Start()
    {
        targets = new List<GameObject>();
        CheckForNewObjects();

        // Save the current weapon and disable shooting
        weaponHandler = player.GetComponent<Weapon_Handler>();
        temp = weaponHandler.weapon;
        weaponHandler.weapon = null;

        // Activate black hole for 10 seconds
        Invoke(nameof(Explode), 10f);

        // Save the original scale
        originalScale = transform.localScale;
        transform.position += offset;
    }

    void Update()
    {
        // Periodically check for new objects
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            CheckForNewObjects();
        }

        // Pulsating effect: dynamically change the scale of the black hole
        float scaleModifier = Mathf.Lerp(1, 1.2f, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);
        transform.localScale = originalScale * scaleModifier;

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
                    // Apply pull force
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pullRadius);
        foreach (Collider hit in hitColliders)
        {
            if (targets.Contains(hit.gameObject))
            {
                Player_Health ph = hit.GetComponent<Player_Health>();
                if (ph != null && ph.current_health > 0)
                {
                    if (ph.current_health - 100 <= 0)
                    {
                        // Handle player death
                        PhotonNetwork.Instantiate(deathEffect.name, hit.transform.position, Quaternion.identity);
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
        PhotonNetwork.Destroy(gameObject); // Destroy the black hole object
    }
}
