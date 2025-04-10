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


    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
        float radius = 0.3f;
        Ray ray = new Ray(parent.transform.position, particle_pos.transform.up);
        PhotonNetwork.Instantiate(particle_trail.name, particle_pos.transform.position, particle_pos.transform.rotation);
        Instantiate(particle_explosion, explosion_pos.transform.position, particle_pos.transform.rotation);

        // This ray is used to check min_distance from player's side
        Ray distance = new Ray(player.transform.position + new Vector3(radius / 2, 0, 0), particle_pos.transform.up);
        if (Physics.Raycast(distance, min_distance))
        {
            // If min_distance is too short, do nothing or fix your "shoot from center" logic
        }
        else
        {
            // 1) Attempt SphereCastIgnoreKinematic with the "normal" radius
            RaycastHit? possibleHit = SphereCastIgnoreKinematic(ray, radius, range);

            if (possibleHit.HasValue)
            {
                RaycastHit hit = possibleHit.Value;
                if (hit.distance <= range)
                {
                    PhotonNetwork.Instantiate(impact_explosion.name, hit.point, Quaternion.identity);

                    // Example damage/target logic
                    if ((hit.collider.CompareTag("AI_Player") || hit.collider.CompareTag("Player")) &&
                        hit.collider.GetComponent<Team_Handler>().GetTeam() != player.GetComponent<Team_Handler>().GetTeam())
                    {
                        Player_Health ph = hit.collider.GetComponent<Player_Health>();
                        if (ph != null && ph.current_health > 0)
                        {
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
                    else if ((hit.collider.CompareTag("AI_Head") || hit.collider.CompareTag("Player_Head")) &&
                             hit.collider.GetComponentInParent<Team_Handler>().GetTeam() != parent.GetComponent<Team_Handler>().GetTeam())
                    {
                        Player_Health ph = hit.collider.GetComponentInParent<Player_Health>();
                        if (ph != null && ph.current_health > 0)
                        {
                            if (ph.current_health - (damage * 1.5f) <= 0)
                            {
                                PhotonNetwork.Instantiate(death_effect.name, hit.point, Quaternion.identity);
                                hit.collider.transform.position = new Vector3(9999, 9999, 9999);
                                SpawnCoin(hit.transform.parent.gameObject, hit.point);
                                parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);
                            }
                            ph.Remove_Health(damage * 1.5f);
                        }
                    }
                }
            }
            else
            {
                // 2) If the first cast didn't hit a valid non-kinematic object, try with bigger radius
                RaycastHit? secondHit = SphereCastIgnoreKinematic(ray, radius * 2, range);

                if (secondHit.HasValue)
                {
                    RaycastHit hit = secondHit.Value;
                    if (hit.distance <= range)
                    {
                        PhotonNetwork.Instantiate(impact_explosion.name, hit.point, Quaternion.identity);

                        // Example damage/target logic
                        if ((hit.collider.CompareTag("AI_Player") || hit.collider.CompareTag("Player")) &&
                            hit.collider.GetComponent<Team_Handler>().GetTeam() != parent.GetComponent<Team_Handler>().GetTeam())
                        {
                            Player_Health ph = hit.collider.GetComponent<Player_Health>();
                            if (ph != null && ph.current_health > 0)
                            {
                                if (ph.current_health - (damage / 2) <= 0)
                                {
                                    PhotonNetwork.Instantiate(death_effect.name, hit.point, Quaternion.identity);
                                    hit.collider.transform.position = new Vector3(9999, 9999, 9999);
                                    SpawnCoin(hit.transform.gameObject, hit.point);
                                    parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);
                                }
                                ph.Remove_Health(damage / 2);
                            }
                        }
                        else if ((hit.collider.CompareTag("AI_Head") || hit.collider.CompareTag("Player_Head")) &&
                                 hit.collider.GetComponentInParent<Team_Handler>().GetTeam() != parent.GetComponent<Team_Handler>().GetTeam())
                        {
                            Player_Health ph = hit.collider.GetComponentInParent<Player_Health>();
                            if (ph != null && ph.current_health > 0)
                            {
                                if (ph.current_health - (damage / 2) <= 0)
                                {
                                    PhotonNetwork.Instantiate(death_effect.name, hit.point, Quaternion.identity);
                                    hit.collider.transform.position = new Vector3(9999, 9999, 9999);
                                    SpawnCoin(hit.transform.parent.gameObject, hit.point);
                                    parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);
                                }
                                ph.Remove_Health(damage / 2);
                            }
                        }
                    }
                }
            }
        }
    }


    private RaycastHit? SphereCastIgnoreKinematic(Ray ray, float radius, float maxDistance, int layerMask = ~0)
    {
        // Grab all hits
        RaycastHit[] hits = Physics.SphereCastAll(
            ray,
            radius,
            maxDistance,
            layerMask,
            QueryTriggerInteraction.Ignore
        );

        if (hits.Length == 0)
            return null;

        // Sort by distance so we process the closest hits first
        System.Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

        // Loop through and ignore any that have a kinematic RB
        foreach (var h in hits)
        {
            if (h.collider.isTrigger && (h.collider.tag != "Player_Head" || h.collider.tag != "AI_Head")) {
                continue;
            }
            // If there's no rigidbody or it's not kinematic, this is our "real" hit
            Rigidbody rb = h.rigidbody;
            if (rb == null || rb.isKinematic == false)
            {
                return h;
            }
        }

        // If all hits had kinematic rigidbodies, return null
        return null;
    }

}
