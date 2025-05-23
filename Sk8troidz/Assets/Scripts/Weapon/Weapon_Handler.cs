using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class Weapon_Handler : MonoBehaviourPunCallbacks
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
    [SerializeField] Slider cooldown; //TODO Add this slider from the playerassets gameobject
    [SerializeField] GameObject super_ammo;
    [SerializeField] GameObject increment;
    [SerializeField] GameObject increment_parent;
    [SerializeField] PhotonView pv;
    public AudioSource sound;
    private KeyCode shootKey;



    public bool isOverTrickBtn; //checks if mouse is hovering over a trick btn
    Super_Bar sb;

    CameraShake cs;

    private void Update()
    {
        time_last_shot += Time.deltaTime;
        if (weapon != null)
        {
            if (sb.slider.value >= 100 && weapon.isSuper == false)
            {

                weapon = weapon.super;
                weapon.player = this.gameObject;
                

                if (!super_ammo.activeSelf)
                { 
                    super_ammo.SetActive(true);
                    
                    
                    if (increment_parent.transform.childCount == 0)
                    {
                        for (int i = 0; i < weapon.max_ammo; i++)
                        {
                            GameObject increment_clone = Instantiate(increment, increment_parent.transform);
                            increment_clone.SetActive(true);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < weapon.max_ammo; i++)
                        {
                            increment_parent.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(255, 255, 225, 100);
                        }
                    }
                }
            }
            if (shoot_delay < weapon.weapon_delay)
            {
                shoot_delay += Time.deltaTime;
            }
            if (Input.GetKey(shootKey) || Input.GetMouseButton(0) || btn.isDown)
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
    bool TypingIntoInput()
    {
        if (EventSystem.current == null) return false;               // no EventSystem in scene
        var go = EventSystem.current.currentSelectedGameObject;
        if (go == null) return false;                      // nothing focused

        // Standard UI InputField
        if (go.GetComponent<UnityEngine.UI.InputField>() != null) return true;
        // TextMeshPro InputField
        if (go.GetComponent<TMPro.TMP_InputField>() != null) return true;

        return false;
    }
    public void FireCheck() //Check if you even are allowed to shoot yet
    {
        if (weapon != null && this.gameObject.GetComponent<PlayerMovement>().enabled)
        {
            if (shoot_delay >= weapon.weapon_delay && weapon.attack_cost <= cooldown.value && pv.IsMine)
            {
                cooldown.value -= weapon.attack_cost;
                shoot_delay = 0;
                Shoot_Weapon();

            }
        }
    }

    public void Shoot_Weapon()
    {
        cs.Shake(weapon.shake, 0.25f);
        sound.Play();
        pv.RPC("Play_Sound", RpcTarget.Others);
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
                if (weapon.ammo > 0)
                    increment_parent.transform.GetChild(weapon.ammo).gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                if (weapon.ammo <= 0)
                {
                    RemoveSuper();

                }
            }

        }
    }
    public void RemoveSuper()
    {
        sb.ChangeAmount(-(sb.slider.value / 2));
        super_ammo.SetActive(false);
        weapon = temp_weapon;
        weapon.super.ammo = weapon.super.max_ammo;
    }
    IEnumerator Weapon_Up()
    {
        cs.Shake(weapon.shake, 0.25f);
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
            if(weapon.ammo > 0)
              increment_parent.transform.GetChild(weapon.ammo).gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
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
            cs = GetComponent<CameraShake>();
            curr_gun = Instantiate(weapon.instance, weapon_loc.transform);
            curr_gun.transform.position += weapon.offset;
            
            temp_weapon = weapon;
            sb = GetComponent<Super_Bar>();
            sound.clip = weapon.sound;
            weapon.super.ammo = weapon.super.max_ammo;
            
            weapon.pv = this.pv;
            if (pv.IsMine)
            {
                weapon.player = this.gameObject;
                weapon.super.player = weapon.player;
                pv.RPC("SetWeapon", RpcTarget.Others, weapon.name, pv.ViewID);
                GetComponent<PlayerMovement>().maxSpeedBase -= weapon.weight;
                shootKey = (KeyCode)PlayerPrefs.GetInt("ShootKey", (int)KeyCode.Q);
            }
           
        }

        curr_gun.transform.parent = weapon_loc.transform;
        
    }
    [PunRPC]
    public void Play_Sound()
    {
        sound.Play();
    }
    [PunRPC] void SetWeapon(string currname, int viewID)
    {

        GameObject weapon_list = GameObject.Find("WeaponList");
        GameObject player = PhotonView.Find(viewID).gameObject;
        GameObject loc = player.GetComponent<Weapon_Handler>().weapon_loc;
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


    }


}
