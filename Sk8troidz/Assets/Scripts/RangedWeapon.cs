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
    public float explosionRadius = 5f; // The radius of the explosion effect

    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
        Ray ray = new Ray(parent.transform.position, particle_pos.transform.up);
        RaycastHit hit;
        bool hasHit = Physics.SphereCast(ray, explosionRadius, out hit, range);

        PhotonNetwork.Instantiate(particle_trail.name, particle_pos.transform.position, particle_pos.transform.rotation);

        if (hasHit)
        {
            Instantiate(particle_explosion, explosion_pos.transform.position, particle_pos.transform.rotation);
            Vector3 explosionPoint = hit.point;
            Collider[] colliders = Physics.OverlapSphere(explosionPoint, explosionRadius);

            foreach (var hitCollider in colliders)
            {
                if (hitCollider.tag == "Player" && hitCollider.GetComponent<PhotonView>().Owner.GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
                {
                    ApplyDamage(hitCollider.gameObject, hitCollider.transform);
                }
            }

            PhotonNetwork.Instantiate(impact_explosion.name, hit.point, Quaternion.identity);
        }
    }

    private void ApplyDamage(GameObject target, Transform hitTransform)
    {
        Player_Health ph = target.GetComponent<Player_Health>();
        if (ph != null && ph.current_health > 0)
        {
            float damageToApply = CalculateDamage(target.transform.position, hitTransform.position);
            if (ph.current_health - damageToApply <= 0)
            {
                SpawnCoin(target, hitTransform);
                ph.PlayerLastHit(pv.ViewID);
                parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);
            }
            ph.Remove_Health(damageToApply);
        }
    }

    float CalculateDamage(Vector3 targetPosition, Vector3 explosionPosition)
    {
        // Simple distance-based damage calculation
        float distance = Vector3.Distance(targetPosition, explosionPosition);
        float damage = Mathf.Max(0, damage - distance); // Decrease damage with distance
        return damage;
    }

  

void SpawnCoin(GameObject dead_player, Transform trans)
    {
        Debug.Log(player.GetComponent<PhotonView>().Owner.NickName);
        Debug.Log(dead_player.GetComponent<PhotonView>().Owner.NickName);

        Debug.Log("This should happen only once");
        GameObject coin_clone = PhotonNetwork.Instantiate(coin.name, trans.position, Quaternion.identity);
        Debug.Log(coin_clone.name);
        coin_clone.GetComponent<Point>().player = this.player;
        coin_clone.GetComponent<Point>().value = (dead_player.GetComponent<PhotonView>().Owner.GetScore() / 2) + 1;


    }
}



        
       
