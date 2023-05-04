
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Weapon : ScriptableObject
{
    public string weapon_name;
    public string weapon_description;
    public float weapon_delay;
    public float damage;
    public int attack_cost;
    public float knockback;
    public Vector3 offset;
    public PhotonView pv;
   


    public GameObject instance;
    //Add var for sound

    


   public virtual void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos) { }

   
}
