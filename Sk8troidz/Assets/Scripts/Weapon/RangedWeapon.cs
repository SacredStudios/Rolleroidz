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
    public float _radius = 0.7f;

    protected virtual float Radius => _radius;

    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
        //  Debug.DrawRay(parent.transform.position, particle_pos.transform.up * range, Color.green); //chage this to capsulecast



        Ray ray = new Ray(parent.transform.position, particle_pos.transform.up); //-new Vector3(radius, 0, 0), 
        RaycastHit hit = new RaycastHit();
        PhotonNetwork.Instantiate(particle_trail.name, particle_pos.transform.position, particle_pos.transform.rotation);
        Instantiate(particle_explosion, explosion_pos.transform.position, particle_pos.transform.rotation);

        Ray distance = new Ray(player.transform.position + new Vector3(Radius / 2, 0, 0), particle_pos.transform.up);

        if (Physics.Raycast(distance, min_distance))
        {
            //TODO: fix this by shooting from center of player
        }
        else
        {
            if (Physics.SphereCast(ray, Radius, out hit, range)) //Target Acquired
            {
                CalculateRaycasts(parent, ray, Radius, out hit, range);                
            }
            
        }
    }
    void CalculateRaycasts(GameObject parent, Ray ray, float Radius, out RaycastHit hit, float range) {
        if (Radius < 0.2f)
        {
            hit = default;
            return;
        }

        if (Physics.SphereCast(ray, Radius, out hit, range)) //Target Acquired
        {



            if (hit.distance <= range)
            {
                //  Debug.Log(hit.collider.GetComponent<PhotonView>().Owner.GetPhotonTeam() + " + " + PhotonNetwork.LocalPlayer.GetPhotonTeam());
                PhotonNetwork.Instantiate(impact_explosion.name, hit.point, Quaternion.identity);
                //"if()" is a good name of a book
                if ((hit.collider.tag == "AI_Player" || hit.collider.tag == "Player") && hit.collider.GetComponent<Team_Handler>().GetTeam() != player.GetComponent<Team_Handler>().GetTeam())
                {
                    Player_Health ph = hit.collider.GetComponent<Player_Health>();
                    if (ph != null)
                    {
                        if (ph.current_health > 0)
                        {
                            ApplyDamage(ph, parent, hit);

                        }

                    }
                    else
                    {

                    }

                }
                else if ((hit.collider.tag == "AI_Head" || hit.collider.tag == "Player_Head") && hit.collider.GetComponentInParent<Team_Handler>().GetTeam() != player.GetComponent<Team_Handler>().GetTeam())
                {
                    Player_Health ph = hit.collider.GetComponentInParent<Player_Health>();
                    if (ph != null)
                    {
                        if (ph.current_health > 0)
                        {
                            ApplyDamage(ph, parent, hit);
                        }
                    }
                    else
                    {
                        Debug.Log("too short");
                    }


                }
                else {
                    CalculateRaycasts(parent, ray, Radius / 2f, out hit, range);
                }

            }
        }
    }
    protected virtual void ApplyDamage(Player_Health ph, GameObject parent, RaycastHit hit)
    {
        if (ph.current_health - damage <= 0)
        {
            PhotonNetwork.Instantiate(death_effect.name, hit.point, Quaternion.identity);
            Transform oldpos = hit.collider.transform;
            hit.collider.transform.position = new Vector3(9999, 9999, 9999);
            SpawnCoin(hit.transform.gameObject, hit.point);
            //ph.PlayerLastHit(pv.ViewID); //base this off of the gameObject, not ID
            parent.GetComponentInParent<Super_Bar>().ChangeAmount(35);


        }
        ph.Remove_Health(damage);
    }
}