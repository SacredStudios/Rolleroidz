using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Weapon_Handler : MonoBehaviourPunCallbacks
{
    public Weapon weapon;
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
 

    private void Update()
    {
        time_last_shot += Time.deltaTime;
        if (shoot_delay < weapon.weapon_delay)
        {
            shoot_delay += Time.deltaTime;
        }
        if (Input.GetButton("Fire1") && shoot_delay >= weapon.weapon_delay && weapon.attack_cost<=cooldown.value && pv.IsMine)
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
        }
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
        weapon.Shoot(curr_gun, particle_pos, explosion_pos);
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
        //curr_gun = PhotonNetwork.Instantiate(weapon.instance.name, new Vector3(0, 0, 0), Quaternion.identity);
            //pun call to instantiate weapon (like coins)
           
            
        if (weapon != null)
        {
            curr_gun = Instantiate(weapon.instance, weapon_loc.transform);
            pv.RPC("SetWeapon", RpcTarget.Others, weapon.name, pv.ViewID);
        }
        

      
        curr_gun.transform.parent = weapon_loc.transform;
        Debug.Log(weapon.name + " + " +weapon.instance.name);
        weapon.pv = this.pv;
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
            Debug.Log("w.name"); //Photon Hashtable might be more efficient. Yes Photon has a custom version of Hashtable. This was not a typo.
            if (w.name == currname)
            {
                Debug.Log("found it");
                curr_gun = Instantiate(w.instance, loc.transform);
                weapon = w;
            }
        }
        
    }

}
