using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class AI_Weapon_Handler : MonoBehaviour
{

    public Weapon weapon;
    public Weapon temp_weapon;
    public GameObject curr_gun;
    [SerializeField] HoldButton btn;
    [SerializeField] GameObject weapon_loc;
    [SerializeField] GameObject particle_pos;
    [SerializeField] GameObject explosion_pos;
    [SerializeField] Animator animator;
    float shoot_delay;
    float time_last_shot;
    bool weapon_up;
    [SerializeField] Slider cooldown;
    [SerializeField] GameObject super_ammo;
    [SerializeField] GameObject increment;
    [SerializeField] GameObject increment_parent;
    [SerializeField] PhotonView pv;
    public AudioSource sound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject weapon_list = GameObject.Find("WeaponList");
        weapon = weapon_list.GetComponent<Weapon_List>().all_weapon_list[0];
        curr_gun = Instantiate(weapon.instance, weapon_loc.transform);
        curr_gun.transform.position += weapon.offset;

    }

    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    void SetWeapon(string currname, int viewID)
    {
/*
        GameObject weapon_list = GameObject.Find("WeaponList");
        GameObject player = PhotonView.Find(viewID).gameObject;
        //GameObject loc = player.GetComponent<Weapon_Handler>().weapon_loc;
        foreach (Weapon w in weapon_list.GetComponent<Weapon_List>().all_weapon_list)
        {
            //Photon Hashtable might be more efficient. Yes Photon has a custom version of Hashtable. This was not a typo.
            if (w.name == currname)
            {
                curr_gun = Instantiate(w.instance, loc.transform);
                curr_gun.transform.position += weapon.offset;
                weapon = w;
                sound.clip = w.sound;
            }
        }
        for (int i = 1; i < weapon_loc.transform.childCount; i++)
        {
            Destroy(weapon_loc.transform.GetChild(i).gameObject);
        }
*/

    }
}
