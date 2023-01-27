using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ScriptableObject
{
    public string weapon_name;
    public string weapon_description;
    public float weapon_delay;
    public int damage;
    public int max_ammo;
    public float knockback;
    public Vector3 offset;
   

    public GameObject instance;
    //Add var for sound

    
    public GameObject impact_effect;


   public virtual void Shoot(GameObject parent, GameObject particle_pos, GameObject explosion_pos) { }

   
}
