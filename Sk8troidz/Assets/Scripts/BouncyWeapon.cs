using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
[CreateAssetMenu(fileName = "new_weapon", menuName = "Scripts/BouncyWeapon", order = 1)]
public class BouncyWeapon : Weapon
{

    public GameObject ball;
    public GameObject particle_explosion;




    public override void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos)
    {
     
        GameObject ball_clone = PhotonNetwork.Instantiate(ball.name, particle_pos.transform.position, particle_pos.transform.rotation);
        Instantiate(particle_explosion, explosion_pos.transform.position, particle_pos.transform.rotation);
    }


}





