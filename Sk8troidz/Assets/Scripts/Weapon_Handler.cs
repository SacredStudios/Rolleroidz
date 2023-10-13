using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Weapon_Handler : MonoBehaviourPunCallbacks
{
    public Weapon weapon;
    public Weapon temp_weapon;
    public GameObject curr_gun;
   
    [SerializeField] GameObject weapon_loc;
    [SerializeField] GameObject particle_pos;
    [SerializeField] GameObject explosion_pos;
    [SerializeField] Animator animator;
    float shoot_delay;
    float time_last_shot;
    bool weapon_up;
    [SerializeField] Slider cooldown;
    [SerializeField] PhotonView pv;

 
    public bool isOverTrickBtn; //checks if mouse is hovering over a trick btn
    Super_Bar sb;


    private void Update()
    {
        time_last_shot += Time.deltaTime;
        if (weapon != null)
        {
            if (sb.slider.value >= 100 && weapon.isSuper == false)
            {
                weapon = weapon.super;
            }
            if (shoot_delay < weapon.weapon_delay)
            {
                shoot_delay += Time.deltaTime;
            }
            if (Input.GetButton("Fire1") && shoot_delay >= weapon.weapon_delay && weapon.attack_cost <= cooldown.value && pv.IsMine && !isOverTrickBtn)
            {
                cooldown.value -= weapon.attack_cost;
                shoot_delay = 0;
                Shoot_Weapon();
            }
            if (time_last_shot > 1f && weapon_up)
            {
                weapon_up = false;
                StartCoroutine(Weapon_Down());

            }
        }
    }

    void Shoot_Weapon()
    {
        time_last_shot = 0;
        animator.Play("Gun Layer.Shoot", 2, 0);
       
        if (animator.GetLayerWeight(2) <= 0.5)
        {
            weapon_up = true;
            StartCoroutine(Weapon_Up());           
        }
        else
        {
            
            weapon.Shoot(curr_gun, particle_pos, explosion_pos);
            if (weapon.isSuper)
            {
                
                weapon.ammo -= 1;
                if (weapon.ammo <= 0)
                {
                    RemoveSuper();

                }
            }

        }
    }
    public void RemoveSuper()
    {
        sb.ChangeAmount(-100);       
        weapon = temp_weapon;
        weapon.super.ammo = weapon.super.max_ammo;
    }
    IEnumerator Weapon_Up()
    {
        float i = 0;
        while(i<=1)
        {
            yield return new WaitForSeconds(0.01f);
            i += 0.2f;
            animator.SetLayerWeight(2, i);           
        }
        if (sb.slider.value >= 100 && !weapon.isSuper)
        {
            weapon = weapon.super;
        }
        weapon.Shoot(curr_gun, particle_pos, explosion_pos);
        if (weapon.isSuper)
        {
            weapon.ammo -= 1;
            if (weapon.ammo <= 0)
            {
                RemoveSuper();
            }
        }
        yield return null;
    }
    IEnumerator Weapon_Down()
    {
        float i = 1;
        while (i >= 0)
        {
            yield return new WaitForSeconds(0.03f);
            i -= 0.1f;
            animator.SetLayerWeight(2, i);

        }
        yield return null;
    }
    private void Start()
    {
        if (weapon != null)
        {
            curr_gun = Instantiate(weapon.instance, weapon_loc.transform);
            curr_gun.transform.position += weapon.offset;
            pv.RPC("SetWeapon", RpcTarget.Others, weapon.name, pv.ViewID);
            temp_weapon = weapon;
            sb = GetComponent<Super_Bar>();
            weapon.super.ammo = weapon.super.max_ammo;
        }

        curr_gun.transform.parent = weapon_loc.transform;
        //Debug.Log(weapon.name + " + " +weapon.instance.name);
        weapon.pv = this.pv;
        weapon.player = this.gameObject;
    }
    [PunRPC] void SetWeapon(string currname, int viewID)
    {
        GameObject weapon_list = GameObject.Find("WeaponList");
        GameObject player = PhotonView.Find(viewID).gameObject;
        //Debug.Log(player.name);
        GameObject loc = player.GetComponent<Weapon_Handler>().weapon_loc;
        Debug.Log(loc.name);
        foreach (Weapon w in weapon_list.GetComponent<Weapon_List>().all_weapon_list)
        {
            //Photon Hashtable might be more efficient. Yes Photon has a custom version of Hashtable. This was not a typo.
            if (w.name == currname)
            {
                curr_gun = Instantiate(w.instance, loc.transform);
                curr_gun.transform.position += weapon.offset;
                weapon = w;
            }
        }
        
    }
    

}
