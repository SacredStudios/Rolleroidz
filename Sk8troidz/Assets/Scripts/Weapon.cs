using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ScriptableObject
{
    public string weapon_name;
    public string weapon_description;
    public float weapon_Speed;
    public int damage;
    public int max_ammo;
    public float knockback;
    public float range;
    //Add var for sound

    public GameObject particle_trail1;
    public GameObject particle_trail2;
    public GameObject impact_effect;
    

    // Update is called once per frame
   public virtual void Activate(GameObject parent) { }

    public virtual void Deactivate(GameObject parent) { }
}
