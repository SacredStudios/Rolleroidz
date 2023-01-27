using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Handler : MonoBehaviour
{
    public Weapon weapon;
    public GameObject curr_gun;
   
    [SerializeField] GameObject weapon_loc;
    [SerializeField] GameObject particle_pos;
    [SerializeField] GameObject explosion_pos;
    [SerializeField] Animator animator;
    float shoot_delay;
    float time_last_shot;
 

    private void Update()
    {
        time_last_shot += Time.deltaTime;
        if (shoot_delay < weapon.weapon_delay)
        {
            shoot_delay += Time.deltaTime;
        }
        if (Input.GetButton("Fire1") && shoot_delay >= weapon.weapon_delay)
        {
            shoot_delay = 0;
            Shoot_Weapon();                             
        }
        if (time_last_shot > 5)
        {
            //putweapondown anim
            animator.SetLayerWeight(2, 0);
        }
    }

    void Shoot_Weapon()
    {
        time_last_shot = 0;
        weapon.Shoot(curr_gun, particle_pos, explosion_pos);
        if (animator.GetLayerWeight(2) == 0)
        {

            
            StartCoroutine(Shoot_Anim());
        }
    }
    IEnumerator Shoot_Anim()
    {
        float i = 0;
        while(i<=1)
        {
            yield return new WaitForSeconds(0.01f);
            i += 0.2f;
            animator.SetLayerWeight(2, i);
            
        }
        
    }
    private void Awake()
    {
       
        curr_gun = Instantiate(weapon.instance, weapon_loc.transform);
    }
}
