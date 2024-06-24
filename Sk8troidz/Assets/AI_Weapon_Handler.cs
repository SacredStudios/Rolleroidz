using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class AI_Weapon_Handler : MonoBehaviour
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
    [SerializeField] GameObject super_ammo;
    [SerializeField] GameObject increment;
    [SerializeField] GameObject increment_parent;
    [SerializeField] PhotonView pv;
    [SerializeField] Super_Bar sb;
    public AudioSource sound;

    void Start()
    {
        GameObject weapon_list = GameObject.Find("WeaponList");
        weapon = weapon_list.GetComponent<Weapon_List>().all_weapon_list[0];
        curr_gun = Instantiate(weapon.instance, weapon_loc.transform);
        curr_gun.transform.position += weapon.offset;
        curr_gun.transform.parent = weapon_loc.transform;
        if (pv.IsMine)
        {
            weapon.player = this.gameObject;
            weapon.super.player = weapon.player;

        }
    }

    void Update()
    {
        time_last_shot += Time.deltaTime;
        if (weapon != null)
        {
           /* if (sb.slider.value >= 100 && weapon.isSuper == false)
            {
                weapon = weapon.super;
            } */

            if (shoot_delay < weapon.weapon_delay)
            {
                shoot_delay += Time.deltaTime;
            }
            if (Input.GetButton("Fire1"))
            {
                FireCheck();
            }
            if (time_last_shot > 1f && weapon_up)
            {
                weapon_up = false;
                StartCoroutine(Weapon_Down());

            }
        }
    }
    public void FireCheck()
    {
        if (shoot_delay >= weapon.weapon_delay && weapon.attack_cost <= cooldown.value && pv.IsMine)
        {
            cooldown.value -= weapon.attack_cost;
            shoot_delay = 0;
            Shoot_Weapon();

        }
    }

    public void Shoot_Weapon()
    {

        // pv.RPC("Play_Sound", RpcTarget.Others);
        time_last_shot = 0;
        // animator.Play("Gun Layer.Shoot", 2, 0);

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
                if (weapon.ammo > 0)
                    increment_parent.transform.GetChild(weapon.ammo).gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                if (weapon.ammo <= 0)
                {
                    // RemoveSuper();

                }
            }

        }
    }

        IEnumerator Weapon_Up()
        {
            //cs.Shake(weapon.shake, 0.25f);
            float i = 0;
            while (i <= 1)
            {
                yield return new WaitForSeconds(0.01f);
                i += 0.2f;
                animator.SetLayerWeight(2, i);
            }
            /*if (sb.slider.value >= 100 && !weapon.isSuper)
            {
                weapon = weapon.super;
            } */
            weapon.Shoot(curr_gun, particle_pos, explosion_pos);
            if (weapon.isSuper)
            {
                weapon.ammo -= 1;
                if (weapon.ammo > 0)
                    increment_parent.transform.GetChild(weapon.ammo).gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                if (weapon.ammo <= 0)
                {
                    //  RemoveSuper();
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
