using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Slot : MonoBehaviour
{
    public Weapon weapon;
    public GameObject curr_gun;
    [SerializeField] GameObject spine; //rename to spine
    [SerializeField] GameObject weapon_loc;
    [SerializeField] GameObject particle_pos;
    [SerializeField] GameObject explosion_pos;

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            
            weapon.Shoot(curr_gun);
        }
    }
    private void Awake()
    {
        weapon.spine = spine;
        weapon.particle_pos = this.particle_pos;
        weapon.explosion_pos = this.explosion_pos;
        curr_gun = Instantiate(weapon.instance, weapon_loc.transform);

    }
}
