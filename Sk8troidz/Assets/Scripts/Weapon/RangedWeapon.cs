using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

[CreateAssetMenu(fileName = "new_weapon", menuName = "Scripts/RangedWeapon", order = 1)]
public class RangedWeapon : Weapon
{
    public GameObject particle_trail;
    public GameObject particle_explosion;
    public GameObject impact_explosion;

    // Assumed inherited or declared elsewhere:
    // - float min_distance
    // - float damage
    // - float range
    // - GameObject death_effect
    // - GameObject player;
    // - void SpawnCoin(GameObject target, Vector3 position);

    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
        // Instantiate visual effects for muzzle flash and explosion (if any)
        PhotonNetwork.Instantiate(particle_trail.name, particle_pos.transform.position, particle_pos.transform.rotation);
        Instantiate(particle_explosion, explosion_pos.transform.position, particle_pos.transform.rotation);

        // Set up the main ray from the player's position along the particle's forward/up direction.
        float radius = 0.7f;
        Ray ray = new Ray(parent.transform.position + new Vector3(radius / 2f, 0, 0), particle_pos.transform.up);

        // Check if the shot should be blocked very close to the player (optional logic)
        Ray distanceRay = new Ray(player.transform.position + new Vector3(radius / 2f, 0, 0), particle_pos.transform.up);
        if (Physics.Raycast(distanceRay, min_distance))
        {
            // Optionally do nothing if there's an immediate obstruction.
            return;
        }

        // Try to get a hit using the primary, smaller sphere radius.
        RaycastHit? validHit = SphereCastIgnoreKinematic(ray, radius, range);

        // If no hit was found, try again with a larger sphere radius.
        if (!validHit.HasValue)
        {
            validHit = SphereCastIgnoreKinematic(ray, radius * 2f, range);
        }

        // Process the first valid hit found (if any).
        if (validHit.HasValue)
        {
            RaycastHit hit = validHit.Value;
            if (hit.distance <= range)
            {
                // Instantiate impact explosion effect at the hit point.
                PhotonNetwork.Instantiate(impact_explosion.name, hit.point, Quaternion.identity);

                // Check if the hit is a body collider (non-head) of a target.
                if ((hit.collider.CompareTag("AI_Player") || hit.collider.CompareTag("Player")) &&
                    hit.collider.GetComponent<Team_Handler>().GetTeam() != player.GetComponent<Team_Handler>().GetTeam())
                {
                    Player_Health ph = hit.collider.GetComponent<Player_Health>();
                    if (ph != null && ph.current_health > 0)
                    {
                        // If damage is lethal, instantiate death effects and reposition the target.
                        if (ph.current_health - damage <= 0)
                        {
                            PhotonNetwork.Instantiate(death_effect.name, hit.point, Quaternion.identity);
                            hit.collider.transform.position = new Vector3(9999, 9999, 9999);
                            SpawnCoin(hit.transform.gameObject, hit.point);
                            parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);
                        }
                        ph.Remove_Health(damage);
                    }
                }
                // Check if the hit is a head collider (special case) regardless of the isTrigger flag.
                else if ((hit.collider.CompareTag("AI_Head") || hit.collider.CompareTag("Player_Head")) &&
                         hit.collider.GetComponentInParent<Team_Handler>().GetTeam() != player.GetComponent<Team_Handler>().GetTeam())
                {
                    Player_Health ph = hit.collider.GetComponentInParent<Player_Health>();
                    if (ph != null && ph.current_health > 0)
                    {
                        // A headshot deals increased damage.
                        if (ph.current_health - (damage * 1.5f) <= 0)
                        {
                            PhotonNetwork.Instantiate(death_effect.name, hit.point, Quaternion.identity);
                            hit.collider.transform.position = new Vector3(9999, 9999, 9999);
                            SpawnCoin(hit.transform.parent.gameObject, hit.point);
                            parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);
                        }
                        ph.Remove_Health(damage * 1.5f);
                    }
                    else
                    {
                        Debug.Log("too short");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Performs a sphere cast and returns the first hit that satisfies the following conditions:
    /// - If a collider's 'isTrigger' property is true, only allow it if its tag is "Player_Head" or "AI_Head".
    /// - If the collider is attached to a Rigidbody that is kinematic, ignore it unless it's a head collider.
    /// </summary>
    /// <param name="ray">The shooting ray.</param>
    /// <param name="radius">The sphere cast radius.</param>
    /// <param name="maxDistance">The maximum distance of the cast.</param>
    /// <param name="layerMask">Optional layer mask (default is all layers).</param>
    /// <returns>The first valid RaycastHit, or null if none is found.</returns>
    private RaycastHit? SphereCastIgnoreKinematic(Ray ray, float radius, float maxDistance, int layerMask = ~0)
    {
        // Use QueryTriggerInteraction.Collide so that trigger colliders are reported.
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, maxDistance, layerMask, QueryTriggerInteraction.Collide);
        if (hits.Length == 0)
            return null;

        // Sort the hits by distance, nearest first.
        System.Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

        // Loop through the hits and return the first valid one.
        foreach (RaycastHit hit in hits)
        {
            // Ignore any collider that is a trigger unless it is tagged as "Player_Head" or "AI_Head".
            if (hit.collider.isTrigger && hit.collider.tag != "Player_Head" && hit.collider.tag != "AI_Head")
                continue;

            // If the hit object has a Rigidbody and it is kinematic, skip it (unless it's a head collider).
            if (hit.rigidbody != null && hit.rigidbody.isKinematic &&
                hit.collider.tag != "Player_Head" && hit.collider.tag != "AI_Head")
                continue;

            return hit;
        }

        return null;  // No valid hit was found.
    }
}
