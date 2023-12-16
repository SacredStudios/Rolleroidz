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
    



    // MAKE GAME LIKE NSMBDS MULTIPLAYER

    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
        //  Debug.DrawRay(parent.transform.position, particle_pos.transform.up * range, Color.green); //chage this to capsulecast
        


        Ray ray = new Ray(parent.transform.position, particle_pos.transform.up);
        RaycastHit hit = new RaycastHit();
        PhotonNetwork.Instantiate(particle_trail.name, particle_pos.transform.position, particle_pos.transform.rotation);
        Instantiate(particle_explosion, explosion_pos.transform.position, particle_pos.transform.rotation);

        if (Physics.Raycast(ray, out hit)) //Target Acquired
        {


            if (hit.distance <= range)
            {
                //Debug.Log(hit.collider.tag);
                //  Debug.Log(hit.collider.GetComponent<PhotonView>().Owner.GetPhotonTeam() + " + " + PhotonNetwork.LocalPlayer.GetPhotonTeam());
                PhotonNetwork.Instantiate(impact_explosion.name, hit.point, Quaternion.identity);
                //"if()" is a good name of a book
                if (hit.collider.tag == "Player" && hit.collider.GetComponent<PhotonView>().Owner.GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
                {
                    Player_Health ph = hit.collider.GetComponent<Player_Health>();
                    if (ph != null)
                    {
                        if (ph.current_health - damage <= 0)
                        {
                            SpawnPoint(hit.collider.gameObject);
                            parent.GetComponentInParent<Super_Bar>().ChangeAmount(25);

                        }
                        ph.Remove_Health(damage);

                    }
                    else
                    {
                        Debug.Log("hit");
                    }
                    //call function from individual joints
                    // Debug.Log(hit.collider.gameObject.GetComponent<PhotonView>().Owner.GetPhotonTeam() + "+" + pv.Owner.GetPhotonTeam());

                }
                else if (hit.collider.tag == "Player_Head" && hit.collider.GetComponentInParent<PhotonView>().Owner.GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
                {
                    Player_Health ph = hit.collider.GetComponentInParent<Player_Health>();
                    if (ph != null)
                    {
                        if (ph.current_health - damage <= 0)
                        {
                            SpawnPoint(hit.collider.gameObject);
                            parent.GetComponentInParent<Super_Bar>().ChangeAmount(25);

                        }
                        ph.Remove_Health(damage * 1.5f);
                    }
                    else
                    {
                        Debug.Log("hit");
                    }
                    //Debug.Log("HeadShot");

                }
            }
        }
        
    }
    
    }



        
       
