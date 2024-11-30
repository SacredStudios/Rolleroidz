using UnityEngine;
using System.Collections.Generic;

public class Black_Hole : MonoBehaviour
{
    [Header("Black Hole Settings")]
    public float basePullForce = 1000f; // The base strength of the pull
    public float pullRadius = 10000f; // The radius within which objects are pulled
    public float updateInterval = 0.5f; // Time interval to check for new objects (in seconds)

    private List<Rigidbody> targets; // List to store all Rigidbody components of tagged objects
    private float nextUpdateTime = 0f; // Timer to control update frequency

    void Start()
    {
        targets = new List<Rigidbody>();
    }

    void Update()
    {
        // Only check for new objects at specified intervals
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;

            // Check for newly instantiated objects
            CheckForNewObjects();
        }
    }

    void FixedUpdate()
    {
        foreach (Rigidbody rb in targets)
        {
            if (rb != null)
            {
                Vector3 direction = (transform.position - rb.position).normalized;
                float distance = Vector3.Distance(transform.position, rb.position);

                if (distance <= pullRadius)
                {
                    // Calculate pull strength using inverse square law
                    float pullStrength = 200 * basePullForce / Mathf.Pow(distance, 2);
                    rb.AddForce(direction * pullStrength, ForceMode.Acceleration);
                }
            }
        }
    }

    private void CheckForNewObjects()
    {
        // Find all objects with the tags "Player" and "AI_Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] aiPlayers = GameObject.FindGameObjectsWithTag("AI_Player");

        AddRigidbodies(players);
        AddRigidbodies(aiPlayers);
    }

    private void AddRigidbodies(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null && !targets.Contains(rb))
            {
                targets.Add(rb); // Add only if it’s not already in the list
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
