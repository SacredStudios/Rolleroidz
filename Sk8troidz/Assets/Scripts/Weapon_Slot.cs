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
    float shoot_delay;

    private void Update()
    {
    
        if (shoot_delay < weapon.weapon_delay)
        {
            shoot_delay += Time.deltaTime;
        }
        if (Input.GetButton("Fire1") && shoot_delay >= weapon.weapon_delay)
        {
            shoot_delay = 0;
            Shoot_Weapon();                             
        }
    }

    void Shoot_Weapon()
    {
        weapon.Shoot(curr_gun, spine, particle_pos, explosion_pos);
    }
    private void Awake()
    {                       
        curr_gun = Instantiate(weapon.instance, weapon_loc.transform);
    }
}
